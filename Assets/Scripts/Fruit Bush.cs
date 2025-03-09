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

    [Tooltip("List of possible item prefabs to spawn")]
    [SerializeField] private GameObject[] itemPrefabs;

    [Tooltip("Cooldown time in seconds before another item can be spawned")]
    [SerializeField] private float spawnCooldown = 2.0f;

    [Tooltip("Time it takes for the item to grow to full size")]
    [SerializeField] private float growDuration = 1.0f;

    private Transform player;
    private Transform spawnPoint;
    private bool canSpawn = true;
    private float cooldownTimer;
    private GameObject currentSpawnedItem;

    // UI Variables
    public GameObject canvas;
    public Transform childObject;
    public TextMeshProUGUI popUp;
    public PauseMenuManager pause;
    [SerializeField] private PlayerInput playerInput;
    private string controlScheme;

    private void Start()
    {
        canvas = GameObject.Find("MessageCanvas");
        childObject = canvas.transform.Find("BushMessage");
        popUp = childObject.GetComponent<TextMeshProUGUI>();
        popUp.gameObject.SetActive(false);
        pause = GameObject.Find("Pause Menu").GetComponent<PauseMenuManager>();

        spawnPoint = transform.Find("SpawnPoint");
        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint child object not found.", this);
            return;
        }

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

        itemInteraction = FindFirstObjectByType<ItemInteraction>();
        if (itemInteraction == null)
        {
            Debug.LogError("ItemInteraction component is missing from this GameObject.", this);
            return;
        }

        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogError("Item prefabs list is empty.", this);
            return;
        }

        pickUpAction.Enable();
        pickUpAction.performed += SpawnItem;
        playerInput.onControlsChanged += (input) => UpdateControlScheme();
    }

    private void Update()
    {
        if (!canSpawn)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canSpawn = true;
            }
        }

        if (currentSpawnedItem != null)
        {
            float distanceToItem = Vector3.Distance(currentSpawnedItem.transform.position, transform.position);
            if (distanceToItem > pickUpDistance)
            {
                currentSpawnedItem = null;
            }
        }
    }

    private void SpawnItem(InputAction.CallbackContext context)
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (currentSpawnedItem != null)
        {
            Debug.Log("Cannot spawn new item until the previous one is moved out of range.");
            return;
        }

        if (context.performed && !itemInteraction.itemIsPicked && distanceToPlayer <= pickUpDistance && canSpawn)
        {
            GameObject selectedPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            GameObject newSpawnedItem = Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = newSpawnedItem.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
            }

            StartCoroutine(GrowItem(newSpawnedItem));
            currentSpawnedItem = newSpawnedItem;
            canSpawn = false;
            cooldownTimer = spawnCooldown;
        }
        else
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

        item.transform.localScale = targetScale;
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    private void OnDestroy()
    {
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

    public void PopUpOn(string notification)
    {
        popUp.gameObject.SetActive(true);
        popUp.text = notification;
    }

    public void PopUpOff()
    {
        popUp.gameObject.SetActive(false);
        popUp.text = "";
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
        popUp.text = controlScheme == "Keyboard and Mouse" ? "Press E to get a fruit" : "Press A to get a fruit";
    }
}
