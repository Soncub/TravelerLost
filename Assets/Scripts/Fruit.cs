using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Fruit : MonoBehaviour
{
    [Tooltip("Distance for offering the fruit (will change gizmo size to match)")]
    [SerializeField] private float offerDistance = 5.0f; // Distance to check if the creature is in range

    [SerializeField] private TextMeshProUGUI messageUI;  // UI to display messages
    [Tooltip("KeyBind for offering the fruit to the creature")]
    [SerializeField] private InputActionReference offerAction;    

    [SerializeField] private Transform player;          // Reference to the player
    [Tooltip("Visual of how far you can be to offer the fruit (green circle)")]
    [SerializeField] private bool enableGizmos;

    private CreatureController creature;                // Reference to the creature
    private bool isOffering = false;                    // Is the fruit currently being offered
    private ItemInteraction itemInteraction;            // Reference to ItemInteraction for pickup status

    public CharacterController characterController;
    public PlayerController playerMovement;
    public float dissapearTimer;
    private float time;

    private void Start()
    {
        creature = FindObjectOfType<CreatureController>();
        player = GameObject.Find("Player").transform;
        characterController = player.GetComponent<CharacterController>();

        offerAction.action.Enable();
        offerAction.action.performed += OfferFruit;

        itemInteraction = GetComponent<ItemInteraction>();

        if (messageUI != null)
            messageUI.gameObject.SetActive(false);

        playerMovement = player.GetComponent<PlayerController>();
    }
    public void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                playerMovement.EnablePlayerController();
            }
        }
    }

    private void OnDestroy()
    {
        offerAction.action.Disable();
        offerAction.action.performed -= OfferFruit;
    }

    private void OfferFruit(InputAction.CallbackContext context)
    {
        if (context.performed && !isOffering && itemInteraction != null && itemInteraction.itemIsPicked && IsGrounded())
        {
            float distanceToCreature = Vector3.Distance(player.position, creature.transform.position);

            if (distanceToCreature <= offerDistance)
            {
                StartCoroutine(OfferFruitCoroutine());
            }
        }
    }

    private IEnumerator OfferFruitCoroutine()
    {
        isOffering = true;

        // Disable player movement
        if (playerMovement != null)
        {
            playerMovement.DisablePlayerController();
            time = dissapearTimer;
        }

        // Show offering animation or state (optional)
        ShowMessage("Offering the fruit...");

        // Calculate the position in front of the player
        Vector3 frontOfPlayer = player.position + player.forward * 2f; // Adjust the distance as needed

        // Command the creature to approach the position in front of the player
        creature.NewTargetDestination(frontOfPlayer);

        // Wait for the creature to reach the calculated position
        while (Vector3.Distance(frontOfPlayer, creature.transform.position) > 3f) // Tolerance distance
        {
            yield return null;
        }

        // Creature eats the fruit
        Destroy(gameObject); // Destroy the currently held fruit only

        // Re-enable player movement
        if (playerMovement != null)
        {
            playerMovement.EnablePlayerController();
            time = 0;
        }

        isOffering = false;
    }

    private void ShowMessage(string message)
    {
        if (messageUI != null)
        {
            messageUI.gameObject.SetActive(true);
            messageUI.text = message;
            StartCoroutine(HideMessageAfterDelay());
        }
    }

    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (messageUI != null)
        {
            messageUI.gameObject.SetActive(false);
            messageUI.text = string.Empty;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (enableGizmos)
        {
            Gizmos.DrawWireSphere(transform.position, offerDistance);
        }
    }
    private bool IsGrounded() => characterController.isGrounded;
}
