using System.Collections;
using System.Collections.Generic;
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
    [Tooltip("Minimum rotation for jarring noise that causes the creature to lose focus (0-360)")]
    [SerializeField] private float minBadRotation;
    [Tooltip("Maximum rotation for jarring noise that causes the creature to lose focus (0-360)")]
    [SerializeField] private float maxBadRotation;
    [Tooltip("Maximum distance for the creature to react to the jarring noise and lose focus")]
    [SerializeField] private float maxBadDistance;
    private float curRotation;

    [Tooltip("Input for interacting with the statue")]
    [SerializeField] private InputActionReference interactAction;
    [Tooltip("Input for moving the statue")]
    [SerializeField] private InputActionReference motionAction;
    private float input;

    void Start()
    {
        //Set Variable Defaults
        player = FindFirstObjectByType<PlayerController>();
        creature = FindFirstObjectByType<CreatureController>();
        curRotation = transform.localRotation.eulerAngles.y;
        updateTimer = updateTime;

        //Enable and subscribe to the actions
        interactAction.action.Enable();
        interactAction.action.performed += Interact;
        interactAction.action.canceled += Interact;
        motionAction.action.Enable();
        motionAction.action.performed += Move;
        motionAction.action.canceled += Move;
    }

    private void Update()
    {
        updateTimer -= Time.deltaTime;
        if (updateTimer < 0)
        {
            //On a timer, either call or distract the creature if its in range based on rotation
            updateTimer += updateTime;
            if (
                CheckAngle(curRotation, minBadRotation, maxBadRotation) &&
                Vector3.Distance(player.transform.position, creature.transform.position) <= maxBadDistance
                )
                creature.LoseFocus();
            else if (
                CheckAngle(curRotation, minWhistleRotation, maxWhistleRotation) &&
                Vector3.Distance(player.transform.position, creature.transform.position) <= maxWhistleDistance
                )
                creature.NewTargetDestination(callPosition.position);
        }
        if (interacting)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * input, Space.World);
            curRotation = transform.localRotation.eulerAngles.y;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        //When pressed and in range, start interaction and disable player movement so inputs are used only for this
        if (!interacting && context.performed)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= maxInteractDistance)
            {
                interacting = true;
                player.DisablePlayerController();
            }
        }
        //When unpressed, stop interaction and re-enable player movement
        if (interacting && context.canceled)
        {
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
}
