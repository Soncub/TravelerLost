using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CrystalLight : MonoBehaviour
{
    private PlayerController player;

    [Tooltip("How fast the crystal light rotates when moved by the player during interaction")]
    [SerializeField] private float rotationSpeed;

    [Tooltip("Input for interacting with the crystal light")]
    [SerializeField] private InputActionReference interactAction;
    [Tooltip("Input for moving the crystal light")]
    [SerializeField] private InputActionReference motionAction;
    private float input;
    private bool interacting = false;

    [Tooltip("Maximum distance for the player to interact with the crystal light")]
    [SerializeField] private float maxInteractDistance;
    [Tooltip("Maximum distance for the raycast")]
    [SerializeField] private float raycastDistance = 50f;

    [Tooltip("Layer mask for raycast to target other pillars")]
    [SerializeField] private LayerMask pillarLayerMask;



    [Tooltip("Pop up text")]
    public TextMeshProUGUI popUp;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (interacting)
        {
            // Rotate crystal light
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * input, Space.World);

            // Shoot raycast
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, pillarLayerMask))
            {
                PillarLogic pillarLogic = hit.collider.GetComponent<PillarLogic>();
              //  if (pillarLogic != null && pillarLogic.HasCrystal() && !pillarLogic.IsLit)
               // {
               //     pillarLogic.LightUp();
               // }
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!interacting && context.performed)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= maxInteractDistance)
            {
                interacting = true;
                player.DisablePlayerController();
            }
        }

        if (interacting && context.canceled)
        {
            interacting = false;
            player.EnablePlayerController();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (interacting)
            input = context.ReadValue<Vector2>().x;
    }
}
