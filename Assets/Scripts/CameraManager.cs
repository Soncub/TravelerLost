using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform target;
    private float distanceToPlayer;
    private Vector2 input;
    private bool whistling;

    [SerializeField] private MouseSensitivity mouseSensitivity;
    [SerializeField] private CameraAngle cameraAngle;

    private CameraRotation cameraRotation;

    private void Awake()
    {
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
    }

    public void Look(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    public void Whistle(InputAction.CallbackContext context)
    {
        if (!whistling && context.performed)
            whistling = true;
        if (whistling && context.canceled)
            whistling = false;
    }

    public void Update()
    {
        if (!whistling) 
        {
            cameraRotation.Yaw += input.x * mouseSensitivity.horizontal * BoolToInt(mouseSensitivity.invertHorizontal) * Time.deltaTime;
            cameraRotation.Pitch += input.y * mouseSensitivity.vertical * BoolToInt(mouseSensitivity.invertVertical) * Time.deltaTime;
            cameraRotation.Pitch = Mathf.Clamp(cameraRotation.Pitch, cameraAngle.min, cameraAngle.max);
        }
    }

    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(cameraRotation.Pitch, cameraRotation.Yaw, 0.0f);
        transform.position = target.position - transform.forward * distanceToPlayer;
    }

    private static int BoolToInt(bool b) => b ? 1 : -1;

    public void EnableCameraManager()
    {
        this.enabled = true;
    }

    public void DisableCameraManager()
    {
        this.enabled = false;
    }
}

[System.Serializable]
public struct MouseSensitivity
{
    public float horizontal;
    public float vertical;
    public bool invertVertical;
    public bool invertHorizontal;
}
public struct CameraRotation
{
    public float Pitch;
    public float Yaw;
}

[System.Serializable]
public struct CameraAngle
{
    public float min;
    public float max;
}
