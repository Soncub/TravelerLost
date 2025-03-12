using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveCreatureInteraction : CreatureInteractable
{
    public List<OtherCreatureController> neededOtherCreatures;
    private bool waiting = false;
    private CreatureController creature;

    private void Start()
    {
        creature = FindFirstObjectByType<CreatureController>();
    }

    public override void Interact()
    {
        waiting = true;
        foreach (var creature in neededOtherCreatures)
            creature.SendToInteraction(this);
    }

    private void FixedUpdate()
    {
        if (waiting)
        {
            if (Vector3.Distance(transform.position, creature.transform.position) > interactionDistance)
                return;
            foreach (var creature in neededOtherCreatures) {
                if (Vector3.Distance(transform.position, creature.transform.position) > interactionDistance)
                    return;
            }
            foreach (var creature in neededOtherCreatures)
            {
                creature.EndInteraction();
                creature.SetToFollow();
            }
            waiting = false;
            onInteract.Invoke();
        }
    }
}
