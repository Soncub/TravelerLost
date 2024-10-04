using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    private Transform pickUpPoint;
    private Transform player;

    [Tooltip("Distance for picking up object (will change gizmo size to match)")]
    [SerializeField] private float pickUpDistance;

    [Tooltip("Amount of force added when throwing")]
    [SerializeField] private float forceMulti;

    [Tooltip("Visual for Distnace for picking up object")] 
    [SerializeField] private bool enableGizmos;

    private bool itemIsPicked;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        pickUpPoint = GameObject.Find("PickUpPoint").transform;
    }

    private void Update()
    {
        // Calculate distance dynamically
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= pickUpDistance)
        {
            if (Input.GetKeyDown(KeyCode.E) && !itemIsPicked && pickUpPoint.childCount < 1)
            {
                rb.useGravity = false;
                rb.velocity = Vector3.zero; // Stop object movement when picked up
                this.transform.position = pickUpPoint.position;
                this.transform.parent = pickUpPoint;

                itemIsPicked = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && itemIsPicked)
        {
            rb.AddForce(player.transform.forward * forceMulti, ForceMode.Impulse);
            this.transform.parent = null;
            rb.useGravity = true;
            itemIsPicked = false;

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (enableGizmos)
        {
            Gizmos.DrawWireSphere(this.transform.position, pickUpDistance);
        }
    }
}
