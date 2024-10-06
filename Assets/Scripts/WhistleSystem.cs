using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class WhistleSystem : MonoBehaviour
{
    [Tooltip("UI object to move around for pointing where the whistle should fire")]
    [SerializeField] private GameObject whistleMarker;
    [Tooltip("How fast the whistle point should move")]
    [SerializeField] private float whistleMoveSpeed = 1;
    [Tooltip("Camera used as a reference for whistling (should be the same one used when whistling)")]
    [SerializeField] private Camera refCamera;
    [Tooltip("The furthest you can whistle for the creature to 'follow' it")]
    [SerializeField] private float travelRange = 10;
    [Tooltip("The furthest the creature can be from the player to 'hear' it")]
    [SerializeField] private float listenRange = 10;

    private Vector3 markerAnchorPoint;
    private Vector2 input;
    private bool whistling;

    private CreatureController creature;
    private PlayerController player;

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
        if (whistling)
            whistleMarker.transform.position += (Vector3)input * Time.deltaTime;
    }

    public void Whistle(InputAction.CallbackContext context)
    {
        //When starting to be pressed, start whistling and show the marker at the default position
        if (context.started)
        {
            whistling = true;
            whistleMarker.transform.position = markerAnchorPoint;
            whistleMarker.SetActive(true);
        }
        //When unpressed, stop whistling and find the point to move the creature to
        if (context.performed)
        {
            whistling = false;
            whistleMarker.SetActive(false);
            //Do a raycast from the marker position, then attract the creature if it's in listen+travel range
            RaycastHit hit;
            if (Physics.Raycast(refCamera.ScreenPointToRay(whistleMarker.transform.position), out hit) &&
                Vector3.Distance(player.transform.position, creature.transform.position) < listenRange &&
                Vector3.Distance(hit.point, creature.transform.position) < travelRange)
            {
                creature.NewTargetDestination(hit.point);
            }
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }
}
