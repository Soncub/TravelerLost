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
    private bool whistling;

    private CreatureController creature;
    private PlayerController player;
    public AudioClip[] soundEffects;
    public AudioSource whistleSound;
    private int currentIndex = 0;

    private void Start()
    {
        //Set the default position to be where the point starts
        markerAnchorPoint = whistleMarker.transform.position;
        //Find the player and creature
        creature = FindFirstObjectByType<CreatureController>();
        player = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        //When whistling, move the pointer based on input
        if (whistling && whistlingEnabled)
        {
            whistleMarker.transform.position += Time.deltaTime * whistleMoveSpeed * (Vector3)input;
            if (Physics.Raycast(refCamera.ScreenPointToRay(whistleMarker.transform.position), out RaycastHit hit))
            {
                threeDWhistleMarker.SetActive(true);
                threeDWhistleMarker.transform.position = hit.point;

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
                    color.material.color = works ? Color.gray : Color.red;
                }
            }
        }
    }

    public void Whistle(InputAction.CallbackContext context)
    {
        //When pressed, start whistling and show the marker at the default position
        if (whistlingEnabled && !whistling && context.performed)
        {
            whistling = true;
            whistleMarker.transform.position = markerAnchorPoint;
            //whistleMarker.SetActive(true);

        }
        //When unpressed, stop whistling and find the point to move the creature to
        if (whistling && context.canceled)
        {
            PlayNextSound();
            whistling = false;
            whistleMarker.SetActive(false);
            threeDWhistleMarker.SetActive(false);
            //Do a raycast from the marker position, then attract the creature if it's in listen+travel range
            if (Physics.Raycast(refCamera.ScreenPointToRay(whistleMarker.transform.position), out RaycastHit hit) &&
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
                    if (NavMesh.SamplePosition(hit.point, out navHit, travelRange, NavMesh.AllAreas))
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
