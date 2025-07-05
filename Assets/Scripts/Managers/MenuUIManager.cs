using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [Header("Menu UI Manager")]
    [Space(10)]
    [Header("Blur Effect")]
    [SerializeField] private Image blurImage;

    private void Start()
    {
        Init();
    }
    private void Init()
    {
        blurImage.gameObject.SetActive(true);
    }
    //Test
    public void PlayButton()
    {
        SceneTransitionManager.Instance.LoadScene("Game");
    }
}