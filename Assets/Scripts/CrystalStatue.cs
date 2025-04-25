using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrystalStatue : ChargeSource
{
    // --- Charging and Light Logic Fields ---
    [Tooltip("Reference to the ItemDivot this logic is tied to")]
    [SerializeField] private ItemDivot itemDivot;

    [Tooltip("Distance for the raycast to check for other pillars")]
    [SerializeField] private float pillarRaycastDistance = 10f;

    [Tooltip("LayerMask for detecting other pillars")]
    [SerializeField] private LayerMask pillarLayerMask;

    [Tooltip("Is this the first pillar in the chain and should light up without a source?")]
    [SerializeField] private bool isFirst = false;

    private HashSet<ChargeSource> hitPillars = new HashSet<ChargeSource>();

    // --- Rotation and Input Fields ---
    private PlayerController player;

    [Tooltip("How fast the crystal statue rotates when moved by the player during interaction")]
    [SerializeField] private float rotationSpeed;

    [Tooltip("Input for interacting with the crystal statue")]
    [SerializeField] private InputActionReference interactAction;

    [Tooltip("Input for moving the crystal statue")]
    [SerializeField] private InputActionReference motionAction;

    private float input;
    private bool interacting = false;

    [Tooltip("Maximum distance for the player to interact with the crystal statue")]
    [SerializeField] private float maxInteractDistance;

    public Animator animator;
    int isRotatingHash;
    bool isRotating;
    int isReverseHash;
    bool isReverse;
    public AudioSource lightUp;

    [Tooltip("Pop up text")]
    public GameObject canvas;
    public Transform childObject;
    public TextMeshProUGUI popUp;
    public PauseMenuManager pause;
    [SerializeField] private PlayerInput playerInput;
    private string controlScheme;

    private void Start()
    {
        canvas = GameObject.Find("MessageCanvas");
        childObject = canvas.transform.Find("StatueMessage");
        popUp = childObject.GetComponent<TextMeshProUGUI>();
        popUp.gameObject.SetActive(false);
        pause = GameObject.Find("Pause Menu").GetComponent<PauseMenuManager>();
        isRotatingHash = Animator.StringToHash("IsRotating");
        isReverseHash = Animator.StringToHash("IsReverse");
        animator = GameObject.Find("MC Animations1").GetComponent<Animator>();
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
            //Debug.Log("Item Divot reference is not set in CrystalStatue.");
            return;
        }

        if (beamObject == null)
        {
            //Debug.Log("Beam Object reference is not set in CrystalStatue.");
            return;
        }

        itemDivot.PlaceItemEvent.AddListener(OnItemPlaced);
        itemDivot.ReleaseItemEvent.AddListener(OnItemReleased);

        beamObject.SetActive(false);
        if (darkObject != null)
        darkObject.SetActive(true);

        // Input Actions
        if (interactAction != null) interactAction.action.performed += Interact;
        if (interactAction != null) interactAction.action.canceled += Interact;
        if (motionAction != null) motionAction.action.performed += Move;
        playerInput.onControlsChanged += (input) => UpdateControlScheme();
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
        isRotating = animator.GetBool(isRotatingHash);
        isReverse = animator.GetBool(isReverseHash);
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
    public void OnItemPlaced()
    {
        if (isFirst)
            Charge();
        else
            CastRayToManagePillars();
        UpdateBeam();
    }

    public void OnItemReleased()
    {
        UpdateBeam();
    }

    private void CastRayToManagePillars()
    {
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        Debug.DrawRay(transform.position, transform.forward * pillarRaycastDistance, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, pillarRaycastDistance, pillarLayerMask))
        {
            ChargeSource otherPillar = hit.collider.GetComponent<ChargeSource>();
            if (otherPillar != null)
            {
                if (!hitPillars.Contains(otherPillar))
                {
                    otherPillar.Charge();
                    hitPillars.Add(otherPillar);
                }
            }
        }

        HashSet<ChargeSource> toRemove = new HashSet<ChargeSource>(hitPillars);
        foreach (var pillar in toRemove)
        {
            if (!Physics.Raycast(ray, out hit, pillarRaycastDistance, pillarLayerMask) || hit.collider.GetComponent<ChargeSource>() != pillar)
            {
                pillar.Uncharge();
                hitPillars.Remove(pillar);
            }
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
                animator.SetBool(isRotatingHash, true);
                animator.speed = 0;
            }
        }

        if (interacting && context.canceled)
        {
            animator.speed = 1;
            animator.SetBool(isRotatingHash, false);
            animator.SetBool(isReverseHash, false);
            interacting = false;
            player.EnablePlayerController();
        }
    }

    private void Move(InputAction.CallbackContext context)
    {
        if (interacting)
        {
            input = context.ReadValue<Vector2>().x;
            if (input == 0)
            {
                animator.SetBool(isReverseHash, false);
                animator.speed = 0;
            }
            else if (motionAction.action.ReadValue<Vector2>().x < -0.2f/*Input.GetKeyDown(KeyCode.LeftArrow)*/)
            {
                animator.speed = 1;
                animator.SetBool(isReverseHash, true);
            }
            else if (motionAction.action.ReadValue<Vector2>().x > 0.2f/*Input.GetAxisRaw("Horizontal") > 0*/)
            {
                animator.speed = 1;
            }
        }
    }

    public override void Charge()
    {
        if (!isLit)
        {
            isLit = true;
            UpdateBeam();
        }
    }

    public override void Uncharge()
    {
        if (isLit)
        {
            isLit = false;
            UpdateBeam();
        }
    }

    public void UpdateBeam()
    {
        if ((isLit || isFirst) && itemDivot.ItemIsPlaced)
        {
            if (!beamObject.activeSelf)
            {
                beamObject.SetActive(true);
                lightUp.Play();
                if (darkObject != null)
                    darkObject.SetActive(false);
                //Debug.Log($"{name} is now lit up.");
            }
        } else
        {
            if (beamObject.activeSelf)
            {
                beamObject.SetActive(false);
                if (darkObject != null)
                    darkObject.SetActive(true);
                //Debug.Log($"{name} is now unlit.");
                foreach (var pillar in hitPillars)
                {
                    pillar.Uncharge();
                    hitPillars.Remove(pillar);
                }
            }
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

        Gizmos.DrawLine(transform.position, transform.position + transform.forward * pillarRaycastDistance);
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
            popUp.text = "Hold Q to interact with the statue";
        }
        else if (controlScheme == "Gamepad")
        {
            popUp.text = "Hold X to interact with the statue";
        }
    }
}
