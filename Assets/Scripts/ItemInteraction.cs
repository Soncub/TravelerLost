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
    [SerializeField] private InputActionReference pickUpAction;
    //Ui Variable
    public GameObject canvas;
    public Transform childObject;
    public TextMeshProUGUI popUp;
    public PauseMenuManager pause;
    [SerializeField] private PlayerInput playerInput;
    private string controlScheme;

    private void Start()
    {
        //UI Assign
        canvas = GameObject.Find("MessageCanvas");
        childObject = canvas.transform.Find("ItemMessage");
        popUp = childObject.GetComponent<TextMeshProUGUI>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        pickUpPoint = GameObject.Find("PickUpPoint").transform;
        //UI Script
        popUp.gameObject.SetActive(false);
        //pause = GameObject.Find("Pause Menu").GetComponent<PauseMenuManager>();

        // Enable the Input Action and subscribe to it
        pickUpAction.action.Enable();
        pickUpAction.action.performed += PickUp;
        playerInput = GameObject.Find("PlayerInput").GetComponent<PlayerInput>();
        playerInput.onControlsChanged += (input) => UpdateControlScheme();
    }
    //UI Script
    private void Update()
    {
        /*
        float range = Vector3.Distance(player.position, transform.position);
        if (range <= pickUpDistance)
        {
            if (gameObject.tag == "Crystal")
            {
                PopUpOn("Press E to Pick Up Crystal");
                if (pause.isPaused == true)
                {
                    PopUpOff();
                }
            }
            else if (gameObject.tag == "Item")
            {
                PopUpOn("Press E to Pick Up Fruit");
                if (pause.isPaused == true)
                {
                    PopUpOff();
                }
            }
        }
        else
        {
            PopUpOff();
        }
        */
    }

    public void PickUp(InputAction.CallbackContext context)
    {
        // Calculate distance dynamically
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (context.performed && distanceToPlayer <= pickUpDistance && !itemIsPicked && pickUpPoint.childCount < 1)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero; // Stop object movement when picked up
            rb.detectCollisions = false;
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
            rb.detectCollisions |= true;
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
        pickUpAction.action.Disable();
        pickUpAction.action.performed -= PickUp;
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            UpdateControlScheme();
            popUp.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            popUp.gameObject.SetActive(false);
        }
    }
    private void UpdateControlScheme()
    {
        controlScheme = playerInput.currentControlScheme;
        if (controlScheme == "Keyboard and Mouse")
        {
            popUp.text = "Press E to pick up the item";
        }
        else if (controlScheme == "Gamepad")
        {
            popUp.text = "Press A to pick up the item";
        }
    }
}
