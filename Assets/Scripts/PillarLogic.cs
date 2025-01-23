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

    // Track pillars that are currently being hit by this pillar's raycast
    private HashSet<PillarLogic> hitPillars = new HashSet<PillarLogic>();

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
        if (itemDivot != null)
        {
            itemDivot.PlaceItemEvent.RemoveListener(OnItemPlaced);
            itemDivot.ReleaseItemEvent.RemoveListener(OnItemReleased);
        }
    }

    private void Update()
    {
        // If this pillar is lit, cast a ray to check for other pillars
        if (isLit)
        {
            CastRayToManagePillars();
        }
    }

    private void OnItemPlaced()
    {
        if (isFirst)
        {
            // The first pillar automatically lights up when an item is placed
            LightUpPillar();
        }
    }

    private void OnItemReleased()
    {
        // Disable the pillar object and stop all interactions
        UnlightPillar();
    }

    private void CastRayToManagePillars()
    {
        // Cast a ray forward from the lit pillar's position
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));

        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance, pillarLayerMask))
        {
            // Check if the hit object has a PillarLogic component
            PillarLogic otherPillar = hit.collider.GetComponent<PillarLogic>();
            if (otherPillar != null && otherPillar.itemDivot.ItemIsPlaced)
            {
                // If not already tracked, light up the other pillar
                if (!hitPillars.Contains(otherPillar))
                {
                    otherPillar.LightUpPillar();
                    hitPillars.Add(otherPillar);
                }
            }
        }

        // Unlight pillars no longer hit by the raycast
        HashSet<PillarLogic> toRemove = new HashSet<PillarLogic>(hitPillars);
        foreach (var pillar in toRemove)
        {
            // Check if the raycast is no longer hitting this pillar
            if (!Physics.Raycast(ray, out hit, raycastDistance, pillarLayerMask) || hit.collider.GetComponent<PillarLogic>() != pillar)
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

    private void OnDrawGizmos()
    {
        if (isLit)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawLine(transform.position, transform.position + transform.forward * raycastDistance);
    }
}
