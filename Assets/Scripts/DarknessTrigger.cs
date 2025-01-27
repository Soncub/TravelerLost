using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DarknessTrigger : MonoBehaviour
{
    [Tooltip("Location for the creature to flee to when it finds itself in this darkness")]
    [SerializeField] Transform fleeLocation;

    private void OnTriggerEnter(Collider other)
    {
        CreatureController creature = other.GetComponentInParent<CreatureController>();
        if (creature != null)
            creature.NewFleeingTarget(fleeLocation.position);
    }
}
