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
    //Ui Variable
    public TextMeshProUGUI popUp;

    private void Start()
    {
        //UI Assign
        popUp = transform.Find("Canvas/Message").GetComponent<TextMeshProUGUI>();
        itemInteraction = FindFirstObjectByType<ItemInteraction>();
        player = GameObject.Find("Player").transform;
        spawnPoint = transform.Find("SpawnPoint");
        //UI Script
        popUp.gameObject.SetActive(false);

        // Ensure spawn point exists
        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint child object not found.", this);
            return;
        }

        // Enable the Input Action and subscribe to it
        pickUpAction.Enable();
        pickUpAction.performed += SpawnItem;

        if (itemInteraction == null)
        {
            Debug.LogError("ItemInteraction component is missing from this GameObject.", this);
            return;
        }
    }

    private void Update()
    {
        //UI Script
        float range = Vector3.Distance(player.position, transform.position);
        if (range <= pickUpDistance)
        {
            PopUpOn("Press E to Create Fruit");
        }
        else
        {
            PopUpOff();
        }
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
                GameObject spawnedItem = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);

                // Disable the Rigidbody to prevent it from falling
                Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }

                StartCoroutine(GrowItem(spawnedItem));
                Debug.Log("Item spawned at spawn point.");

                // Track the currently spawned item
                currentSpawnedItem = spawnedItem;

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
}
