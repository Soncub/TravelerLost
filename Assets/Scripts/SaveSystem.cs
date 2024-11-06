using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private static string filePath = Application.persistentDataPath + "/save.tl";

    public static void Save(SaveData data)
    {
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            new BinaryFormatter().Serialize(stream, data);
        }
    }

    public static SaveData Load()
    {
        if (File.Exists(filePath))
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return new BinaryFormatter().Deserialize(stream) as SaveData;
            }
        }
        else
        {
            return new SaveData();
        }
    }
}
