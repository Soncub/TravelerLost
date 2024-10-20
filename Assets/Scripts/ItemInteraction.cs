using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemInteraction : MonoBehaviour
{
    private Transform pickUpPoint;
    private Transform player;

    [Tooltip("Distance for picking up object (will change gizmo size to match)")]
    [SerializeField] private float pickUpDistance;

    [Tooltip("Amount of force added when throwing")]
    [SerializeField] private float forceMulti;

    [Tooltip("Visual for Distance for picking up object")]
    [SerializeField] private bool enableGizmos;

    public bool itemIsPicked;
    private Rigidbody rb;

    // Reference to the Input Action
    [SerializeField] private InputAction pickUpAction;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        pickUpPoint = GameObject.Find("PickUpPoint").transform;

        // Enable the Input Action and subscribe to it
        pickUpAction.Enable();
        pickUpAction.performed += PickUp;
    }

 

    public void PickUp(InputAction.CallbackContext context)
    {


        // Calculate distance dynamically
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (context.performed && distanceToPlayer <= pickUpDistance && !itemIsPicked && pickUpPoint.childCount < 1)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero; // Stop object movement when picked up
            this.transform.position = pickUpPoint.position;
            this.transform.parent = pickUpPoint;

            itemIsPicked = true;
        }
        else if (itemIsPicked && context.performed)
        {
            rb.AddForce(player.transform.forward * forceMulti, ForceMode.Impulse);
            this.transform.parent = null;
            rb.useGravity = true;
            itemIsPicked = false;
        }
    }

    private void OnDestroy()
    {
        // Disable the Input Action and unsubscribe to prevent memory leaks
        pickUpAction.Disable();
        pickUpAction.performed -= PickUp;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (enableGizmos)
        {
            Gizmos.DrawWireSphere(this.transform.position, pickUpDistance);
        }
    }
}
