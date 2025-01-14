using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarLogic : MonoBehaviour
{
    [Tooltip("Reference to the ItemDivot this logic is tied to")]
    [SerializeField] private ItemDivot itemDivot;

    [Tooltip("GameObject to enable when the item is placed in the divot")]
    [SerializeField] private GameObject pillarObject;

    [Tooltip("Distance for the raycast to check for other pillars")]
    [SerializeField] private float raycastDistance = 10f;

    [Tooltip("LayerMask for detecting other pillars")]
    [SerializeField] private LayerMask pillarLayerMask;

    [Tooltip("Is this the first pillar in the chain?")]
    [SerializeField] private bool isFirst = false;

    private bool isLit = false;

    private void Start()
    {
        if (itemDivot == null)
        {
            Debug.LogError("ItemDivot reference is not set in PillarLogic.");
            return;
        }

        if (pillarObject == null)
        {
            Debug.LogError("PillarObject reference is not set in PillarLogic.");
            return;
        }

        // Subscribe to the ItemDivot's PlaceItemEvent and ReleaseItemEvent
        itemDivot.PlaceItemEvent.AddListener(OnItemPlaced);
        itemDivot.ReleaseItemEvent.AddListener(OnItemReleased);

        // Ensure the pillar object starts disabled
        pillarObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (itemDivot != null)
        {
            itemDivot.PlaceItemEvent.RemoveListener(OnItemPlaced);
            itemDivot.ReleaseItemEvent.RemoveListener(OnItemReleased);
        }
    }

    private void Update()
    {
        // Continuously check for connected pillars if this pillar is lit
        if (isLit || isFirst)
        {
            CheckForConnectedPillar();
        }
    }

    private void OnItemPlaced()
    {
        if (isFirst)
        {
            // The first pillar automatically lights up when an item is placed
            LightUpPillar();
        }
        else
        {
            // Check for connection to a lit pillar
            CheckForConnectedPillar();
        }
    }

    private void OnItemReleased()
    {
        // Disable the pillar object when an item is removed
        pillarObject.SetActive(false);
        isLit = false;
    }

    private void CheckForConnectedPillar()
    {
        // Cast a ray forward from the pillar's position
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));


        // Debugging: Show the ray in the scene for 1 second
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.green);

        // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance, pillarLayerMask))
        {
            // Check if the hit object has a PillarLogic component
            PillarLogic otherPillar = hit.collider.GetComponent<PillarLogic>();
            if (otherPillar != null && otherPillar.isLit)
            {
                // Light up this pillar if it's connected to a lit pillar
                LightUpPillar();
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

    // Gizmo to show the raycast in the scene view
    private void OnDrawGizmos()
    {
        // Set Gizmo color
        Gizmos.color = Color.green;

        // Draw a ray representing the raycast in the scene view
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * raycastDistance);
    }
}
