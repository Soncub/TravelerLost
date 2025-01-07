using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float volume = 1f;
    public float horizontalSensitivity = 75f;
    public float verticalSensitivity = 75f;
    public bool invertHorizontal = false;
    public bool invertVertical = false;

    public SaveData()
    {
        
    }
}
