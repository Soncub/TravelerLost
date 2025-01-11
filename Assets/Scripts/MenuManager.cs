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

    AudioBank audioBank;

    private void Awake()
    {
        audioBank = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioBank>();
    }

    public void ActivateCanvas(string canvasName)
    {
        canvas.SetActive(true);
        controlsCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        creditsCanvas.SetActive(false);

        if (canvasName == "Controls")
        {
            audioBank.PlaySFX(audioBank.selectSound);
            controlsCanvas.SetActive(true);
            canvas.SetActive(false);
            EventSystem.current.SetSelectedGameObject(controlsButtonName);
        }
        else if (canvasName == "Settings")
        {
            audioBank.PlaySFX(audioBank.selectSound);
            settingsCanvas.SetActive(true);
            canvas.SetActive(false);
            EventSystem.current.SetSelectedGameObject(settingsButtonName);
        }
        else if (canvasName == "Credits")
        {
            audioBank.PlaySFX(audioBank.selectSound);
            creditsCanvas.SetActive(true);
            canvas.SetActive(false);
            EventSystem.current.SetSelectedGameObject(creditsButtonName);
        }
        else if (canvasName == "Main")
        {
            audioBank.PlaySFX(audioBank.selectSound);
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
        audioBank.PlaySFX(audioBank.selectSound);
        SaveAndLoad.instance.SaveGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
    }

    public void Close()
    {
        audioBank.PlaySFX(audioBank.selectSound);
        Debug.Log("Quit");
        SaveAndLoad.instance.SaveGame();
        Application.Quit();
    }
}
