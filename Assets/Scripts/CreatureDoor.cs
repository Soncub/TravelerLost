using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureDoor : MonoBehaviour
{
    public GameObject creatureDoor;
    public float moveSpeed = 3f;
    public float maxHeight = 6f;
    public NavMeshAgent creatureAgent;
    private Vector3 movePosition;
    private Vector3 initialPosition;
    private bool isOpening = false;
    private bool doorFullyOpened = false;

    void Start()
    {
        initialPosition = creatureDoor.transform.position;
        movePosition = new Vector3(0.0f, maxHeight, 0.0f);
    }

    void Update()
    {
        if (isOpening && !doorFullyOpened)
        {
            float step = moveSpeed * Time.deltaTime;
            creatureDoor.transform.position = Vector3.MoveTowards(creatureDoor.transform.position, initialPosition + movePosition, step);

            if (creatureDoor.transform.position == initialPosition + movePosition)
            {
                doorFullyOpened = true;
                isOpening = false;
                creatureAgent.isStopped = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Creature") && !isOpening && !doorFullyOpened)
        {
            Debug.Log("Open door.");
            isOpening = true;
            creatureAgent.isStopped = true;
        }
    }
}
