using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBank : MonoBehaviour
{
    [Header("---Audio Source---")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource bgMusicSource;


    [Header("---Audio Clips---")]
    public AudioClip backgroundMusic;
    public AudioClip secondBackgroundMusic;
    public AudioClip selectSound;

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        bgMusicSource.clip = secondBackgroundMusic;
        musicSource.Play();
        bgMusicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
