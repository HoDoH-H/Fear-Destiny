using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int letterPerSecond;
    [SerializeField] ChoiceBox choiceBox;

    public event Action OnShowDialog;
    public event Action OnDialogFinished;

    public static DialogManager Instance { get; private set; }

    private bool isTyping = false;

    public bool IsShowing { get; private set; }


    private void Awake()
    {
        Instance = this;
    }
    string currentLineText;

    public void HandleUpdate()
    {
        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter))
        {
            if (isTyping)
            {
                isTyping = false;
                ShowLineNoDelay(currentLineText);
            }
        }
    }

    public IEnumerator ShowDialogText(string text, bool waitForInput=true, bool autoClose=true)
    {
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);
        currentLineText = text;
        yield return TypeDialog(text);

        yield return new WaitForEndOfFrame();

        if (waitForInput)
            yield return new WaitUntil(() => GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter));

        if (autoClose)
        {
            CloseDialog();
        }
        OnDialogFinished?.Invoke();
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }

    public IEnumerator ShowDialog(Dialog dialog, List<string> choices = null, 
        Action<int> onChoiceSelected=null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        foreach (var line in dialog.Lines)
        {
            yield return TypeDialog(line);
            yield return new WaitUntil(() => GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        dialogBox.SetActive(false);
        IsShowing = false;
        OnDialogFinished?.Invoke();
    }

    public IEnumerator TypeDialog(string line)
    {
        yield return new WaitForEndOfFrame();
        currentLineText = line;

        isTyping = true;

        dialogText.text = "";
        foreach (var letter in line)
        {
            if(!isTyping) 
            {
                yield return new WaitForEndOfFrame();
                yield break; 
            }
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }

        isTyping = false;
    }

    public void ShowLineNoDelay(string line)
    {
        dialogText.text = line;
    }
}
