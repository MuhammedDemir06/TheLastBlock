using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance;

    [Header("Player UI Manager")]
    public bool GamePaused;
    public bool GameFinished;

    [Space(10)]
    [Header("Player Health")]
    [SerializeField] private Image[] playerHearts;
    [SerializeField] private Sprite activeHeartSprite;
    [SerializeField] private Sprite unactiveHeartSprite;
    [Header("Pause Screen")]
    [SerializeField] private AnimatedPanel pauseScreen;
    [Header("Game Over Screen")]
    [SerializeField] private AnimatedPanel gameOverScreen;
    [Header("Next Level Screen")]
    [SerializeField] private AnimatedPanel nextLevelScreen;
    private void OnEnable()
    {
        GameInputManager.PlayerUIPause += PauseButton;
        PlayerHealth.DamageControl += DamageControl;
    }
    private void OnDisable()
    {
        GameInputManager.PlayerUIPause -= PauseButton;
        PlayerHealth.DamageControl -= DamageControl;
    }

    private void Awake()
    {
        Instance = this;
    }
    private void DamageControl(int playerHealthAmount, int value, bool isDamage)
    {
        for (int i = 0; i < playerHearts.Length; i++)
        {
            bool heartIsActive = playerHearts[i].sprite == activeHeartSprite;

            if (isDamage && heartIsActive && value > 0)
            {
                playerHearts[i].sprite = unactiveHeartSprite;
                value--;
            }
            else if (!isDamage && !heartIsActive && value > 0)
            {
                playerHearts[i].sprite = activeHeartSprite;
                value--;
            }

            if (value <= 0)
                break;
        }

        if (playerHealthAmount <= 0)
        {
            gameOverScreen.Show();
            SettingsUI.Instance.PlaySFX(SettingsUI.Instance.GameSoundData.LoseSound);
            Debug.Log("Player Died!");
        }
    }

    private bool newChapterActive;
    public void NextLevel()
    {
        GameFinished = true;
        GamePaused = true;
        nextLevelScreen.Show();

        var data = PlayerDataManager.Instance.CurrentPlayerData;

        if (data.DesiredLevel == data.AllChapterProgress[data.ActiveChapterCount].UnlockedLevelCount)
        {
            data.AllChapterProgress[data.ActiveChapterCount].UnlockedLevelCount += 1;

            Debug.Log("New Level Activated");
        }

        if (data.AllChapterProgress[data.ActiveChapterCount].UnlockedLevelCount - 1 == data.AllChapterProgress[data.ActiveChapterCount].MaxLevelCount && data.AllChapterProgress[data.DesiredChapter + 1].UnlockedLevelCount == 0)
        {
            data.ActiveChapterCount += 1;
            data.AllChapterProgress[data.ActiveChapterCount].UnlockedLevelCount += 1;

            newChapterActive = true;

            Debug.Log("New Chapter Activated");
        }

        PlayerDataManager.Instance.CurrentPlayerData = data;

        PlayerDataManager.Instance.SaveData();

        SettingsUI.Instance.PlaySFX(SettingsUI.Instance.GameSoundData.NextLevelSound);
    }
    private bool nextLevelButtonClick = false;
    //Buttons
    public void NextLevelButton()
    {
        if(!nextLevelButtonClick)
        {
            var data = PlayerDataManager.Instance.CurrentPlayerData;

            if (newChapterActive)
            {
                data.DesiredLevel = data.AllChapterProgress[data.ActiveChapterCount].UnlockedLevelCount;
                data.CurrentChapter = data.AllChapterProgress[data.ActiveChapterCount].ChapterName;
            }
            else if (data.DesiredLevel == data.AllChapterProgress[data.DesiredChapter].MaxLevelCount)
            {
                data.DesiredChapter += 1;

                data.CurrentChapter = data.AllChapterProgress[data.DesiredChapter].ChapterName;

                data.DesiredLevel = 1;
                Debug.Log("Skipped New Chapter " + data.CurrentChapter);
            }
            else
            {
                data.DesiredLevel += 1;
            }

            PlayerDataManager.Instance.CurrentPlayerData = data;

            PlayerDataManager.Instance.SaveData();
        }

        nextLevelButtonClick = true;
    }
    public void PauseButton()
    {
        pauseScreen.Show();
        GamePaused = true;
        SettingsUI.Instance.PlaySFX(SettingsUI.Instance.GameSoundData.PauseSound);
    }
    public void ResumeButton()
    {
        pauseScreen.Hide();
        GamePaused = false;

        SettingsUI.Instance.PlaySFX(SettingsUI.Instance.GameSoundData.UnpauseSound);
    }
    public void NextScene(string sceneName)
    {
        SceneTransitionManager.Instance.LoadScene(sceneName);
    }
}