using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MenuUIManager : MonoBehaviour
{
    [Header("Menu UI Manager")]
    [Space(10)]
    [Header("Blur Effect")]
    [SerializeField] private Image blurImage;
    //UI
    [Header("Chapter && Level")]
    [SerializeField] private Transform chaptersLayout;
    [SerializeField] private GameObject chapterPrefab;  
    [SerializeField] private GameObject levelButtonPrefab;
    [Header("Snap Scroll")]
    [SerializeField] private SnapScroll snapManager;
    [Header("Next Scene Name")]
    [SerializeField] private string nextScene;

    private int chapterCount;
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        blurImage.gameObject.SetActive(true);

        LoadChapters();
    }
    private void LoadChapters()
    {
        string path = Path.Combine(Application.dataPath, "Resources/Levels");

        if (!Directory.Exists(path))
        {
            Debug.LogError("Levels Folder Not Found!");
            return;
        }

        string[] chapterFolders = Directory.GetDirectories(path)
    .OrderBy(folder =>
    {
        string folderName = Path.GetFileName(folder);
        string[] parts = folderName.Split('_');
        return parts.Length > 1 && int.TryParse(parts[0], out int num) ? num : int.MaxValue;
    }).ToArray();

        foreach (string folder in chapterFolders)
        {
            string chapterFolderName = Path.GetFileName(folder);

            string[] parts = chapterFolderName.Split('_');
            string chapterDisplayName = parts.Length > 1 ? parts[1] : chapterFolderName;

            GameObject newChapter = Instantiate(chapterPrefab, chaptersLayout);

            Transform levelsParent = newChapter.transform.Find("Levels Layout");
            Transform lockParent = newChapter.transform.Find("Chapter Title");

            lockParent.GetComponentInChildren<TextMeshProUGUI>().text = chapterDisplayName;

            LoadLevels(chapterFolderName, levelsParent,chapterCount);

            chapterCount += 1;
        }
        ResizeChapterUI(chapterFolders.Length);
    }
    private void LoadLevels(string chapterName,Transform contentParent,int chapterCount)
    {
        LevelData[] allLevels = Resources.LoadAll<LevelData>($"Levels/{chapterName}");

        var sortedLevels = allLevels.OrderBy(lv =>
        {
            Match match = Regex.Match(lv.name, @"\d+");
            return match.Success ? int.Parse(match.Value) : 0;
        }).ToArray();

        for (int i = 0; i < sortedLevels.Length; i++)
        {
            GameObject newLevel = Instantiate(levelButtonPrefab, contentParent);
            int levelIndex = i + 1;

            if (i < PlayerDataManager.Instance.CurrentPlayerData.AllChapterProgress[chapterCount].UnlockedLevelCount)
            {
                newLevel.GetComponentInChildren<TextMeshProUGUI>().text = levelIndex.ToString();

                int capturedIndex = levelIndex;
                newLevel.GetComponent<Button>().onClick.AddListener(() => LevelButton(capturedIndex, chapterName, chapterCount));
            }
            else
            {
                newLevel.GetComponentInChildren<TextMeshProUGUI>().text = "";

                Transform lockTransform = newLevel.transform.Find("Lock Icon");

                lockTransform.gameObject.SetActive(true);
            }
        }
    }
    private void ResizeChapterUI(int chapterCount)
    {
        RectTransform rt = chaptersLayout.GetComponent<RectTransform>();

        float buttonWidth = 1000;
        float totalWidth = chapterCount * buttonWidth;

        rt.sizeDelta = new Vector2(totalWidth, rt.sizeDelta.y);

        snapManager.Snap(); //Update Snap
    }
    public void LevelButton(int levelIndex,string chapterName,int desiredChapter)
    {
        PlayerDataManager.Instance.CurrentPlayerData.CurrentChapter = chapterName;
        PlayerDataManager.Instance.CurrentPlayerData.DesiredLevel = levelIndex;

        PlayerDataManager.Instance.CurrentPlayerData.DesiredChapter = desiredChapter;

        PlayerDataManager.Instance.SaveData();

        SettingsUI.Instance.PlaySFX(SettingsUI.Instance.GameSoundData.ClickSound);
        SceneTransitionManager.Instance.LoadScene(nextScene);
    }
    //Quit
    public void QuitButton()
    {
        SceneTransitionManager.Instance.GameQuit();
    }
}