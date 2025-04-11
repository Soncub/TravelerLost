using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoEndSceneLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;      // Assign in Inspector or via code
    public string nextSceneName;         // The name of the scene to load
    private bool hasSkipped = false;     // Prevents multiple calls

    private void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogError("No VideoPlayer found on this GameObject.");
        }
    }

    private void Update()
    {
        if (!hasSkipped && Input.GetKeyDown(KeyCode.Space))
        {
            SkipVideo();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (!hasSkipped)
        {
            hasSkipped = true;
            LoadNextScene();
        }
    }

    private void SkipVideo()
    {
        hasSkipped = true;
        videoPlayer.Stop();
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
