using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class WhistleSystem : MonoBehaviour
{
    [Tooltip("Should whistling be on")]
    [SerializeField] private bool whistlingEnabled = true;
    [Tooltip("UI object to move around for pointing where the whistle should fire")]
    [SerializeField] private GameObject whistleMarker;
    [SerializeField] private GameObject threeDWhistleMarker;
    [Tooltip("How fast the whistle point should move")]
    [SerializeField] private float whistleMoveSpeed = 1;
    [Tooltip("Camera used as a reference for whistling (should be the same one used when whistling)")]
    [SerializeField] private Camera refCamera;
    [Tooltip("The furthest you can whistle for the creature to 'follow' it")]
    [SerializeField] private float travelRange = 10;
    [Tooltip("The furthest the creature can be from the player to 'hear' it")]
    [SerializeField] private float listenRange = 10;
    [Tooltip("How close the pointer has to be to an interactable to command the creature to interact with it")]
    [SerializeField] private float interactableSnapRange = 10;

    private Vector3 markerAnchorPoint;
    private Vector2 input;
    public bool whistling;

    private CreatureController creature;
    private PlayerController player;
    public AudioClip[] soundEffects;
    public AudioSource whistleSound;
    private int currentIndex = 0;
    private LayerMask layer;
    [SerializeField] private PlayerInput playerInput;
    private string controlScheme;
    private float controllerMultiplier = 5;
    private float controllerWhistle;
    private float pcWhistle;
    public Animator animator;
    public float transitionSpeed = 1.0f;
    private float currentWeight = 0.0f;
    private float targetWeight = 1.0f;
    //public int anilayer = 1;

    private void Start()
    {
        layer = LayerMask.GetMask("Terrain");
        //Set the default position to be where the point starts
        markerAnchorPoint = whistleMarker.transform.position;
        //Find the player and creature
        creature = FindFirstObjectByType<CreatureController>();
        player = FindFirstObjectByType<PlayerController>();
        playerInput.onControlsChanged += (input) => controlScheme = playerInput.currentControlScheme;
        controllerWhistle = whistleMoveSpeed * controllerMultiplier;
        pcWhistle = whistleMoveSpeed;
        animator = GameObject.Find("MC Animations1").GetComponent<Animator>();
        targetWeight = 0.0f;
    }

    private void Update()
    {
        //When whistling, move the pointer based on input
        if (whistling && whistlingEnabled)
        {
            whistleMarker.transform.position += Time.deltaTime * whistleMoveSpeed * (Vector3)input;
            if (Physics.Raycast(refCamera.ScreenPointToRay(whistleMarker.transform.position), out RaycastHit hit, Mathf.Infinity,layer))
            {
                threeDWhistleMarker.SetActive(true);
                NavMeshHit navHit;
                if (NavMesh.SamplePosition(hit.point, out navHit, travelRange, NavMesh.AllAreas))
                {
                    //threeDWhistleMarker.transform.position = hit.point;
                }
                threeDWhistleMarker.transform.position = navHit.position;

                bool works = true;

                if (Vector3.Distance(player.transform.position, creature.transform.position) >= listenRange)
                {
                    works = false;
                }
                if (Vector3.Distance(hit.point, creature.transform.position) >= travelRange)
                {
                    works = false;
                }
                MeshRenderer color = threeDWhistleMarker.GetComponent<MeshRenderer>();
                if (color !=null)
                {
                    color.material.EnableKeyword("_EMISSION");
                    color.material.SetColor("_EmissionColor", works ? Color.gray : Color.red);
                }
            }
        }
        controlScheme = playerInput.currentControlScheme;
        if (controlScheme == "Gamepad")
        {
            whistleMoveSpeed = controllerWhistle;
        }
        else if (controlScheme == "Keyboard and Mouse")
        {
            whistleMoveSpeed = pcWhistle;
        }
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, transitionSpeed * Time.deltaTime);
        animator.SetLayerWeight(1, currentWeight);
    }

    public void Whistle(InputAction.CallbackContext context)
    {
        //When pressed, start whistling and show the marker at the default position
        if (whistlingEnabled && !whistling && context.performed)
        {
            whistling = true;
            whistleMarker.transform.position = markerAnchorPoint;
            targetWeight = 1.0f;
            //animator.SetLayerWeight(anilayer, 0.7f);
            //whistleMarker.SetActive(true);

        }
        //When unpressed, stop whistling and find the point to move the creature to
        if (whistling && context.canceled)
        {
            PlayNextSound();
            whistling = false;
            whistleMarker.SetActive(false);
            threeDWhistleMarker.SetActive(false);
            targetWeight = 0.0f;
            //animator.SetLayerWeight(anilayer, 0f);
            //Do a raycast from the marker position, then attract the creature if it's in listen+travel range
            if (Physics.Raycast(refCamera.ScreenPointToRay(whistleMarker.transform.position), out RaycastHit hit, Mathf.Infinity, layer) &&
                Vector3.Distance(player.transform.position, creature.transform.position) < listenRange &&
                Vector3.Distance(hit.point, creature.transform.position) < travelRange)
            {
                bool flag = false;
                RaycastHit[] interactionCheck = Physics.SphereCastAll(hit.point, interactableSnapRange, transform.forward);
                foreach(RaycastHit check in interactionCheck)
                {
                    CreatureInteractable interactable = check.collider.gameObject.GetComponent<CreatureInteractable>();
                    if (interactable != null)
                    {
                        creature.NewInteractionTarget(interactable);
                        flag = true;
                        break;
                    }
                }
                if(!flag)
                {
                    NavMeshHit navHit;
                    Vector3 destination = hit.point;
                    if (NavMesh.SamplePosition(hit.point, out navHit, travelRange, layer))
                    {
                        destination = navHit.position;
                    }
                    creature.NewTargetDestination(destination);
                    threeDWhistleMarker.transform.position = destination;
                }
            }
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    public void EnableWhistling()
    {
        whistlingEnabled = true;
    }

    public void DisableWhistling() 
    {  
        whistlingEnabled = false; 
    }

    public void PlayNextSound()
    {
        whistleSound.PlayOneShot(soundEffects[currentIndex]);
        currentIndex = (currentIndex + 1) % soundEffects.Length;
    }
}
