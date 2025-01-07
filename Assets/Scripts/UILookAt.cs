using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAt : MonoBehaviour
{
    public Canvas canvas;

    public void Start()
    {
        canvas.worldCamera = Camera.main;
    }
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
