using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAttractor : MonoBehaviour
{
    private CreatureController creature;

    [Tooltip("How close the creature has to be for the object to attract it")]
    [SerializeField] private float attractDistance;
    [Tooltip("Have the creature follow the object as it moves, rather than the position it was in when attracted")]
    [SerializeField] private bool movingObject;

    [Tooltip("Should the object automatically attract the creature on a timed interval")]
    [SerializeField] private bool automaticallyAttract = false;
    [Tooltip("How often the object should automatically attract the creature")]
    [SerializeField] private float automaticAttractionTime = 0;
    private float automaticAttractionTimer = 0;

    [Tooltip("Self destruct after a successful attraction (do not use with moving or interactable objects)")]
    [SerializeField] private bool destroyOnAttract = false;

    private void Start()
    {
        creature = FindFirstObjectByType<CreatureController>();
    }

    private void Update()
    {
        //If set to automatically attract, do so on the timed interval
        if (automaticallyAttract)
        {
            automaticAttractionTimer -= Time.deltaTime;
            if (automaticAttractionTimer <= 0)
            {
                //When it does, reset the timer and attract the creature if it is in range.
                automaticAttractionTimer += automaticAttractionTime;
                if (Vector3.Distance(transform.position, creature.transform.position) > attractDistance)
                    AttractCreature();
            }
        }
    }

    public void AttractCreature()
    {
        //Use the proper creature attraction method
        CreatureInteractable interaction = gameObject.GetComponent<CreatureInteractable>();
        if (interaction != null)
            creature.NewInteractionTarget(interaction);
        else if (movingObject)
            creature.NewMovingTarget(transform);
        else
        {
            creature.NewTargetDestination(transform.position);
            //If self destructive, destroy the attached game object.
            if (destroyOnAttract)
                Destroy(gameObject);
        }
    }
}
