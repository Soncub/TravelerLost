using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;
    public GameObject cutsceneCanvas; // UI Canvas for the cutscene

    void Start()
    {
        videoPlayer.loopPointReached += EndCutscene;
        cutsceneCanvas.SetActive(false); // Hide at start
    }

    public void PlayCutscene()
    {
        cutsceneCanvas.SetActive(true);
        videoPlayer.Play();
        StartCoroutine(ShowVideo());
    }

    IEnumerator ShowVideo()
    {
        while (!videoPlayer.isPlaying)
            yield return null;

        videoDisplay.texture = videoPlayer.texture;
    }

    void EndCutscene(VideoPlayer vp)
    {
        cutsceneCanvas.SetActive(false);
    }
}
