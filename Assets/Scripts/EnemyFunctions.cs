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
    private AudioSource audio;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if(GetComponent<AudioSource>() != null)
            audio = GetComponent<AudioSource>();
        if (startAsleep)
            animator.SetTrigger("Sleep");
        attackCooldown = Random.Range(2, 6);
    }

    private void Update()
    {
        if (fleeing)
            transform.position += fleeSpeed * Time.deltaTime * Vector3.up;
        else if (!startAsleep)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                audio.Play();
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
