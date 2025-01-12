using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    public static AudioSettings instance;
    private float currentMasterVolume = 1f;
    private float currentMusicVolume = 1f;
    private float currentSFXVolume = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }

    public void SetMasterVolume()
    {
        currentMasterVolume = masterSlider.value;
        myMixer.SetFloat("Master", Mathf.Log10(currentMasterVolume) * 20);
    }

    public void SetMusicVolume()
    {
        currentMusicVolume = musicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(currentMusicVolume) * 20);
    }

    public void SetSFXVolume()
    {
        currentSFXVolume = sfxSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(currentSFXVolume) * 20);
    }
    public void LoadSound(SaveData data)
    {
        //Master
        currentMasterVolume = data.masterVolume;
        masterSlider.value = currentMasterVolume;
        //Music
        currentMusicVolume = data.musicVolume;
        musicSlider.value = currentMusicVolume;
        //SFX
        currentSFXVolume = data.sfxVolume;
        sfxSlider.value = currentSFXVolume;
    }
    public SaveData SaveSound(SaveData data)
    {
        data.masterVolume = currentMasterVolume;
        data.musicVolume = currentMusicVolume;
        data.sfxVolume = currentSFXVolume;
        return data;
    }
}
