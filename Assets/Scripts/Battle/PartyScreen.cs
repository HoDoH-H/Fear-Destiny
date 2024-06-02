using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots;
    List<Anigma> anigmas;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Anigma> anigmas)
    {
        this.anigmas = anigmas;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < anigmas.Count)
            {
                memberSlots[i].SetData(anigmas[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

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
}
