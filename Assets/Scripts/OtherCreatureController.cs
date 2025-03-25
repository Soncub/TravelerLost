using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OtherCreatureController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Animator animator;
    private CreatureController mainCreature;

    [Tooltip("How far does the main creature have to be for it to follow it")]
    [SerializeField] private float followDistance = 16;
    [Tooltip("How far does the main creature have to be for it to end its current interaction and continue following")]
    [SerializeField] private float returnDistance = 32;
    [Tooltip("This creature is trapped and shouldnt follow the main creature")]
    [SerializeField] private bool trapped = true;

    private bool currentlyFollowing = false;
    private SaveCreatureInteraction interaction;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        mainCreature = FindFirstObjectByType<CreatureController>();
    }

    private void FixedUpdate()
    {
        if (!trapped)
        {
            animator.SetFloat("speed", agent.velocity.magnitude);
            if (interaction != null)
            {
                if (Vector3.Distance(transform.position, mainCreature.transform.position) > returnDistance)
                    interaction = null;
                else if (Vector3.Distance(transform.position, interaction.transform.position) > interaction.interactionDistance)
                    agent.SetDestination(interaction.transform.position);
                else
                    agent.SetDestination(transform.position);
            } else if (currentlyFollowing)
            {
                if (Vector3.Distance(transform.position, mainCreature.transform.position) > followDistance)
                    agent.SetDestination(mainCreature.transform.position);
                else
                    agent.SetDestination(transform.position);
            }
        }
    }

    public void Free()
    {
        trapped = false;
        SetToFollow();
    }

    public void SetToFollow()
    {
        if (!trapped)
            currentlyFollowing = true;
    }

    public void SendToInteraction(SaveCreatureInteraction interaction)
    {
        this.interaction = interaction;
    }

    public void EndInteraction()
    {
        this.interaction = null;
    }

    public void AnimatorTrigger(string key)
    {
        animator.SetTrigger(key);
    }
}
