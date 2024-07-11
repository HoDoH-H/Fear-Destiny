using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int letterPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

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
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }

    public IEnumerator ShowDialog(Dialog dialog)
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

        dialogBox.SetActive(false);
        IsShowing = false;
        OnCloseDialog?.Invoke();
    }

    public IEnumerator TypeDialog(string line)
    {
        yield return new WaitForEndOfFrame();
        currentLineText = line;

        isTyping = true;

        dialogText.text = "";
        foreach (var letter in line)
        {
            if(!isTyping) { yield break; }
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
