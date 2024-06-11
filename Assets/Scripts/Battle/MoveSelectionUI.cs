using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum State { Choice, MoveSelection}

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    [SerializeField] Color highlightedColor;

    int currentSelection = 0;
    State currState = State.Choice;

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
        // TODO - Make Dialog to ask if player want to forgot the move
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentSelection < moveTexts.Count)
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
                currentSelection = moveTexts.Count;
            }
        }

        UpdateMoveSelection(currentSelection);

        if (Input.GetKeyDown (KeyCode.KeypadEnter | KeyCode.Return)) 
        { 
            onSelected?.Invoke(currentSelection);
        }
    }

    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < AnigmaBase.MaxNumOfMoves+1; i++)
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
}
