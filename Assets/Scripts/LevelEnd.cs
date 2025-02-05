using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LevelEnd : MonoBehaviour
{
    [Tooltip("How long both the player and creature should be here before triggering the events")]
    [SerializeField] float waitTime = 5;
    [Tooltip("Events to invoke when the player and creature have been in here for the wait time (pop ups, changing scenes, cutscenes, etc)")]
    [SerializeField] UnityEvent finishEvent;
    [Tooltip("An image covering the screen. It will fade to black while the timer is counting down.")]
    public Image fade;
    public GameObject fadeImage;
    [SerializeField] float waitTimer = 0;
    [SerializeField] bool creature, player;
    public float fadeTimer = 0;
    public Collider zone;

    private void Update()
    {
        /*if (!creature || !player)
        {
            fadeImage.SetActive(false);
            fadeTimer = 0;
        }*/
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            fadeTimer += Time.deltaTime;
            fadeImage.SetActive(true);
            fade.color = new(0, 0, 0, fadeTimer / waitTime);
            if (waitTimer <= 0 && creature && player)
            {
                finishEvent.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Creature")) 
        { 
            creature = true;
        }
        if (other.gameObject.GetComponent<PlayerController>() != null) 
        {
            player = true;
        }
        if (player && creature)
            waitTimer = waitTime;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Creature"))
        {
            creature = false;
            waitTimer = 0;
            fade.color = new(0, 0, 0, 0);
            fadeTimer = 0;
            fadeImage.SetActive(false);

        }
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            player = false;
            waitTimer = 0;
            fade.color = new(0, 0, 0, 0);
            fadeTimer = 0;
            fadeImage.SetActive(false);
        }
    }

    public void ActivateLevelEnd()
    {
        zone.enabled = true;
    }
}
