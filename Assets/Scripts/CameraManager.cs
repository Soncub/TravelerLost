using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform target;
    public static CameraManager instance;
    private float distanceToPlayer;
    private Vector2 input;
    private bool whistling;
    public Slider horizontalSensitivitySlider;
    public Slider verticalSensitivitySlider;
    public Toggle invertHorizontalToggle;
    public Toggle invertVerticalToggle;

    private float horizontalSensitivityValue = 75;
    private float verticalSensitivityValue = 75;
    private bool invertHorizontalValue = false;
    private bool invertVerticalValue = false;

    [SerializeField] private MouseSensitivity mouseSensitivity;
    [SerializeField] private CameraAngle cameraAngle;

    private CameraRotation cameraRotation;

    private void Awake()
    {
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
    public void LoadCamera(SaveData data)
    {
        horizontalSensitivityValue = data.horizontalSensitivity;
        verticalSensitivityValue = data.verticalSensitivity;
        invertHorizontalValue = data.invertHorizontal;
        invertVerticalValue = data.invertVertical;

        mouseSensitivity.horizontal = horizontalSensitivityValue;
        horizontalSensitivitySlider.value = horizontalSensitivityValue;

        mouseSensitivity.vertical = verticalSensitivityValue;
        verticalSensitivitySlider.value = verticalSensitivityValue;

        mouseSensitivity.invertHorizontal = invertHorizontalValue;
        invertHorizontalToggle.isOn = invertHorizontalValue;

        mouseSensitivity.invertVertical = invertVerticalValue;
        invertVerticalToggle.isOn = invertVerticalValue;
    }
    public void SetHorizontalSensitivity()
    {
        horizontalSensitivityValue = horizontalSensitivitySlider.value;
        mouseSensitivity.horizontal = horizontalSensitivityValue;
    }
    public void SetVerticalSensitivity()
    {
        verticalSensitivityValue = verticalSensitivitySlider.value;
        mouseSensitivity.vertical = verticalSensitivityValue;
    }
    public void SetInvertHorizontal()
    {
        invertHorizontalValue = invertHorizontalToggle.isOn;
        mouseSensitivity.invertHorizontal = invertHorizontalValue;
    }
    public void SetInvertVertical()
    {
        invertVerticalValue = invertVerticalToggle.isOn;
        mouseSensitivity.invertVertical = invertVerticalValue;
    }
    public SaveData SaveCamera(SaveData data)
    {
        data.horizontalSensitivity = horizontalSensitivityValue;
        data.verticalSensitivity = verticalSensitivityValue;
        data.invertHorizontal = invertHorizontalValue;
        data.invertVertical = invertVerticalValue;
        return data;
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
