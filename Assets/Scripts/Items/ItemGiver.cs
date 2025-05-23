using System.Collections;
using UnityEngine;

public class ItemGiver : MonoBehaviour, ISavable
{
    [SerializeField] ItemBase item;
    [SerializeField] int count = 1;
    [SerializeField] Dialog dialog;
    [SerializeField] bool isReusable = false;

    bool used = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        player.GetComponent<Inventory>().AddItem(item, count);

        used = !isReusable;

        AudioManager.Instance.PlaySFX(item.Rarity == ItemRarity.Minor ? AudioId.MinorDiscovery : AudioId.GreatDiscovery, true);

        string dialogText = $"{player.Name} received {item.Name}";
        if (count > 1)
        {
            dialogText = $"{player.Name} received {count} {item.Name}s";
        }

        yield return DialogManager.Instance.ShowDialogText(dialogText);
    }

    public bool CanBeGiven()
    {
        return item != null && count > 0 && !used;
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
