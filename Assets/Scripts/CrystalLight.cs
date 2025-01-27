using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrystalStatue : ChargeSource
{
    // --- Charging and Light Logic Fields ---
    [Tooltip("Reference to the ItemDivot this logic is tied to")]
    [SerializeField] private ItemDivot itemDivot;

    [Tooltip("Distance for the raycast to check for other pillars")]
    [SerializeField] private float pillarRaycastDistance = 10f;

    [Tooltip("LayerMask for detecting other pillars")]
    [SerializeField] private LayerMask pillarLayerMask;

    [Tooltip("Is this the first pillar in the chain and should light up without a source?")]
    [SerializeField] private bool isFirst = false;

    private HashSet<ChargeSource> hitPillars = new HashSet<ChargeSource>();

    // --- Rotation and Input Fields ---
    private PlayerController player;

    [Tooltip("How fast the crystal statue rotates when moved by the player during interaction")]
    [SerializeField] private float rotationSpeed;

    [Tooltip("Input for interacting with the crystal statue")]
    [SerializeField] private InputActionReference interactAction;

    [Tooltip("Input for moving the crystal statue")]
    [SerializeField] private InputActionReference motionAction;

    private float input;
    private bool interacting = false;

    [Tooltip("Maximum distance for the player to interact with the crystal statue")]
    [SerializeField] private float maxInteractDistance;

    private void Start()
    {
        interactAction.action.Enable();
        interactAction.action.performed += Interact;
        interactAction.action.canceled += Interact;
        motionAction.action.Enable();
        motionAction.action.performed += Move;
        motionAction.action.canceled += Move;
        // Initialize Player Controller
        player = FindFirstObjectByType<PlayerController>();

        // Pillar Logic Setup
        if (itemDivot == null)
        {
            Debug.LogError("ItemDivot reference is not set in CrystalStatue.");
            return;
        }

        if (beamObject == null)
        {
            Debug.LogError("PillarObject reference is not set in CrystalStatue.");
            return;
        }

        itemDivot.PlaceItemEvent.AddListener(OnItemPlaced);
        itemDivot.ReleaseItemEvent.AddListener(OnItemReleased);

        beamObject.SetActive(false);
        darkObject.SetActive(true);

        // Input Actions
        if (interactAction != null) interactAction.action.performed += Interact;
        if (interactAction != null) interactAction.action.canceled += Interact;
        if (motionAction != null) motionAction.action.performed += Move;
    }

    private void OnDestroy()
    {
        if (itemDivot != null)
        {
            itemDivot.PlaceItemEvent.RemoveListener(OnItemPlaced);
            itemDivot.ReleaseItemEvent.RemoveListener(OnItemReleased);
        }

        if (interactAction != null) interactAction.action.performed -= Interact;
        if (interactAction != null) interactAction.action.canceled -= Interact;
        if (motionAction != null) motionAction.action.performed -= Move;
    }

    private void Update()
    {
        // Pillar Logic Update
        if (isLit)
        {
            CastRayToManagePillars();
        }

        // Crystal Light Update
        if (interacting)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * input, Space.World);
        }
    }

    // --- Pillar Logic Methods ---
    private void OnItemPlaced()
    {
        if (isFirst)
            Charge();
        else
            CastRayToManagePillars();
    }

    private void OnItemReleased()
    {
        Uncharge();
    }

    private void CastRayToManagePillars()
    {
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        Debug.DrawRay(transform.position, transform.forward * pillarRaycastDistance, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, pillarRaycastDistance, pillarLayerMask))
        {
            CrystalStatue otherPillar = hit.collider.GetComponent<CrystalStatue>();
            if (otherPillar != null && otherPillar.itemDivot.ItemIsPlaced)
            {
                if (!hitPillars.Contains(otherPillar))
                {
                    otherPillar.Charge();
                    hitPillars.Add(otherPillar);
                }
            }
        }

        HashSet<ChargeSource> toRemove = new HashSet<ChargeSource>(hitPillars);
        foreach (var pillar in toRemove)
        {
            if (!Physics.Raycast(ray, out hit, pillarRaycastDistance, pillarLayerMask) || hit.collider.GetComponent<CrystalStatue>() != pillar)
            {
                pillar.Uncharge();
                hitPillars.Remove(pillar);
            }
        }
    }

    // --- Crystal Light Methods ---
    private void Interact(InputAction.CallbackContext context)
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

    private void Move(InputAction.CallbackContext context)
    {
        if (interacting)
            input = context.ReadValue<Vector2>().x;
    }
}
