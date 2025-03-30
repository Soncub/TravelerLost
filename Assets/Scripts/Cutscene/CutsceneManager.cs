using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public PlayerController playerController;
    public RawImage videoDisplay;
    public GameObject cutsceneCanvas; // UI Canvas for the cutscene
    private bool isSkipping = false;
    private float skipHoldTime = 2f; // Time required to hold to skip
    private float holdTimer = 0f;

    void Start()
    {
        videoPlayer.loopPointReached += EndCutscene;
        cutsceneCanvas.SetActive(false); // Hide at start

        // Get the PlayerController reference
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
    }

    public void PlayCutscene()
    {
        if (playerController != null)
        {
            playerController.DisablePlayerController(); // Disable movement
        }

        cutsceneCanvas.SetActive(true);
        videoPlayer.Play();
        StartCoroutine(ShowVideo());
        StartCoroutine(CheckSkipInput()); // Start listening for skip input
    }

    IEnumerator ShowVideo()
    {
        while (!videoPlayer.isPlaying)
            yield return null;

        videoDisplay.texture = videoPlayer.texture;
    }

    IEnumerator CheckSkipInput()
    {
        while (videoPlayer.isPlaying)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= skipHoldTime)
                {
                    SkipCutscene();
                    yield break;
                }
            }
            else
            {
                holdTimer = 0f; // Reset if the key is released
            }
            yield return null;
        }
    }

    void SkipCutscene()
    {
        if (isSkipping) return; // Prevent multiple skips
        isSkipping = true;

        videoPlayer.Stop();
        EndCutscene(videoPlayer);
    }

    void EndCutscene(VideoPlayer vp)
    {
        cutsceneCanvas.SetActive(false);

        if (playerController != null)
        {
            playerController.EnablePlayerController(); // Re-enable movement
        }
    }
}
