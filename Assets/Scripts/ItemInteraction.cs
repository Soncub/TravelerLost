using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ItemInteraction : MonoBehaviour
{
    [SerializeField] UnityEvent PickUpEvent;
    [SerializeField] UnityEvent DropEvent;
    private Transform pickUpPoint;
    private Transform player;

    [Tooltip("Distance for picking up object (will change gizmo size to match)")]
    [SerializeField] private float pickUpDistance;

    [Tooltip("Offset distance when placing the object in front of the player")]
    [SerializeField] private float placeOffset = 1.0f;

    [Tooltip("Visual for Distance for picking up object")]
    [SerializeField] private bool enableGizmos;

    public bool itemIsPicked;
    private Rigidbody rb;

    // Reference to the Input Action
    [SerializeField] private InputAction pickUpAction;
    //Ui Variable
    public TextMeshProUGUI popUp;

    private void Start()
    {
        //UI Assign
        popUp = transform.Find("Canvas/Message").GetComponent<TextMeshProUGUI>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        pickUpPoint = GameObject.Find("PickUpPoint").transform;
        //UI Script
        popUp.gameObject.SetActive(false);

        // Enable the Input Action and subscribe to it
        pickUpAction.Enable();
        pickUpAction.performed += PickUp;
    }
    //UI Script
    private void Update()
    {
        float range = Vector3.Distance(player.position, transform.position);
        if (range <= pickUpDistance)
        {
            PopUpOn("Press E to Pick Up Fruit");
        }
        else
        {
            PopUpOff();
        }
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
            PickUpEvent.Invoke();
        }
        else if (itemIsPicked && context.performed)
        {
            Vector3 placePosition = player.position + player.forward * placeOffset;
            this.transform.position = placePosition;
            this.transform.parent = null;
            rb.useGravity = true;
            itemIsPicked = false;
            DropEvent.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Divot"))
        {
            // Access the ItemDivot script on the colliding GameObject
            ItemDivot divot = collision.gameObject.GetComponent<ItemDivot>();
            if (divot != null)
            {
                // Call PlaceItem method, passing in this GameObject and a reference to this ItemInteraction
                divot.PlaceItem(gameObject, this);
            }
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
    //UI Script
    public void PopUpOn(string notification)
    {
        popUp.gameObject.SetActive(true);
        popUp.text = notification;
    }
    public void PopUpOff()
    {
        popUp.gameObject.SetActive(false);
        popUp.text = null;
    }
}
