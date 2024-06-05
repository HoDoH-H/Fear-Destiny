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


    private void Awake()
    {
        Instance = this;
    }

    Dialog dialog;
    int currentLine = 0;

    public void HandleUpdate()
    {
        if (InputEvents.Instance.i_Pressed)
        {
            InputEvents.Instance.i_Pressed = false;
            if (isTyping)
            {
                isTyping = false;
                ShowLineNoDelay(dialog.Lines[currentLine]);
            }
            else
            {
                ++currentLine;
                if (currentLine < dialog.Lines.Count)
                {
                    StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                }
                else
                {
                    currentLine = 0;
                    dialogBox.SetActive(false);
                    OnCloseDialog?.Invoke();
                }
            }
        }
    }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        this.dialog = dialog;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public IEnumerator TypeDialog(string line)
    {
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
