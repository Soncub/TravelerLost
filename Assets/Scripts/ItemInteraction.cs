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

    [SerializeField] private float pickUpDistance;
    [SerializeField] private float placeOffset = 1.0f;
    [SerializeField] private bool enableGizmos;

    public bool itemIsPicked;
    private Rigidbody rb;

    [SerializeField] private InputActionReference pickUpAction;
    public GameObject canvas;
    public Transform childObject;
    public TextMeshProUGUI popUp;
    public Transform childObject2;
    public TextMeshProUGUI popUp2;
    public PauseMenuManager pause;
    [SerializeField] private PlayerInput playerInput;
    private string controlScheme;
    public Animator animator;
    public int anilayer = 2;
    public WhistleSystem whistleSystem;
    public Transform respawnPoint;
    public AudioSource pickUpSound;

    // NEW: Only allow pickup after grown
    public bool canBePickedUp = false;

    private void Start()
    {
        canvas = GameObject.Find("MessageCanvas");
        childObject = canvas.transform.Find("ItemMessage");
        popUp = childObject.GetComponent<TextMeshProUGUI>();
        if (gameObject.tag == "Item")
        {
            childObject2 = canvas.transform.Find("FruitMessage");
            popUp2 = childObject2.GetComponent<TextMeshProUGUI>();
        }
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        pickUpPoint = GameObject.Find("PickUpPoint").transform;
        popUp.gameObject.SetActive(false);

        pickUpAction.action.Enable();
        pickUpAction.action.performed += PickUp;
        playerInput = GameObject.Find("PlayerInput").GetComponent<PlayerInput>();
        playerInput.onControlsChanged += (input) => UpdateControlScheme();
        animator = GameObject.Find("MC Animations1").GetComponent<Animator>();
        whistleSystem = GameObject.Find("Player").GetComponent<WhistleSystem>();
        if (respawnPoint == null)
            respawnPoint = GameObject.Find("RespawnPoint").transform;
    }

    public void PickUp(InputAction.CallbackContext context)
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (context.performed && distanceToPlayer <= pickUpDistance && !itemIsPicked && pickUpPoint.childCount < 1 && !whistleSystem.whistling && canBePickedUp)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.detectCollisions = false;
            this.transform.position = pickUpPoint.position;
            this.transform.parent = pickUpPoint;

            itemIsPicked = true;
            PickUpEvent.Invoke();
            animator.SetTrigger("PickUp");
            animator.SetLayerWeight(anilayer, 0.8f);
            pickUpSound.Play();
            if (gameObject.tag == "Item")
            {
                UpdateControlScheme();
                popUp2.gameObject.SetActive(true);
            }
        }
        else if (itemIsPicked && context.performed)
        {
            Vector3 placePosition = player.position + player.forward * placeOffset + player.up * -placeOffset;
            this.transform.position = placePosition;
            this.transform.parent = null;
            rb.useGravity = true;
            rb.detectCollisions = true;
            itemIsPicked = false;
            DropEvent.Invoke();
            animator.SetTrigger("Place");
            animator.SetLayerWeight(anilayer, 0f);
            if (gameObject.tag == "Item")
            {
                UpdateControlScheme();
                popUp2.gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Divot"))
        {
            ItemDivot divot = collision.gameObject.GetComponent<ItemDivot>();
            if (divot != null)
            {
                divot.PlaceItem(gameObject, this);
            }
        }
    }

    private void OnDestroy()
    {
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
        if (other.gameObject.CompareTag("Respawn"))
        {
            Respawn();
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
        popUp.text = controlScheme == "Keyboard and Mouse" ? "Press E to pick up the item" : "Press A to pick up the item";
        if (gameObject.tag == "Item")
        {
            popUp2.text = controlScheme == "Keyboard and Mouse" ? "Press R to offer fruit" : "Press LT to offer fruit";
        }
    }

    private void Respawn()
    {
        this.transform.position = respawnPoint.position;
    }
}
