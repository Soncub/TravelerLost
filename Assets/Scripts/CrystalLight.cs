using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrystalLogic : MonoBehaviour
{
    // --- Pillar Logic Fields ---
    [Tooltip("Reference to the ItemDivot this logic is tied to")]
    [SerializeField] private ItemDivot itemDivot;

    [Tooltip("GameObject to enable when the item is placed in the divot")]
    [SerializeField] private GameObject pillarObject;

    [Tooltip("Distance for the raycast to check for other pillars")]
    [SerializeField] private float pillarRaycastDistance = 10f;

    [Tooltip("LayerMask for detecting other pillars")]
    [SerializeField] private LayerMask pillarLayerMask;

    [Tooltip("Is this the first pillar in the chain?")]
    [SerializeField] private bool isFirst = false;

    private bool isLit = false;
    private HashSet<CrystalLogic> hitPillars = new HashSet<CrystalLogic>();

    // --- Crystal Light Fields ---
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
            Debug.LogError("ItemDivot reference is not set in PillarAndCrystalLogic.");
            return;
        }

        if (pillarObject == null)
        {
            Debug.LogError("PillarObject reference is not set in PillarAndCrystalLogic.");
            return;
        }

        itemDivot.PlaceItemEvent.AddListener(OnItemPlaced);
        itemDivot.ReleaseItemEvent.AddListener(OnItemReleased);

        pillarObject.SetActive(false);

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
        {
            LightUpPillar();
        }
    }

    private void OnItemReleased()
    {
        UnlightPillar();
    }

    private void CastRayToManagePillars()
    {
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        Debug.DrawRay(transform.position, transform.forward * pillarRaycastDistance, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, pillarRaycastDistance, pillarLayerMask))
        {
            CrystalLogic otherPillar = hit.collider.GetComponent<CrystalLogic>();
            if (otherPillar != null && otherPillar.itemDivot.ItemIsPlaced)
            {
                if (!hitPillars.Contains(otherPillar))
                {
                    otherPillar.LightUpPillar();
                    hitPillars.Add(otherPillar);
                }
            }
        }

        HashSet<CrystalLogic> toRemove = new HashSet<CrystalLogic>(hitPillars);
        foreach (var pillar in toRemove)
        {
            if (!Physics.Raycast(ray, out hit, pillarRaycastDistance, pillarLayerMask) || hit.collider.GetComponent<CrystalLogic>() != pillar)
            {
                pillar.UnlightPillar();
                hitPillars.Remove(pillar);
            }
        }
    }

    public void LightUpPillar()
    {
        if (!isLit)
        {
            isLit = true;
            pillarObject.SetActive(true);
            Debug.Log($"{name} is now lit up.");
        }
    }

    public void UnlightPillar()
    {
        if (isLit)
        {
            isLit = false;
            pillarObject.SetActive(false);
            Debug.Log($"{name} is now unlit.");
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
