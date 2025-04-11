using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFunctions : MonoBehaviour
{
    Animator animator;

    [Tooltip("Should start sleeping")]
    [SerializeField] private bool startAsleep;
    [Tooltip("How long to play the flee animation before destroying the game object")]
    [SerializeField] private float animationTime;
    [Tooltip("How fast should it flee")]
    [SerializeField] private float fleeSpeed = 15;
    private bool fleeing = false;
    private float attackCooldown;
    private AudioSource attack;
    public bool forwardOtherWay;
    //Vector3 flyAway;
    float fleeDistance = 0.0f;
    float incrementRate = 15.0f;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if(GetComponent<AudioSource>() != null)
            attack = GetComponent<AudioSource>();
        if (startAsleep)
            animator.SetTrigger("Sleep");
        attackCooldown = Random.Range(2, 6);
        animator.speed = Random.Range(0.8f, 1.2f);
        //flyAway = new Vector3(fleeDistance, fleeDistance, 0.0f);
    }

    private void Update()
    {
        if (fleeing)
        {
            fleeDistance += incrementRate * Time.deltaTime;
            //transform.position += fleeSpeed * Time.deltaTime * Vector3.up;
            if (forwardOtherWay)
            {
                transform.position += -Vector3.forward * fleeDistance * Time.deltaTime;
            }
            else
            {
                transform.position += Vector3.forward * fleeDistance * Time.deltaTime;
            }
            transform.position += Vector3.up * fleeDistance * Time.deltaTime;
        }
        else if (!startAsleep)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                attack.Play();
                animator.SetTrigger("Attack");
                attackCooldown = Random.Range(2, 6);
            }
        }
    }

    public void DestroySelf(float delay)
    {
        Destroy(gameObject, delay);
    }

    public void ScareOff()
    {
        animator.SetTrigger("Flee");
        fleeing = true;
        Destroy(gameObject, animationTime);
    }
}
