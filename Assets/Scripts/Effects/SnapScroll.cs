using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private float scrollDuration = 0.3f;
    [SerializeField] private int currentIndex = 0;

    private List<RectTransform> chapterPanels = new();

    private void Start()
    {
        foreach (Transform child in content)
            chapterPanels.Add(child as RectTransform);

        SnapTo(currentIndex);
    }
    public void SnapTo(int index)
    {
        if (index < 0 || index >= chapterPanels.Count) return;

        currentIndex = index;

        float targetPos = (float)index / (chapterPanels.Count - 1);
        DOTween.To(() => scrollRect.horizontalNormalizedPosition,
                   x => scrollRect.horizontalNormalizedPosition = x,
                   targetPos,
                   scrollDuration);
    }
    public void NextChapter()
    {
        if (currentIndex < chapterPanels.Count - 1)
            SnapTo(currentIndex + 1);
    }
    public void PreviousChapter()
    {
        if (currentIndex > 0)
            SnapTo(currentIndex - 1);
    }
}
