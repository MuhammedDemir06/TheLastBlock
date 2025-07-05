using DG.Tweening;
using UnityEngine;

public class AnimatedPanel : MonoBehaviour
{
    [Header("Target Panel")]
    public RectTransform targetPanel;

    [Header("Animation Settings")]
    public float animationDuration = 0.4f;
    public Ease openEase = Ease.OutBack;
    public Ease closeEase = Ease.InBack;

    private Vector3 originalScale;

    public bool HideOnStart = true;

    private void Start()
    {
        if (targetPanel == null) return;

        originalScale = targetPanel.localScale;

        if (HideOnStart)
        {
            targetPanel.localScale = Vector3.zero;
            targetPanel.gameObject.SetActive(false);
        }
        else
        {
            targetPanel.gameObject.SetActive(true);
        }
    }
    public void Show()
    {
        if (targetPanel == null) return;

        targetPanel.gameObject.SetActive(true);
        targetPanel.localScale = Vector3.zero;
        targetPanel.DOScale(originalScale, animationDuration).SetEase(openEase);
    }
    public void Hide()
    {
        if (targetPanel == null) return;

        targetPanel.DOScale(Vector3.zero, animationDuration).SetEase(closeEase)
            .OnComplete(() => targetPanel.gameObject.SetActive(false));
    }
}
