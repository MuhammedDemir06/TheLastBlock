using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;


public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [SerializeField] private GameObject canvasGameObject;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Image loadingImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if(Instance ==null)
        {
            DontDestroyOnLoad(gameObject);

            Instance = this;
            fadeImage.color = new Color(0, 0, 0, 1);
            loadingImage.color = new Color(0, 0, 0, 1);
            FadeIn();

        }
    }
    private void DisableFadeImage()
    {
        canvasGameObject.SetActive(false);
    }
    public void FadeIn()
    {
        fadeImage.DOFade(0, fadeDuration);
        loadingImage.DOFade(0, fadeDuration);

        Invoke(nameof(DisableFadeImage), fadeDuration);
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(DoTransition(sceneName));
    }
    private IEnumerator DoTransition(string sceneName)
    {
        canvasGameObject.SetActive(true);

        fadeImage.color = new Color(0, 0, 0, 0);
        loadingImage.color = new Color(1, 1, 1, 0);

        var fadeTween = fadeImage.DOFade(1f, fadeDuration);
        var loadingTween = loadingImage.DOFade(1f, fadeDuration * 0.75f);

        yield return DOTween.Sequence().Join(fadeTween).Join(loadingTween).WaitForCompletion();

        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitForEndOfFrame();

        var fadeOutTween = fadeImage.DOFade(0f, fadeDuration);
        var loadingOutTween = loadingImage.DOFade(0f, fadeDuration * 0.5f); 

        yield return DOTween.Sequence().Join(fadeOutTween).Join(loadingOutTween).WaitForCompletion();

        canvasGameObject.SetActive(false);
    }
}