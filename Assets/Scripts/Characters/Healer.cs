using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialog(dialog,
            choices: new List<string>() { "Accept the offer.", "Refuse Politely." },
            onChoiceSelected: (choiceIndex) => selectedChoice = choiceIndex);

        if (selectedChoice == 1)
        {
            // If user refuse
            yield return DialogManager.Instance.ShowDialogText($"Oh... Ok... Nevermind. Come back later if you want to.");
            yield break;
        }

        yield return Fader.Instance.FadeIn(0.5f);

        var playerParty = player.GetComponent<BattlerParty>();
        playerParty.Battlers.ForEach(b => b.Heal());

        playerParty.PartyUpdated();

        yield return new WaitForSeconds(0.5f);
        yield return Fader.Instance.FadeOut(0.5f);
        yield return new WaitForSeconds(0.1f);

        yield return DialogManager.Instance.ShowDialogText($"Seems like you, and your companions are in better form now!");
    }
}
