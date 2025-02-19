using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FruitBush : MonoBehaviour
{
    ItemInteraction itemInteraction;

    [Tooltip("Distance for picking up object (will change gizmo size to match)")]
    [SerializeField] private float pickUpDistance;

    [Tooltip("Visual for Distance for spawning object in")]
    [SerializeField] private bool enableGizmos;

    [SerializeField] private InputAction pickUpAction;

    [Tooltip("The prefab of the item to spawn")]
    [SerializeField] private GameObject itemPrefab;

    [Tooltip("Cooldown time in seconds before another item can be spawned")]
    [SerializeField] private float spawnCooldown = 2.0f;

    [Tooltip("Time it takes for the item to grow to full size")]
    [SerializeField] private float growDuration = 1.0f;

    private Transform player;
    private Transform spawnPoint;
    private bool canSpawn = true;
    private float cooldownTimer;
    private GameObject currentSpawnedItem;
    //UI Variable
    public GameObject canvas;
    public Transform childObject;
    public TextMeshProUGUI popUp;
    public PauseMenuManager pause;
    [SerializeField] private PlayerInput playerInput;
    private string controlScheme;

    private void Start()
    {
        //UI Assign
        canvas = GameObject.Find("MessageCanvas");
        childObject = canvas.transform.Find("BushMessage");
        popUp = childObject.GetComponent<TextMeshProUGUI>();
        //UI Script
        popUp.gameObject.SetActive(false);
        pause = GameObject.Find("Pause Menu").GetComponent<PauseMenuManager>();
        // Ensure the SpawnPoint exists
        spawnPoint = transform.Find("SpawnPoint");
        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint child object not found.", this);
            return;
        }

        // Ensure the Player exists
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player GameObject not found.", this);
            return;
        }

        // Ensure ItemInteraction is found
        itemInteraction = FindFirstObjectByType<ItemInteraction>();
        if (itemInteraction == null)
        {
            Debug.LogError("ItemInteraction component is missing from this GameObject.", this);
            return;
        }

        // Ensure itemPrefab is assigned
        if (itemPrefab == null)
        {
            Debug.LogError("Item prefab is not assigned.", this);
            return;
        }

        // Enable the Input Action and subscribe to it
        pickUpAction.Enable();
        pickUpAction.performed += SpawnItem;
        playerInput.onControlsChanged += (input) => UpdateControlScheme();
    }

    private void Update()
    {
        //UI Variable
        /*float range = Vector3.Distance(player.transform.position, transform.position);
        if (range <= pickUpDistance)
        {
            PopUpOn("Press E to Get a Fruit");
            if (pause.isPaused == true)
            {
                PopUpOff();
            }
        }
        else
        {
            PopUpOff();
        }*/
        // Handle cooldown timer
        if (!canSpawn)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canSpawn = true;
            }
        }

        // Check if the current spawned item is still within pickup distance
        if (currentSpawnedItem != null)
        {
            float distanceToItem = Vector3.Distance(currentSpawnedItem.transform.position, transform.position);
            if (distanceToItem > pickUpDistance)
            {
                currentSpawnedItem = null; // Allow new item to be spawned if it's moved away
            }
        }
    }

    private void SpawnItem(InputAction.CallbackContext context)
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // Prevent spawning if there is still an item within the pickup distance
        if (currentSpawnedItem != null)
        {
            Debug.Log("Cannot spawn new item until the previous one is moved out of range.");
            return;
        }

        if (context.performed && !itemInteraction.itemIsPicked && distanceToPlayer <= pickUpDistance && canSpawn)
        {
            if (itemPrefab != null)
            {
                // Create a new item each time SpawnItem is called
                GameObject newSpawnedItem = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
                Rigidbody rb = newSpawnedItem.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = true; // Disable physics during growth
                }

                StartCoroutine(GrowItem(newSpawnedItem));
                Debug.Log("Item spawned at spawn point.");

                // Track the currently spawned item
                currentSpawnedItem = newSpawnedItem;

                // Start cooldown
                canSpawn = false;
                cooldownTimer = spawnCooldown;
            }
            else
            {
                Debug.LogError("Item prefab is not assigned.", this);
            }
        }
        else if (context.performed && (itemInteraction.itemIsPicked || distanceToPlayer >= pickUpDistance || !canSpawn))
        {
            Debug.Log("Cannot spawn item.");
        }
    }

    private IEnumerator GrowItem(GameObject item)
    {
        float elapsedTime = 0f;
        Vector3 targetScale = item.transform.localScale;
        item.transform.localScale = Vector3.zero;

        while (elapsedTime < growDuration)
        {
            item.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime / growDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the item reaches its final size
        item.transform.localScale = targetScale;

        // Re-enable the Rigidbody to allow it to fall
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    private void OnDestroy()
    {
        // Disable the Input Action and unsubscribe to prevent memory leaks
        pickUpAction.Disable();
        pickUpAction.performed -= SpawnItem;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (enableGizmos)
        {
            Gizmos.DrawWireSphere(this.transform.position, pickUpDistance);
        }
    }
    //UI Script
    public void PopUpOn(string notification)
    {
        popUp.gameObject.SetActive(true);
        popUp.text = notification;
    }
    public void PopUpOff()
    {
        popUp.gameObject.SetActive(false);
        popUp.text = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            UpdateControlScheme();
            popUp.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            popUp.gameObject.SetActive(false);
        }
    }
    private void UpdateControlScheme()
    {
        controlScheme = playerInput.currentControlScheme;
        if (controlScheme == "Keyboard and Mouse")
        {
            popUp.text = "Press E to get a fruit";
        }
        else if (controlScheme == "Gamepad")
        {
            popUp.text = "Press A to get a fruit";
        }
    }
}
