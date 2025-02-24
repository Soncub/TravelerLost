using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OtherCreatureController : MonoBehaviour
{
    private NavMeshAgent agent;
    private CreatureController mainCreature;

    [Tooltip("How far does the main creature have to be for it to follow it")]
    [SerializeField] private float followDistance = 7;
    [Tooltip("This creature is trapped and shouldnt follow the main creature")]
    [SerializeField] private bool trapped = true;

    private bool currentlyFollowing = false;
    private SaveCreatureInteraction interaction;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCreature = GetComponent<CreatureController>();
    }

    private void FixedUpdate()
    {
        if (currentlyFollowing)
        {
            if (Vector3.Distance(transform.position, mainCreature.transform.position) > followDistance)
                agent.SetDestination(mainCreature.transform.position);
            else
                agent.SetDestination(transform.position);
        } else if (interaction != null)
        {
            if (Vector3.Distance(transform.position, interaction.transform.position) > interaction.interactionDistance)
                agent.SetDestination(interaction.transform.position);
            else
                agent.SetDestination(transform.position);
        }
    }

    public void Free()
    {
        trapped = false;
    }

    public void SetToFollow()
    {
        if (!trapped)
            currentlyFollowing = true;
    }

    public void SendToInteraction(SaveCreatureInteraction interaction)
    {
        this.interaction = interaction;
        currentlyFollowing = false;
    }

    public void EndInteraction()
    {
        this.interaction = null;
    }
}
