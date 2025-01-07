using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Slider volumeSlider;

    private float currentVolume = 1;

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

    public void LoadVolume(SaveData data)
    {
        currentVolume = data.volume;
        AudioListener.volume = currentVolume;
        volumeSlider.value = currentVolume;
    }

    public void SetVolume()
    {
        currentVolume = volumeSlider.value;
        AudioListener.volume = currentVolume;
    }

    public SaveData SaveVolume(SaveData data)
    {
        data.volume = currentVolume;
        return data;
    }
}
