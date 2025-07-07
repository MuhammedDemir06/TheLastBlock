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
            Debug.Log("Player Died!");
        }
    }

    public void NextLevel()
    {
        GameFinished = true;
        GamePaused = true;
        nextLevelScreen.Show();
    }
    //Buttons
    public void PauseButton()
    {
        pauseScreen.Show();
        GamePaused = true;
    }
    public void ResumeButton()
    {
        pauseScreen.Hide();
        GamePaused = false;
    }
    public void NextScene(string sceneName)
    {
        SceneTransitionManager.Instance.LoadScene(sceneName);
    }
}