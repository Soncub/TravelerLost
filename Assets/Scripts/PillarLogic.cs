using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarLogic : MonoBehaviour
{
    [Tooltip("Reference to the ItemDivot this logic is tied to")]
    [SerializeField] private ItemDivot itemDivot;

    [Tooltip("GameObject to enable when the item is placed in the divot")]
    [SerializeField] private GameObject SpotLight;

    private void Start()
    {
        if (itemDivot == null)
        {
            Debug.LogError("ItemDivot reference is not set in PillarLogic.");
            return;
        }

        if (SpotLight == null)
        {
            Debug.LogError("PillarObject reference is not set in PillarLogic.");
            return;
        }

        // Subscribe to the ItemDivot's PlaceItemEvent and ReleaseItemEvent
        itemDivot.PlaceItemEvent.AddListener(OnItemPlaced);
        itemDivot.ReleaseItemEvent.AddListener(OnItemReleased);

        // Ensure the pillar object starts disabled
        SpotLight.SetActive(false);
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

    private void OnItemPlaced()
    {
        // Enable the pillar object when an item is placed
        SpotLight.SetActive(true);
    }

    private void OnItemReleased()
    {
        // Disable the pillar object when an item is removed
        SpotLight.SetActive(false);
    }
}
