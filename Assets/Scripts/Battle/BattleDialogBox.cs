using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int letterPerSecond;

    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject choiceBox;

    [SerializeField] List<TextMeshProUGUI> actionTexts;
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    [SerializeField] List<TextMeshProUGUI> memberTexts;

    [SerializeField] TextMeshProUGUI upText;
    [SerializeField] TextMeshProUGUI typeText;

    [SerializeField] TextMeshProUGUI yesText;
    [SerializeField] TextMeshProUGUI noText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/letterPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enable)
    {
        dialogText.gameObject.SetActive(enable);
    }

    public void EnableActionSelector(bool enable)
    {
        actionSelector.SetActive(enable);
    }

    public void EnableChoiceBox(bool enable)
    {
        choiceBox.SetActive(enable);
    }

    public void EnableMoveSelector(bool enable)
    {
        moveSelector.SetActive(enable);
        moveDetails.SetActive(enable);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = GlobalSettings.Instance.HighlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateChoiceBox(bool yesSelected)
    {
        if (yesSelected)
        {
            yesText.color = GlobalSettings.Instance.HighlightedColor;
            noText.color = Color.black;
        }
        else
        {
            yesText.color = Color.black;
            noText.color = GlobalSettings.Instance.HighlightedColor;
        }
    }

    public void UpdateMoveSelection(int selectedAction, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                moveTexts[i].color = GlobalSettings.Instance.HighlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }

        if (move != null)
        {
            upText.text = $"UP {move.UP}/{move.Base.UP}";
            typeText.text = move.Base.Type.ToString();

            if (move.UP <= 0)
                upText.color = Color.red;
            else
                upText.color = Color.black;
        }
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0;i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }
}
