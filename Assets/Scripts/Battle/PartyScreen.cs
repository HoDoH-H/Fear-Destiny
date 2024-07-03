using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;

    PartyMemberUI[] memberSlots;
    List<Anigma> anigmas;

    int selection = 0;

    public Anigma SelectedMember => anigmas[selection];

    /// <summary>
    /// Party screen can be called from different states like ActionSelection, RunningTurn, AboutToUse
    /// </summary>
    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }

    public void SetPartyData(List<Anigma> anigmas)
    {
        this.anigmas = anigmas;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < anigmas.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(anigmas[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        UpdateMemberSelection(selection);

        messageText.text = "Choose a Anigma.";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < anigmas.Count; i++)
        {
            if (i == selectedMember)
            {
                memberSlots[i].SetSelected(true);
            }
            else
            {
                memberSlots[i].SetSelected(false);
            }
        }
    }

    public void SetMessageText(string messageText)
    {
        this.messageText.text = messageText;
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selection == 0 && anigmas.Count > 1 || selection == 2 && anigmas.Count > 3 || selection == 4 && anigmas.Count > 5)
            {
                ++selection;
            }
            else if (selection == 2 && anigmas.Count == 3 || selection == 4 && anigmas.Count == 5)
            {
                --selection;
            }
            else
            {
                if (selection == 1 || selection == 3 || selection == 5)
                    --selection;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selection == 1 || selection == 3 || selection == 5)
            {
                --selection;
            }
            else
            {
                if (anigmas.Count > 1 && selection == 0 || anigmas.Count > 3 && selection == 2 || anigmas.Count > 5 && selection == 4)
                {
                    ++selection;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selection > 1)
            {
                selection = selection - 2;
            }
            else
            {
                if (anigmas.Count > 4 && selection == 0 || anigmas.Count > 5 && selection == 1)
                {
                    selection = selection + 4;
                }
                else if (anigmas.Count > 2 && selection == 0 || anigmas.Count > 3 && selection == 1)
                {
                    selection = selection + 2;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selection + 2 < anigmas.Count)
            {
                selection = selection + 2;
            }
            else if (selection + 1 < anigmas.Count && selection % 2 == 1)
            {
                selection++;
            }
            else
            {
                if (selection % 2 == 0)
                    selection = 0;
                else
                    selection = 1;
            }
        }

        if (selection != prevSelection)
            UpdateMemberSelection(selection);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
        {
            onBack?.Invoke();
        }
    }
}
