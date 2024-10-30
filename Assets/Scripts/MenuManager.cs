using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject controlsCanvas;
    public GameObject settingsCanvas;
    public GameObject creditsCanvas;

    public GameObject menuButtonName;
    public GameObject controlsButtonName;
    public GameObject settingsButtonName;
    public GameObject creditsButtonName;

    public void ActivateCanvas(string canvasName)
    {
        canvas.SetActive(true);
        controlsCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        creditsCanvas.SetActive(false);

        if (canvasName == "Controls")
        {
            controlsCanvas.SetActive(true);
            canvas.SetActive(false);
            EventSystem.current.SetSelectedGameObject(controlsButtonName);
        }
        else if (canvasName == "Settings")
        {
            settingsCanvas.SetActive(true);
            canvas.SetActive(false);
            EventSystem.current.SetSelectedGameObject(settingsButtonName);
        }
        else if (canvasName == "Credits")
        {
            creditsCanvas.SetActive(true);
            canvas.SetActive(false);
            EventSystem.current.SetSelectedGameObject(creditsButtonName);
        }
        else if (canvasName == "Main")
        {
            canvas.SetActive(true);
            controlsCanvas.SetActive(false);
            settingsCanvas.SetActive(false);
            creditsCanvas.SetActive(false);
            EventSystem.current.SetSelectedGameObject(menuButtonName);
        }
    }

    public void ControlsPopUp()
    {
        ActivateCanvas("Controls");
    }

    public void SettingsPopUp()
    {
        ActivateCanvas("Settings");
    }

    public void CreditsPopUp()
    {
        ActivateCanvas("Credits");
    }

    public void MenuPopUp()
    {
        ActivateCanvas("Main");
    }

    public void LoadLevel1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
    }

    public void Close()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
