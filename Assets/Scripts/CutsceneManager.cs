using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject loadingScreen;
    public GameObject background;
    public Slider slider;
    public Text progressText;
    public int nextSceneIndex;
    private bool hasSkipped = false;

    private void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoEnd;
        else
            Debug.LogError("No VideoPlayer found on this GameObject.");
    }

    private void Update()
    {
        if (!hasSkipped && Input.GetKeyDown(KeyCode.Space) || Gamepad.current.startButton.wasPressedThisFrame)
        {
            SkipVideo();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (!hasSkipped)
        {
            hasSkipped = true;
            //EndCutsceneAndLoadLevel();
            background.SetActive(true);
            StartCoroutine(LoadAsynchronously(nextSceneIndex));
        }
    }

    private void SkipVideo()
    {
        hasSkipped = true;
        videoPlayer.Stop();
        //EndCutsceneAndLoadLevel();
        background.SetActive(true);
        StartCoroutine(LoadAsynchronously(nextSceneIndex));
    }

    /*
    private void EndCutsceneAndLoadLevel()
    {
        if (levelLoader != null)
        {
            levelLoader.LoadLevel(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("LevelLoader reference not set in CutsceneManager.");
        }
    }
    */

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            //Debug.Log(progress);
            slider.value = progress;
            progressText.text = progress * 100f + "%";

            yield return null;
        }
    }
}
