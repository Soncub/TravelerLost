using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FruitBush : MonoBehaviour
{
    [SerializeField] private float pickUpDistance;
    [SerializeField] private bool enableGizmos;
    [SerializeField] private InputAction pickUpAction;
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private float spawnCooldown = 2.0f;
    [SerializeField] private float growDuration = 1.0f;

    private Transform player;
    private Transform spawnPoint;
    private bool canSpawn = true;
    private float cooldownTimer;
    private List<GameObject> spawnedItems = new List<GameObject>();

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
        player = GameObject.Find("Player").transform;

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
                canSpawn = true;
        }

        spawnedItems.RemoveAll(item => item == null || Vector3.Distance(item.transform.position, transform.position) > pickUpDistance);
    }

    private void SpawnItem(InputAction.CallbackContext context)
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (context.performed && distanceToPlayer <= pickUpDistance && canSpawn)
        {
            GameObject selectedPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            GameObject newItem = Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = newItem.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            StartCoroutine(GrowItem(newItem));
            spawnedItems.Add(newItem);
            canSpawn = false;
            cooldownTimer = spawnCooldown;
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
            rb.isKinematic = false;

        ItemInteraction itemInteraction = item.GetComponent<ItemInteraction>();
        if (itemInteraction != null)
            itemInteraction.canBePickedUp = true; 
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
            Gizmos.DrawWireSphere(this.transform.position, pickUpDistance);
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
