using UnityEngine;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public LevelLoader levelLoader;
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
            EndCutsceneAndLoadLevel();
        }
    }

    private void SkipVideo()
    {
        hasSkipped = true;
        videoPlayer.Stop();
        EndCutsceneAndLoadLevel();
    }

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
}
