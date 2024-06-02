using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Anigma> anigmas)
    {
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
}
