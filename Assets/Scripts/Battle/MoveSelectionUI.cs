using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
            if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Down))
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
            else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Up))
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
            if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Down) || GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Up))
            {
                wantToChange = !wantToChange;
            }
        }

        if (currState == ForgetMoveState.MoveSelection)
            UpdateMoveSelection(currentSelection);
        else if (currState == ForgetMoveState.Choice)
            UpdateDialogSelection();

        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter) && canSelect) 
        {
            if (currState == ForgetMoveState.MoveSelection)
            {
                dialogSelectionPanel.SetActive(false);
                moveSelectionPanel.SetActive(false);
                dialogSelectionPanel.SetActive(true);
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
                    onSelected?.Invoke(BattlerBase.MaxNumOfMoves);
                }
            }
        }
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Back) && canSelect)
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
                    onSelected?.Invoke(BattlerBase.MaxNumOfMoves);
                }
            }
        }
    }

    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < BattlerBase.MaxNumOfMoves; i++)
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
            if (GameController.Instance.State == GameState.Battle)
                yield return dialogBox.TypeDialog($"Choose a move you want to forget...");
            else
                yield return DialogManager.Instance.ShowDialogText($"Choose a move you want to forget...", true, false);
            moveSelectionPanel.SetActive(true);
        }
        else if (currState == ForgetMoveState.Choice)
        {
            dialogSelectionPanel.SetActive(false);
            moveSelectionPanel.SetActive(false);
            if (GameController.Instance.State == GameState.Battle)
                yield return dialogBox.TypeDialog($"Do you want to forget a move?");
            else
                yield return DialogManager.Instance.ShowDialogText($"Do you want to forget a move?", true, false);
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
