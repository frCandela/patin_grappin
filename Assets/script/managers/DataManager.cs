using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tobii.Gaming;
using UnityEngine;

[Serializable] public class GameData
{
    public bool music_muted = false;
    public bool fx_muted = false;
}

public class DataManager : MonoBehaviour
{
    //properties
    public static GameData gameData;

    //Private
    private const string m_gameDataFileName = "game_settings.json";
    private static string m_pathSettings;

    //Singleton design pattern
    private static DataManager m_instance = null;

    private void Awake()
    {
        //Singleton design pattern
        if ( ! m_instance)
        {
            m_instance = this;
            m_pathSettings = Application.dataPath + "/" + m_gameDataFileName;
            LoadGameData();
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public static void SaveGameData()
    {
        string jsonString  = JsonUtility.ToJson(gameData);
        File.WriteAllText(m_pathSettings, jsonString);
    }

    public void LoadGameData()
    {
        //If settings exist loads them
        if ( File.Exists(m_pathSettings))
        {
            string jsonString = File.ReadAllText(m_pathSettings);
            gameData = JsonUtility.FromJson<GameData>(jsonString);
        }
        //Else create new settings
        else
        {
            gameData = new GameData();
            SaveGameData();
        }
    }
}
