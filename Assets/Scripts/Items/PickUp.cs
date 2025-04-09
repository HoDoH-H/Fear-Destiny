using System.Collections;
using UnityEngine;

public class PickUp : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] ItemBase item;

    public bool Used { get; set; } = false;

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            initiator.GetComponent<Inventory>().AddItem(item);

            Used = true;

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            AudioManager.Instance.PlaySFX(item.Rarity == ItemRarity.Minor ? AudioId.MinorDiscovery : AudioId.GreatDiscovery, true);
            yield return DialogManager.Instance.ShowDialogText($"{initiator.GetComponent<PlayerController>().Name} found {item.Name}");
        }
    }

    public object CaptureState()
    {
        return Used;
    }

    public void RestoreState(object state)
    {
        Used = (bool)state;
        GetComponent<SpriteRenderer>().enabled = !Used;
        GetComponent<BoxCollider2D>().enabled = !Used;
    }
}
