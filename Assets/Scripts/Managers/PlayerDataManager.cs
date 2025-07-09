using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class NewChapter
{
    public string ChapterName;
    public int LevelInChapter;
}
[System.Serializable]
public class PlayerData
{
    [Header("Game")]
    public int CurrentLevel;
    public int DesiredLevel;
    public string ChapterName;
    [Header("Chapter")]
    public List<NewChapter> Chapter;
    [Header("Settings")]
    public float GameSound;
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public PlayerData CurrentPlayerData;
    private string filePath;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        CurrentPlayerData = LoadData();
    }
    public void SaveData()
    {
        string json = JsonUtility.ToJson(CurrentPlayerData, true);
        File.WriteAllText(filePath, json);
    }
    private PlayerData LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            return data;
        }
        else
        {
            CurrentPlayerData = NewPlayerData();
            return CurrentPlayerData;
        }
    }
    private PlayerData NewPlayerData()
    {
        PlayerData newPlayerData = new PlayerData
        {
            CurrentLevel = 1,
            DesiredLevel = 0,
            GameSound = 40,
            ChapterName = "First"
        };

        string json = JsonUtility.ToJson(newPlayerData, true);
        File.WriteAllText(filePath, json);

        return newPlayerData;
    }
    public static void DeleteData()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "playerData.json")))
        {
            File.Delete(Path.Combine(Application.persistentDataPath, "playerData.json"));
            Debug.Log("Data Deleted.");
        }
    }
}