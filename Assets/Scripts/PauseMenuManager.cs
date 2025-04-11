using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuManager : BookAnimation
{
    [SerializeField] private GameObject pauseMenuCanavs;
    [SerializeField] private GameObject settingsMenuCanavs;
    [SerializeField] private GameObject controlsMenuCanavs;

    [SerializeField] private PlayerController player;
    [SerializeField] private WhistleSystem whistle;

    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject backSettingsButton;
    [SerializeField] private GameObject backControlsButton;
    AudioBank audioBank;
    public bool isPaused;
    [SerializeField] LevelEnd levelEnd;
    public LevelLoader levelLoader;
    public AudioSource openBook;

    private void Awake()
    {
        audioBank = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioBank>();
    }
    private void Start()
    {
        pauseMenuCanavs.SetActive(false);
        settingsMenuCanavs.SetActive(false);
        controlsMenuCanavs.SetActive(false);
    }

    private void Update()
    {
        if(levelEnd.fadeTimer == 0)
        {
            if (InputManager.instance.menuOpenCloseInput && !IsAnimating())
            {
                if (!isPaused)
                {
                    Pause();
                }
                else
                {
                    Unpause();
                }
            }
        }
    }

    public void Pause()
    {
        OpenBook();
        openBook.Play();
        
        StartCoroutine(PauseLogic(animationLength));
    }

    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1f;

        player.enabled = true;
        whistle.enabled = true;

        CloseAllMenus();

        CloseBook();
    }

    private void OpenMainMenu()
    {
        audioBank.PlaySFX(audioBank.selectSound);
        pauseMenuCanavs.SetActive(true);
        settingsMenuCanavs.SetActive(false);
        controlsMenuCanavs.SetActive(false);

        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    private void OpenSettings()
    {
        audioBank.PlaySFX(audioBank.selectSound);
        settingsMenuCanavs.SetActive(true);
        pauseMenuCanavs.SetActive(false);
        controlsMenuCanavs.SetActive(false);

        EventSystem.current.SetSelectedGameObject(backSettingsButton);
    }

    private void OpenControls()
    {
        audioBank.PlaySFX(audioBank.selectSound);
        controlsMenuCanavs.SetActive(true);
        settingsMenuCanavs.SetActive(false);
        pauseMenuCanavs.SetActive(false);

        EventSystem.current.SetSelectedGameObject(backControlsButton);
    }

    private void CloseAllMenus()
    {
        audioBank.PlaySFX(audioBank.selectSound);
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
        CloseAllMenus();
        /*audioBank.PlaySFX(audioBank.selectSound);
        Time.timeScale = 1f;
        SaveAndLoad.instance.SaveGame();*/
        levelLoader.LoadLevel(0);
        //UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
    public void QuitGame()
    {
        audioBank.PlaySFX(audioBank.selectSound);
        Time.timeScale = 1f;
        Debug.Log("Quit");
        Application.Quit();
    }

    #region IEnumerators
    IEnumerator PauseLogic(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        OpenMainMenu();

        isPaused = true;
        Time.timeScale = 0f;

        player.enabled = false;
        whistle.enabled = false;

        CloseAnimationState(true);
    }
    #endregion
}
