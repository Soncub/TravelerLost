using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerItemInteraction : MonoBehaviour
{
    // Detection radius for picking up items
    [SerializeField] private float detectionRadius = 5f;
    public Items_ScriptableObject Items;

    private GameObject closestItem;
    private bool CanPickUp;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void Start()
    {
        CanPickUp = false;
    }

    private void Update()
    {
        DetectItemsInRange();

        // Check to see if you can pick up the object
        if (CanPickUp && Input.GetKeyDown(KeyCode.F))
        {
            ItemPickUp();
        }
    }

    // Detects if any item is within the detection radius
    void DetectItemsInRange()
    {
        // Find all colliders within the detection radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        float closestDistance = Mathf.Infinity;
        closestItem = null;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Item"))
            {
                float distanceToItem = Vector3.Distance(transform.position, collider.transform.position);

                if (distanceToItem < closestDistance)
                {
                    closestDistance = distanceToItem;
                    closestItem = collider.gameObject;
                }
            }
        }

        // If an item is within range, set CanPickUp to true
        if (closestItem != null)
        {
            CanPickUp = true;
        }
        else
        {
            CanPickUp = false;
        }
    }

    // Logic for picking up the item
    void ItemPickUp()
    {
        Debug.Log("Attempting to pick up object: " + closestItem.name);

        CanPickUp = false; // Resets pick up status after the interaction
    }
}
