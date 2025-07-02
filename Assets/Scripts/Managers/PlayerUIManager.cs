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

    private void Awake()
    {
        Instance = this;
    }
    public void DamageControl(int playerHealthAmount,int value, bool isDamage)
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

        if(playerHealthAmount<=0)
        {
            Debug.Log("Player Died!");
        }
    }
}