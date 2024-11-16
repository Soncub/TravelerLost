using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemDivot : MonoBehaviour
{
    [Tooltip("Reference to the empty GameObject where the item will be placed when dropped")]
    [SerializeField] private Transform dropLocation;
    [Tooltip("Enables Player to be able to place an item here.")]
    [SerializeField] private bool canBePlaced;
    [Tooltip("Is one of the Required placements.")]
    [SerializeField] public bool isKey;

    public UnityEvent PlaceItemEvent;
    public UnityEvent ReleaseItemEvent; // New event for item removal

    private bool itemIsPlaced = false;
    public bool ItemIsPlaced => itemIsPlaced;
    private GameObject placedItem;
    private bool locked;

    private void Update()
    {
        if (itemIsPlaced && placedItem.transform.parent != dropLocation)
        {
            ReleaseItem(null); // Call release without passing an ItemInteraction reference
        }
    }

    public void PlaceItem(GameObject item, ItemInteraction itemInteraction)
    {
        if (!itemIsPlaced)
        {
            placedItem = item;
            Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
            item.transform.position = dropLocation.position;
            item.transform.rotation = dropLocation.rotation;
            item.transform.SetParent(dropLocation); // Lock item to the location
            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = true; // Set to kinematic when placed
            }
            itemIsPlaced = true;
            Debug.Log("Item placed in divot.");
            PlaceItemEvent.Invoke(); // Trigger event when item is placed
        }
    }

    public void ReleaseItem(ItemInteraction itemInteraction)
    {
        if (itemIsPlaced)
        {
            Rigidbody itemRigidbody = placedItem.GetComponent<Rigidbody>();

            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = false; // Set back to non-kinematic when released
            }

            itemIsPlaced = false;
            placedItem = null;
            Debug.Log("Item picked up from divot.");
            ReleaseItemEvent.Invoke(); // Trigger event when item is released
        }
    }

    public bool CanPlaceItem()
    {
        return !itemIsPlaced;
    }
}
