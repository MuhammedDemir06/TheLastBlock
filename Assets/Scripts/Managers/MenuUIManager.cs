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

        string[] chapterFolders = Directory.GetDirectories(path);

        foreach (string folder in chapterFolders)
        {
            string chapterName = Path.GetFileName(folder);

            GameObject newChapter = Instantiate(chapterPrefab, chaptersLayout);

            Transform levelsParent = newChapter.transform.Find("Levels Layout");
            Transform lockParent = newChapter.transform.Find("Chapter Title");
            
            lockParent.GetComponentInChildren<TextMeshProUGUI>().text = chapterName;

            LoadLevels(chapterName,levelsParent);
        }
        ResizeChapterUI(chapterFolders.Length);
    }
    private void LoadLevels(string chapterName,Transform contentParent)
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

            if (i < PlayerDataManager.Instance.CurrentPlayerData.CurrentLevel)
            {
                newLevel.GetComponentInChildren<TextMeshProUGUI>().text = levelIndex.ToString();

                int capturedIndex = levelIndex;
                newLevel.GetComponent<Button>().onClick.AddListener(() => LevelButton(capturedIndex,chapterName));
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
    public void LevelButton(int levelIndex,string chapterName)
    {
        PlayerDataManager.Instance.CurrentPlayerData.ChapterName = chapterName;
        PlayerDataManager.Instance.CurrentPlayerData.DesiredLevel = levelIndex;
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