using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

[System.Serializable]
public class ChapterProgress
{
    public string ChapterName;
    public int UnlockedLevelCount;
    public int MaxLevelCount;
}

[System.Serializable]
public class PlayerData
{
    [Header("Game Progress")]
    public string CurrentChapter;
    public int DesiredLevel;
    public int DesiredChapter;
    public int ActiveChapterCount;

    public List<ChapterProgress> AllChapterProgress = new();

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
            DesiredChapter = 0,
            ActiveChapterCount = 0,
            DesiredLevel = 0,
            GameSound = 40,
            AllChapterProgress = new List<ChapterProgress>()
        };

        string levelsPath = Path.Combine(Application.dataPath, "Resources/Levels");

        if (Directory.Exists(levelsPath))
        {
            string[] chapterFolders = Directory.GetDirectories(levelsPath);

            var sortedChapters = chapterFolders.OrderBy(folder =>
            {
                string folderName = Path.GetFileName(folder);
                Match match = Regex.Match(folderName, @"\d+");
                return match.Success ? int.Parse(match.Value) : int.MaxValue;
            });

            foreach (string folder in sortedChapters)
            {
                string chapterName = Path.GetFileName(folder);

                // Chapter içindeki Level sayısını bul
                string[] levelFiles = Directory.GetFiles(folder, "*.asset");
                int levelCount = levelFiles.Length;

                int unlockedLevel = newPlayerData.AllChapterProgress.Count == 0 ? 1 : 0;

                newPlayerData.AllChapterProgress.Add(new ChapterProgress
                {
                    ChapterName = chapterName,
                    UnlockedLevelCount = unlockedLevel,
                    MaxLevelCount = levelCount
                });
            }

            // Başlangıç chapter’ını ilk sıraya al
            if (newPlayerData.AllChapterProgress.Count > 0)
                newPlayerData.CurrentChapter = newPlayerData.AllChapterProgress[0].ChapterName;
        }

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