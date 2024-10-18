using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptToggleController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CameraManager cameraManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (playerController.enabled)
                playerController.DisablePlayerController();
            else
                playerController.EnablePlayerController();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cameraManager.enabled)
                cameraManager.DisableCameraManager();
            else
                cameraManager.EnableCameraManager();
        }
    }
}
