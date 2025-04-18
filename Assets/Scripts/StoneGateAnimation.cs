using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneGateAnimation : MonoBehaviour
{
    [SerializeField] Animator gateAnimator;
    [SerializeField] ParticleSystem dust;
    [SerializeField] float animationLength;
    [SerializeField] AudioSource sound;

    void Start()
    {
        dust.Stop();
    }

    public void OpenGate()
    {
        dust.Play();
        sound.Play();
        sound.loop = true;
        StartCoroutine(PlayAndWaitForAnim());
    }

    public IEnumerator PlayAndWaitForAnim()
    {
        gateAnimator.SetTrigger("Move");

        yield return new WaitForSeconds(animationLength);

        dust.Stop();
        sound.Stop();
        sound.loop = false;
    }
}
