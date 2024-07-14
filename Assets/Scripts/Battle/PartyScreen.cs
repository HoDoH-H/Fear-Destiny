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
    AnigmaParty party;

    int selection = 0;

    public Anigma SelectedMember => anigmas[selection];

    /// <summary>
    /// Party screen can be called from different states like ActionSelection, RunningTurn, AboutToUse
    /// </summary>
    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

        party = AnigmaParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void SetPartyData()
    {
        anigmas = party.Anigmas;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < anigmas.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(anigmas[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        UpdateMemberSelection(selection);

        messageText.text = "Choose an Anigma.";
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

        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Right))
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
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Left))
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
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Up))
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
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Down))
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

        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter))
        {
            onSelected?.Invoke();
        }
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Back))
        {
            onBack?.Invoke();
        }
    }

    public void ShowIfMemoryUsable(MemoryItem memory)
    {
        for (int i = 0; i < anigmas.Count; i++)
        {
            string message = memory.CanBeTaught(anigmas[i]) ? "Compatible!" : "Not Compatible!";
            message = anigmas[i].HasMove(memory.Move) ? "Already Carved!" : message;
            memberSlots[i].SetMessage(message);
        }
    }

    public void ClearMembersSlotsMessage()
    {
        for (int i = 0; i < anigmas.Count; i++)
        {
            memberSlots[i].SetMessage("");
        }
    }
}
