using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CountSelectorUI : MonoBehaviour
{
    TextMeshProUGUI text;

    bool isAnimating;
    Vector2 originalPosition;

    bool selected;
    int currentCount;

    int maxCount;
    float pricePerUnit;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        originalPosition = transform.localPosition;
        var t = GetComponent<RectTransform>();
        transform.localPosition = new Vector3(originalPosition.x + t.rect.width * 1.25f, originalPosition.y);
    }

    public IEnumerator ShowSelector(int maxCount, float pricePerUnit,
        Action<int> onCountSelected)
    {
        isAnimating = true;
        selected = false;
        currentCount = 1;

        this.maxCount = maxCount;
        this.pricePerUnit = pricePerUnit;

        gameObject.SetActive(true);
        SetValues();
        var t = GetComponent<RectTransform>();
        transform.localPosition = new Vector3(originalPosition.x + t.rect.width * 1.25f, originalPosition.y);
        yield return transform.DOLocalMoveX(originalPosition.x, 0.2f).WaitForCompletion();
        yield return new WaitForSeconds(0.1f);

        isAnimating = false;

        yield return new WaitUntil(() => selected == true);

        onCountSelected?.Invoke(currentCount);

        yield return transform.DOLocalMoveX(originalPosition.x + t.rect.width * 1.25f, 0.2f).WaitForCompletion();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isAnimating && !selected)
        {
            var prevCount = currentCount;

            if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Up))
                currentCount++;
            else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Down))
                currentCount--;

            currentCount = Mathf.Clamp(currentCount, 1, maxCount);

            if (currentCount != prevCount)
                SetValues();

            if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter))
                selected = true;
            else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Back))
            {
                currentCount = 0;
                selected = true;
            }
        }
    }

    void SetValues()
    {
        text.text = $"x{currentCount}  -  $ {pricePerUnit * currentCount}";
    }
}
