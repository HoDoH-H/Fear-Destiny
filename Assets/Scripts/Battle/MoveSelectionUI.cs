using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ForgetMoveState { Choice, MoveSelection}

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    [SerializeField] List<TextMeshProUGUI> dialogTexts;
    [SerializeField] Color highlightedColor;
    [SerializeField] GameObject moveSelectionPanel;
    [SerializeField] GameObject dialogSelectionPanel;
    [SerializeField] BattleDialogBox dialogBox;

    int currentSelection = 0;
    bool wantToChange = true;
    bool canSelect = true;
    ForgetMoveState currState = ForgetMoveState.Choice;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i < currentMoves.Count; i++)
        {
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
    }

    public void HandleMoveSelection(Action<int> onSelected)
    {
        if (currState == ForgetMoveState.MoveSelection && canSelect)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentSelection < moveTexts.Count - 2)
                {
                    ++currentSelection;
                }
                else
                {
                    currentSelection = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentSelection > 0)
                {
                    --currentSelection;
                }
                else
                {
                    currentSelection = moveTexts.Count - 2;
                }
            }
        }
        else if (currState == ForgetMoveState.Choice && canSelect)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                wantToChange = !wantToChange;
            }
        }

        if (currState == ForgetMoveState.MoveSelection)
            UpdateMoveSelection(currentSelection);
        else if (currState == ForgetMoveState.Choice)
            UpdateDialogSelection();

        if (Input.GetKeyDown(KeyCode.KeypadEnter) && canSelect || Input.GetKeyDown(KeyCode.Return) && canSelect) 
        {
            if (currState == ForgetMoveState.MoveSelection)
            {
                wantToChange = true;
                var selected = currentSelection;
                currentSelection = 0;
                currState = ForgetMoveState.Choice;
                onSelected?.Invoke(selected);
            }
            else
            {
                if (wantToChange)
                {
                    currState = ForgetMoveState.MoveSelection;
                    StartCoroutine(UpdateState());
                }
                else
                {
                    wantToChange = true;
                    currState = ForgetMoveState.Choice;
                    onSelected?.Invoke(AnigmaBase.MaxNumOfMoves);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Backspace) && canSelect || Input.GetKeyDown(KeyCode.Escape) && canSelect)
        {
            if (currState == ForgetMoveState.MoveSelection)
            {
                currState = ForgetMoveState.Choice;
                StartCoroutine(UpdateState());
            }
            else
            {
                if (wantToChange)
                {
                    wantToChange = !wantToChange;
                    UpdateDialogSelection();
                }
                else
                {
                    wantToChange = true;
                    currState = ForgetMoveState.Choice;
                    onSelected?.Invoke(AnigmaBase.MaxNumOfMoves + 1);
                }
            }
        }
    }

    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < AnigmaBase.MaxNumOfMoves; i++)
        {
            if (i == selection)
            {
                moveTexts[i].color = highlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
    }

    IEnumerator UpdateState()
    {
        canSelect = false;
        if (currState == ForgetMoveState.MoveSelection)
        {
            dialogSelectionPanel.SetActive(false);
            yield return dialogBox.TypeDialog($"Choose a move you want to forget...");
            moveSelectionPanel.SetActive(true);
        }
        else if (currState == ForgetMoveState.Choice)
        {
            dialogSelectionPanel.SetActive(false);
            moveSelectionPanel.SetActive(false);
            yield return dialogBox.TypeDialog($"Do you want to forget a move?");
            dialogSelectionPanel.SetActive(true);
        }
        canSelect = true;
    }

    public void UpdateDialogSelection()
    {
        if (wantToChange)
        {
            dialogTexts[0].color = highlightedColor;
            dialogTexts[1].color = Color.black;
        }
        else
        {
            dialogTexts[0].color = Color.black;
            dialogTexts[1].color = highlightedColor;
        }
    }
}
