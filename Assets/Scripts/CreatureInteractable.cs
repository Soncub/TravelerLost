using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CreatureInteractable : MonoBehaviour
{
    [Tooltip("How close the creature needs to be to start the interaction")]
    public float interactionDistance;
    [Tooltip("What should happen when the creature interacts with this object")]
    [SerializeField] public UnityEvent onInteract;
}
