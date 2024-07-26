using System.Collections;
using UnityEngine;

public class BattlerGiver : MonoBehaviour, ISavable
{
    [SerializeField] Battler battlerToGive;
    [SerializeField] Dialog dialog;
    [SerializeField] bool isReusable = false;

    bool used = false;

    public IEnumerator GiveBattler(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        battlerToGive.Init();
        player.GetComponent<BattlerParty>().AddBattler(battlerToGive);

        used = !isReusable;

        yield return DialogManager.Instance.ShowDialogText($"{ player.Name} received { battlerToGive.Base.Name}");
    }

    public bool CanBeGiven()
    {
        return battlerToGive != null && !used;
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;
    }
}
