using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class CreatureController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Animator animator;
    private Transform movingTarget;
    public CreatureInteractable interactable;

    [Tooltip("Flee position distance")]
    [SerializeField] private float fleeRange = 3;

    [Tooltip("How long it takes for the creature to lose focus on an object/whistle")]
    [SerializeField] private float targetFocusTime = 5;
    private float focusTimeLeft = 0;
    private bool afraid = false;
    private float idleTime;
    public AudioSource growl;
    public AudioSource whimper;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        animator.SetFloat("speed", agent.velocity.magnitude);
        if (!afraid)
        {
            //If currently focused, update focus time. Ignore this if its afraid, as when its afraid focus does not matter
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
                        interactable.Interact();
                        interactable = null;
                        LoseFocus();

                    }
                    else
                        agent.SetDestination(movingTarget.position);
                }
            }
            //If not focused on something, keep track of how long it hasn't moved for and randomly play idle animations.
            else
            {
                idleTime += Time.deltaTime;
                if (idleTime >= 10)
                {   
                    animator.SetTrigger("idleRandom");
                    idleTime = 0;
                }
            }
        }
        //If currently afraid, calm down if close enough to the flee position
        if (afraid && agent.remainingDistance <= fleeRange)
        {
            LoseFocus();
            animator.SetBool("fleeing", false);
        }
    }
 
    public void NewTargetDestination(Vector3 position)
    {
        //When the creature is afraid, it cannot get a new destination
        if (afraid)
            return;
        //Set the destination and erase the current moving target if there is one
        if (movingTarget != null)
            movingTarget = null;
        growl.Play();
        focusTimeLeft = targetFocusTime;
        agent.SetDestination(position);
    }

    public void NewMovingTarget(Transform transform)
    {
        //When the creature is afraid, it cannot get a new destination
        if (afraid)
            return;
        //Set a new moving target that will update every frame
        growl.Play();
        movingTarget = transform;
        focusTimeLeft = targetFocusTime;
        agent.SetDestination(movingTarget.position);
    }

    public void NewInteractionTarget(CreatureInteractable interactable)
    {
        //When the creature is afraid, it cannot get a new destination
        if (afraid)
            return;
        this.interactable = interactable;
        NewMovingTarget(interactable.transform);
    }

    public void LoseFocus()
    {
        afraid = false;
        movingTarget = null;
        interactable = null;
        agent.SetDestination(transform.position);
    }

    public void NewFleeingTarget(Vector3 position)
    {
        //Set afraid to true, so that it will ignore all other commands until it has fled from the darkness
        afraid = true;
        //Set the destination and erase the current moving target if there is one
        if (movingTarget != null)
            movingTarget = null;
        agent.SetDestination(position);
        whimper.Play();
        animator.SetBool("fleeing", true);
    }

    public void AnimatorTrigger(string key)
    {
        animator.SetTrigger(key);
    }
}
