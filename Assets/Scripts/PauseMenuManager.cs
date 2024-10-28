using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanavs;
    [SerializeField] private GameObject settingsMenuCanavs;
    [SerializeField] private GameObject controlsMenuCanavs;

    [SerializeField] private PlayerController player;
    [SerializeField] private WhistleSystem whistle;

    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject backSettingsButton;
    [SerializeField] private GameObject backControlsButton;

    private bool isPaused;
    private void Start()
    {
        pauseMenuCanavs.SetActive(false);
        settingsMenuCanavs.SetActive(false);
        controlsMenuCanavs.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.instance.menuOpenCloseInput)
        {
            if(!isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        player.enabled = false;
        whistle.enabled = false;

        OpenMainMenu();
    }

    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1f;

        player.enabled = true;
        whistle.enabled = true;

        CloseAllMenus();
    }

    private void OpenMainMenu()
    {
        pauseMenuCanavs.SetActive(true);
        settingsMenuCanavs.SetActive(false);
        controlsMenuCanavs.SetActive(false);

        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    private void OpenSettings()
    {
        settingsMenuCanavs.SetActive(true);
        pauseMenuCanavs.SetActive(false);
        controlsMenuCanavs.SetActive(false);

        EventSystem.current.SetSelectedGameObject(backSettingsButton);
    }

    private void OpenControls()
    {
        controlsMenuCanavs.SetActive(true);
        settingsMenuCanavs.SetActive(false);
        pauseMenuCanavs.SetActive(false);

        EventSystem.current.SetSelectedGameObject(backControlsButton);
    }

    private void CloseAllMenus()
    {
        pauseMenuCanavs.SetActive(false);
        settingsMenuCanavs.SetActive(false);
        controlsMenuCanavs.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void PressSettings()
    {
        OpenSettings();
    }

    public void PressControls()
    {
        OpenControls();
    }

    public void PressResume()
    {
        Unpause();
    }

    public void PressBack()
    {
        OpenMainMenu();
    }
    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
