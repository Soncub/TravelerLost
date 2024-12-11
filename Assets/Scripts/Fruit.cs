using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Fruit : MonoBehaviour
{
    [SerializeField] private float offerDistance = 5.0f; // Distance to check if the creature is in range
    [SerializeField] private TextMeshProUGUI messageUI;  // UI to display messages
    [SerializeField] private InputAction offerAction;    // Input action to offer the fruit
    [SerializeField] private Transform player;          // Reference to the player
    private CreatureController creature;                // Reference to the creature
    private bool isOffering = false;                    // Is the fruit currently being offered

    private void Start()
    {
        creature = FindObjectOfType<CreatureController>();
        player = GameObject.Find("Player").transform;

        offerAction.Enable();
        offerAction.performed += OfferFruit;

        if (messageUI != null)
            messageUI.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        offerAction.Disable();
        offerAction.performed -= OfferFruit;
    }

    private void OfferFruit(InputAction.CallbackContext context)
    {
        if (context.performed && !isOffering)
        {
            float distanceToCreature = Vector3.Distance(player.position, creature.transform.position);

            if (distanceToCreature <= offerDistance)
            {
                StartCoroutine(OfferFruitCoroutine());
            }
            else
            {
                ShowMessage("The creature can't see you from afar.");
            }
        }
    }

    private IEnumerator OfferFruitCoroutine()
    {
        isOffering = true;

        // Disable player movement
        PlayerController playerMovement = player.GetComponent<PlayerController>();
        if (playerMovement != null)
            playerMovement.DisablePlayerController();

        // Show offering animation or state (optional)
        ShowMessage("Offering the fruit...");

        // Calculate the position in front of the player
        Vector3 frontOfPlayer = player.position + player.forward * 1.5f; // Adjust the distance as needed

        // Command the creature to approach the position in front of the player
        creature.NewTargetDestination(frontOfPlayer);

        // Wait for the creature to reach the calculated position
        while (Vector3.Distance(frontOfPlayer, creature.transform.position) > 0.5f) // Tolerance distance
        {
            yield return null;
        }

        // Creature eats the fruit
        ShowMessage("The creature happily eats the fruit.");
        Destroy(gameObject); // Destroy the fruit after it's eaten

        // Re-enable player movement
        if (playerMovement != null)
            playerMovement.EnablePlayerController();

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
}
