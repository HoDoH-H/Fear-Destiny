using System.Collections;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public QuestBase Base {  get; private set; }
    public QuestStatus Status { get; private set; }

    public Quest(QuestBase _base)
    {
        Base = _base;
    }

    public Quest(QuestSaveData saveData)
    {
        Base = QuestDB.GetObjectByName(saveData.name);
        Status = saveData.status;
    }

    public QuestSaveData GetSaveData()
    {
        var saveData = new QuestSaveData()
        {
            name = Base.name,
            status = Status,
        };

        return saveData;
    }

    public IEnumerator StartQuest()
    {
        Status = QuestStatus.Started;

        yield return DialogManager.Instance.ShowDialog(Base.StartDialog);

        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
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

        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
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

[System.Serializable]
public class QuestSaveData
{
    public string name;
    public QuestStatus status;
    public RequiredBattleStatus battleStatus;
}

public enum QuestStatus { None, Started, Completed, Failed}