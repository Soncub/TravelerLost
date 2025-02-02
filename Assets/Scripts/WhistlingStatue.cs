using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;

public class WhistlingStatue : MonoBehaviour
{
    [Tooltip("Position to call creature to when in a whistling position")]
    [SerializeField] private Transform callPosition;
    [Tooltip("How often the whistling or jarring noise should attract the creature to callPosition or cause it to lose focus respectively")]
    [SerializeField] private float updateTime;
    [Tooltip("How fast the statue rotates when moved by the player during interaction")]
    [SerializeField] private float rotationSpeed;
    private float updateTimer;

    private PlayerController player;
    private CreatureController creature;
    private bool interacting = false;

    [Tooltip("Maximum distance for the player to interact with the statue")]
    [SerializeField] private float maxInteractDistance;
    [Tooltip("Minimum rotation for whistling (0-360)")]
    [SerializeField] private float minWhistleRotation;
    [Tooltip("Maximum rotation for whistling (0-360)")]
    [SerializeField] private float maxWhistleRotation;
    [Tooltip("Maximum distance for the creature to hear the whistling and be attracted to the call position")]
    [SerializeField] private float maxWhistleDistance;
    [Tooltip("Particles to toggle on when whistling")]
    [SerializeField] private GameObject whistleParticles;
    [Tooltip("Minimum rotation for jarring noise that causes the creature to lose focus (0-360)")]
    [SerializeField] private float minBadRotation;
    [Tooltip("Maximum rotation for jarring noise that causes the creature to lose focus (0-360)")]
    [SerializeField] private float maxBadRotation;
    [Tooltip("Maximum distance for the creature to react to the jarring noise and lose focus")]
    [SerializeField] private float maxBadDistance;
    [Tooltip("Particles to toggle on when making the jarring noise")]
    [SerializeField] private GameObject badParticles;
    private float curRotation;

    [Tooltip("Input for interacting with the statue")]
    [SerializeField] private InputActionReference interactAction;
    [Tooltip("Input for moving the statue")]
    [SerializeField] private InputActionReference motionAction;
    private float input;

    [Tooltip("Pop up text")]
    public GameObject canvas;
    public Transform childObject;
    public TextMeshProUGUI popUp;
    public PauseMenuManager pause;

    public AudioSource musicSource;
    public AudioSource happyWhistle;
    public AudioSource sadWhistle;
    public AudioSource swivel;
    [Tooltip("Whistling Noise")]
    [SerializeField] private AudioClip goodWhistle;
    [Tooltip("Bad Noise")]
    [SerializeField] private AudioClip badWhistle;

    public void Awake()
    {
        happyWhistle.time = musicSource.time;
    }

    void Start()
    {
        happyWhistle.Play();
        sadWhistle.Play();
        InvokeRepeating("AdjustTiming", 0f, 1f);
        //UI Assign
        canvas = GameObject.Find("MessageCanvas");
        childObject = canvas.transform.Find("StatueMessage");
        popUp = childObject.GetComponent<TextMeshProUGUI>();
        //Set Variable Defaults
        player = FindFirstObjectByType<PlayerController>();
        creature = FindFirstObjectByType<CreatureController>();
        //whistle = GetComponent<AudioSource>();
        //UI Script
        popUp.gameObject.SetActive(false);
        pause = GameObject.Find("Pause Menu").GetComponent<PauseMenuManager>();
        //Enable and subscribe to the actions
        interactAction.action.Enable();
        interactAction.action.performed += Interact;
        interactAction.action.canceled += Interact;
        motionAction.action.Enable();
        motionAction.action.performed += Move;
        motionAction.action.canceled += Move;
        //Angle Checking for audio and particles
        curRotation = transform.localRotation.eulerAngles.y;
        updateTimer = updateTime;
        if (CheckAngle(curRotation, minBadRotation, maxBadRotation))
        {
            sadWhistle.volume = 1f;
            happyWhistle.volume = 0.001f;
            whistleParticles.SetActive(false);
            badParticles.SetActive(true);
        }
        else if (CheckAngle(curRotation, minWhistleRotation, maxWhistleRotation))
        {
            sadWhistle.volume = 0.001f;
            happyWhistle.volume = 1f;
            whistleParticles.SetActive(true);
            badParticles.SetActive(false);
        }
        else
        {
            sadWhistle.volume = 0.001f;
            happyWhistle.volume = 0.001f;
            whistleParticles.SetActive(false);
            badParticles.SetActive(false);
        }
    }

    private void Update()
    {
        //UI Variable
        /*float range = Vector3.Distance(player.transform.position, transform.position);
        if (range <= maxInteractDistance)
        {
            if(!interacting)
            {
                PopUpOn("Press Q to Interact with Statue");
                if (pause.isPaused == true)
                {
                    PopUpOff();
                }
            }
            else
            {
                PopUpOn("Move Left or Right to Rotate the Statue");
                if (pause.isPaused == true)
                {
                    PopUpOff();
                }
            }

        }
        else
        {
            PopUpOff();
        }*/
        updateTimer -= Time.deltaTime;
        if (updateTimer < 0)
        {
            //On a timer, either call or distract the creature if its in range based on rotation
            updateTimer += updateTime;
            if (
                CheckAngle(curRotation, minBadRotation, maxBadRotation) &&
                Vector3.Distance(transform.position, creature.transform.position) <= maxBadDistance
                )
                creature.LoseFocus();
            else if (
                CheckAngle(curRotation, minWhistleRotation, maxWhistleRotation) &&
                Vector3.Distance(transform.position, creature.transform.position) <= maxWhistleDistance
                )
                creature.NewTargetDestination(callPosition.position);
        }
        if (interacting)
        {
            //Angle Checking for audio and particles
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * input, Space.World);
            curRotation = transform.localRotation.eulerAngles.y;
            if (CheckAngle(curRotation, minBadRotation, maxBadRotation))
            {
                sadWhistle.volume = 1f;
                happyWhistle.volume = 0.001f;
                whistleParticles.SetActive(false);
                badParticles.SetActive(true);
            }
            else if (CheckAngle(curRotation, minWhistleRotation, maxWhistleRotation))
            {
                sadWhistle.volume = 0.001f;
                happyWhistle.volume = 1f;
                whistleParticles.SetActive(true);
                badParticles.SetActive(false);
            }
            else
            {
                sadWhistle.volume = 0.001f;
                happyWhistle.volume = 0.001f;
                whistleParticles.SetActive(false);
                badParticles.SetActive(false);
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        //When pressed and in range, start interaction and disable player movement so inputs are used only for this
        if (!interacting && context.performed)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= maxInteractDistance)
            {
                swivel.Play();
                interacting = true;
                player.DisablePlayerController();
            }
        }
        //When unpressed, stop interaction and re-enable player movement
        if (interacting && context.canceled)
        {
            swivel.Stop();
            interacting = false;
            player.EnablePlayerController();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (interacting)
            input = context.ReadValue<Vector2>().x;
    }

    static private bool CheckAngle(float check, float min, float max)
    {
        return min < max ? (check >= min) && (check <= max) : ! ((check < min) && (check > max));
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
    public void AdjustTiming()
    {
         if (happyWhistle.time != musicSource.time)
         {
            if (musicSource.time < 16.410)
            {
                happyWhistle.time = musicSource.time;
            }
            else if (musicSource.time < 32.820)
            {
                happyWhistle.time = musicSource.time - 16.410f;
            }
            else if (musicSource.time < 49.230)
            {
                happyWhistle.time = musicSource.time - 32.820f;
            }
            else if (musicSource.time < 65.640)
            {
                happyWhistle.time = musicSource.time - 49.230f;
            }
            else if (musicSource.time < 82.050)
            {
                happyWhistle.time = musicSource.time - 65.640f;
            }
            else if (musicSource.time < 98.460)
            {
                happyWhistle.time = musicSource.time - 82.050f;
            }
            else if (musicSource.time < 114.870)
            {
                happyWhistle.time = musicSource.time - 98.460f;
            }
            else if (musicSource.time < 131.280)
            {
                happyWhistle.time = musicSource.time - 114.870f;
            }
         }
    }
}
