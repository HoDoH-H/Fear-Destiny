using System.Collections;
using UnityEngine;

public class Quest
{
    public QuestBase Base {  get; private set; }
    public QuestStatus Status { get; private set; }

    public Quest(QuestBase _base)
    {
        Base = _base;
    }

    public IEnumerator StartQuest()
    {
        Status = QuestStatus.Started;

        yield return DialogManager.Instance.ShowDialog(Base.StartDialog);
    }

    public IEnumerator CompleteQuest()
    {
        Status = QuestStatus.Completed;

        yield return DialogManager.Instance.ShowDialog(Base.CompletedDialog);

        var inventory = Inventory.GetInventory();
        if (Base.RequireItem != null)
        {
            inventory.RemoveItem(Base.RequireItem);
        }

        if (Base.RewardItem != null)
        {
            inventory.AddItem(Base.RewardItem);

            yield return DialogManager.Instance.ShowDialogText($"{GameController.Instance.Player.Name} received {Base.RewardItem.Name}");
        }
    }

    public bool CanBeCompleted()
    {
        var inventory = Inventory.GetInventory();
        if (Base.RequireItem != null)
        {
            return inventory.HasItem(Base.RequireItem);
        }
        return true;
    }
}

public enum QuestStatus { None, Started, Completed}
