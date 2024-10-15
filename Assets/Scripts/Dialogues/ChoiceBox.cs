using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceBox : MonoBehaviour
{
    [SerializeField] ChoiceText choicePrefab;

    List<ChoiceText> choiceTexts;
    int currentChoice = 0;

    bool choiceSelected = false;

    Vector3 originalPosition;

    bool isAnimating = false;

    private void Awake()
    {
        originalPosition = transform.localPosition;
        var t = GetComponent<RectTransform>();
        transform.localPosition = new Vector3(originalPosition.x + t.rect.width * 2f, originalPosition.y);
    }

    public IEnumerator ShowChoices(List<string> choices, Action<int> onChoiceSelected)
    {
        choiceSelected = false;
        gameObject.SetActive(true);

        // Delete existing choices
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        choiceTexts = new List<ChoiceText>();

        foreach (var choice in choices)
        {
            var choiceTextObj = Instantiate(choicePrefab, transform);
            choiceTextObj.TextField.text = choice;
            choiceTexts.Add(choiceTextObj);
        }

        // TODO - Open Animation
        yield return OpenTabAnim();

        yield return new WaitUntil(() => choiceSelected == true);

        // TODO - Close Animation
        StartCoroutine(CloseTabAnim());

        onChoiceSelected?.Invoke(currentChoice);

        yield return new WaitUntil(() => isAnimating == false);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Down) && !isAnimating)
            currentChoice++;
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Up) && !isAnimating)
            currentChoice--;

        currentChoice = GameController.Instance.RotateSelection(currentChoice, choiceTexts.Count - 1);

        for (int i = 0; i < choiceTexts.Count; i++)
        {
            choiceTexts[i].SetSelected(i == currentChoice);
        }

        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter))
            choiceSelected = true;
    }

    IEnumerator OpenTabAnim()
    {
        isAnimating = true;
        var t = GetComponent<RectTransform>();
        transform.localPosition = new Vector3(originalPosition.x + t.rect.width * 1.25f, originalPosition.y);
        yield return transform.DOLocalMoveX(originalPosition.x, 0.3f).WaitForCompletion();
        isAnimating = false;
    }

    IEnumerator CloseTabAnim()
    {
        isAnimating = true;
        var t = GetComponent<RectTransform>();
        yield return transform.DOLocalMoveX(originalPosition.x + t.rect.width * 1.25f, 0.3f).WaitForCompletion();
        isAnimating = false;
    }
}
