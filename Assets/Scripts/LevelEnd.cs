using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEnd : MonoBehaviour
{
    [Tooltip("How long both the player and creature should be here before triggering the events")]
    [SerializeField] float waitTime = 5;
    [Tooltip("Events to invoke when the player and creature have been in here for the wait time (pop ups, changing scenes, cutscenes, etc)")]
    [SerializeField] UnityEvent finishEvent;
    float waitTimer = 0;
    bool creature, player;

    private void Update()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0 && creature && player)
            {
                finishEvent.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CreatureController>() != null) 
        { 
            creature = true;
        }
        else if (other.gameObject.GetComponent<PlayerController>() != null) 
        {
            player = true;
        }
        if (player && creature)
            waitTimer = waitTime;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CreatureController>() != null)
        {
            creature = false;
            waitTimer = 0;
        }
        else if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            player = false;
            waitTimer = 0;
        }
    }
}
