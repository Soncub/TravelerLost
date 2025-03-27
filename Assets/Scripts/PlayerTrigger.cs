using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    [Tooltip("Event to run when player enters the trigger")]
    [SerializeField] public UnityEvent enterEvent;
    [Tooltip("Event to run when player leaves the trigger")]
    [SerializeField] public UnityEvent exitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInChildren<PlayerController>() != null)
            enterEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInChildren<PlayerController>() != null)
            exitEvent.Invoke();
    }
}
