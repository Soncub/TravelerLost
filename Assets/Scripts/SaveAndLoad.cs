using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    public static SaveAndLoad instance;

    public SaveData saveData;

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
        LoadGame();
    }

    public void SaveGame()
    {
        saveData = AudioManager.instance.SaveVolume(saveData);
        saveData = CameraManager.instance.SaveCamera(saveData);
        saveData = AudioSettings.instance.SaveSound(saveData);
        saveData = BrightnessManager.instance.SaveBrightness(saveData);
        SaveSystem.Save(saveData);
        Debug.Log("saved");
    }
    public void LoadGame()
    {
        saveData = SaveSystem.Load();
        if (saveData == null)
        {
            saveData = new SaveData();
            Debug.Log("no save found");
        }
        AudioManager.instance.LoadVolume(saveData);
        CameraManager.instance.LoadCamera(saveData);
        AudioSettings.instance.LoadSound(saveData);
        BrightnessManager.instance.LoadBrightness(saveData);
        Debug.Log("volume loaded");
    }
}
