﻿using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoad {

    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        Debug.Log("Saving in " + Application.persistentDataPath + "/savedGames.gd");
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");

        PlayerData data = new PlayerData();
        data.zoom = PlayerStats.instance.Zoom;

        bf.Serialize(file, data);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);

            file.Close();

            PlayerStats.instance.Zoom = data.zoom;
        }
    }
}

[System.Serializable]
class PlayerData
{
    public bool zoom;
}
