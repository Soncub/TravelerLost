using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class CreatureDoor : MonoBehaviour
{
    public GameObject creatureDoor;
    public GameObject lever;
    public float moveSpeed = 3f;
    public float maxHeight = 6f;
    //public NavMeshAgent creatureAgent;
    private Vector3 movePosition;
    private Vector3 initialPosition;
    private bool isOpening = false;
    private bool doorFullyOpened = false;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private float interactDistance;
    private Transform player;
    public TextMeshProUGUI popUp;
    [SerializeField] private PlayerInput playerInput;
    private string controlScheme;
    private bool stopLever;
    public AudioSource sound;
    public AudioClip clip;

    void Start()
    {
        initialPosition = creatureDoor.transform.position;
        movePosition = new Vector3(0.0f, maxHeight, 0.0f);
        interactAction.action.Enable();
        interactAction.action.performed += Flick;
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player GameObject not found.", this);
            return;
        }
        playerInput.onControlsChanged += (input) => UpdateControlScheme();
        popUp.gameObject.SetActive(false);
        sound.clip = clip;
        sound.loop = false;
    }

    void Update()
    {
        if (isOpening && !doorFullyOpened)
        {
            float step = moveSpeed * Time.deltaTime;
            creatureDoor.transform.position = Vector3.MoveTowards(creatureDoor.transform.position, initialPosition + movePosition, step);


            if (creatureDoor.transform.position == initialPosition + movePosition)
            {
                doorFullyOpened = true;
                isOpening = false;
                sound.loop = false;
                /*creatureAgent.isStopped = false;*/
            }
        }
    }

    public void Flick(InputAction.CallbackContext context)
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (context.performed && distanceToPlayer <= interactDistance && stopLever == false)
        {
            lever.transform.position += new Vector3(0f, -1.2f, 0f);
            lever.transform.Rotate(0f, 0f, 80f, Space.Self);
            isOpening = true;
            stopLever = true;
            sound.Play();
            sound.loop = true;
        }
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
            popUp.text = "Press Q to pull the lever";
        }
        else if (controlScheme == "Gamepad")
        {
            popUp.text = "Press X to pull the lever";
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Creature") && !isOpening && !doorFullyOpened)
        {
            Debug.Log("Open door.");
            isOpening = true;
            creatureAgent.isStopped = true;
        }
    }*/
}
