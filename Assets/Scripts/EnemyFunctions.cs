using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFunctions : MonoBehaviour
{
    Animator animator;

    [Tooltip("How long to play the flee animation before destroying the game object")]
    [SerializeField] private float animationTime;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void DestroySelf(float delay)
    {
        Destroy(gameObject, delay);
    }

    public void ScareOff()
    {
        animator.SetTrigger("Flee");
        Destroy(gameObject, animationTime);
    }
}
