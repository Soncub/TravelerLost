using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAnimation : StateMachineBehaviour
{
    [SerializeField] private float timeUntilIdle;
    [SerializeField] private int numberOfAnimations;
    private bool isIdle;
    private float idleTime;
    private int idleAnimation;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isIdle == false)
        {
            idleTime += Time.deltaTime;

            if (idleTime > timeUntilIdle && stateInfo.normalizedTime % 1 < 0.02f)
            { 
                isIdle = true;
                idleAnimation = Random.Range(1, numberOfAnimations + 1);
                idleAnimation = idleAnimation * 2 - 1;

                animator.SetFloat("Blend", idleAnimation - 1);

            }

        }
        else if (stateInfo.normalizedTime % 1 > 0.98)
        {
            ResetIdle();
        }
        animator.SetFloat("Blend", idleAnimation, 0.2f, Time.deltaTime);
    }
    private void ResetIdle()
    {
        if (isIdle)
        {
            idleAnimation--;
        }
        isIdle = false;
        idleTime = 0;
    }
}
