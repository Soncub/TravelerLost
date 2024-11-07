using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CreatureController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform movingTarget;
    private CreatureInteractable interactable;

    [Tooltip("How long it takes for the creature to lose focus on an object/whistle")]
    [SerializeField] private float targetFocusTime = 5;
    private float focusTimeLeft = 0;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        //If currently focused, update focus time and
        if (focusTimeLeft > 0)
        {
            focusTimeLeft -= Time.deltaTime;
            //If focus is lost, stop moving towards a target destination
            if (focusTimeLeft < 0)
                LoseFocus();
            //If focus is not lost and using a moving target, update its destination
            else if (movingTarget != null)
            {
                if (interactable != null && Vector3.Distance(transform.position, movingTarget.position) <= interactable.interactionDistance)
                {
                    interactable.onInteract.Invoke();
                    interactable = null;
                    LoseFocus();
                }
                else
                    agent.SetDestination(movingTarget.position);
            }
        }
    }

    public void NewTargetDestination(Vector3 position)
    {
        //Set the destination and erase the current moving target if there is one
        if (movingTarget != null)
            movingTarget = null;
        focusTimeLeft = targetFocusTime;
        agent.SetDestination(position);
    }

    public void NewMovingTarget(Transform transform)
    {
        //Set a new moving target that will update every frame
        movingTarget = transform;
        focusTimeLeft = targetFocusTime;
        agent.SetDestination(movingTarget.position);
    }

    public void NewInteractionTarget(CreatureInteractable interactable)
    {
        this.interactable = interactable;
        NewMovingTarget(interactable.transform);
    }

    public void LoseFocus()
    {
        movingTarget = null;
        agent.SetDestination(transform.position);
    }
}
