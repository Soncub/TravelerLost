using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager instance;
    public Slider brightnessSlider;
    public PostProcessProfile brightness;
    public PostProcessLayer layer;
    private float currentBrightnessValue = 1f;
    AutoExposure exposure;

    private void Awake()
    {
        /*if(exposure.keyValue.value == null)
        {
            exposure.keyValue.value = 1;
        }*/
        //exposure.keyValue.value = currentBrightnessValue;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        brightness.TryGetSettings(out exposure);
        AdjustBrightness();
    }

    public void AdjustBrightness()
    {
        currentBrightnessValue = brightnessSlider.value;
        exposure.keyValue.value = currentBrightnessValue;

        /*if(value != 0)
        {
            exposure.keyValue.value = value;
        }
        else
        {
            exposure.keyValue.value = .05f;
        }
        currentBrightnessValue = exposure.keyValue.value;*/
    }
    //Save System
    public void LoadBrightness(SaveData data)
    {
        currentBrightnessValue = data.brightnessValue;
        brightnessSlider.value = currentBrightnessValue;
        //exposure.keyValue.value = currentBrightnessValue;
    }

    public SaveData SaveBrightness(SaveData data)
    {
        data.brightnessValue = currentBrightnessValue;
        return data;
    }
}
