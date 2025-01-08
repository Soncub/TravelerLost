using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrystalLight : MonoBehaviour
{
    private PlayerController player;

    [Tooltip("How fast the crystal light rotates when moved by the player during interaction")]
    [SerializeField] private float rotationSpeed;

    [Tooltip("Input for interacting with the crystal light")]
    [SerializeField] private InputActionReference interactAction;
    [Tooltip("Input for moving the crystal light")]
    [SerializeField] private InputActionReference motionAction;
    private float input;
    private bool interacting = false;

    [Tooltip("Maximum distance for the player to interact with the crystal light")]
    [SerializeField] private float maxInteractDistance;
    [Tooltip("Minimum rotation for lighting darkness (0-360)")]
    [SerializeField] private float minGoalRotation;
    [Tooltip("Maximum rotation for lighting darkness (0-360)")]
    [SerializeField] private float maxGoalRotation;
    private float curRotation;

    [Tooltip("Darkness to toggle off when pointed in the goal direction (which would otherwise scare the creature)")]
    [SerializeField] private GameObject darkObject;
    [Tooltip("Light to toggle when powered on")]
    [SerializeField] private GameObject lightObject;

    [Tooltip("Pop up text")]
    public TextMeshProUGUI popUp;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        //UI Variable
        float range = Vector3.Distance(player.transform.position, transform.position);
        if (range <= maxInteractDistance)
        {
            //Should have popups added for when the player holds a crystal and is in range 
            if (!interacting)
                PopUpOn("Press Left Shift to Interact with Light");
            else
                PopUpOn("Move Left or Right to Rotate the Light");
        }
        else
        {
            PopUpOff();
        }
        if (interacting)
        {
            //Angle Movement
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * input, Space.World);
            curRotation = transform.localRotation.eulerAngles.y;
            //Angle checking (add boolean that makes sure its powered when checking)
            if (CheckAngle(curRotation, minGoalRotation, maxGoalRotation))
            {
                darkObject.SetActive(false);
            } 
            //If not in the right rotation, make sure the dark object is active
            else if(!darkObject.activeSelf)
            {
                darkObject.SetActive(true);
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
        return min < max ? (check >= min) && (check <= max) : !((check < min) && (check > max));
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
}