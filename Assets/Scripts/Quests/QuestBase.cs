using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Create a new quest")]
public class QuestBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;

    [SerializeField] Dialog startDialog;
    [SerializeField] Dialog inProgressDialog;
    [SerializeField] Dialog completedDialog;

    [SerializeField] ItemBase requireItem;
    [SerializeField] ItemBase rewardItem;

    public string Name => name;
    public string Description => description;

    public Dialog StartDialog => startDialog;
    public Dialog InProgressDialog => inProgressDialog?.Lines.Count > 0 ? inProgressDialog : startDialog;
    public Dialog CompletedDialog => completedDialog;

    public ItemBase RequireItem => requireItem;
    public ItemBase RewardItem => rewardItem;
}

public enum RequiredBattleStatus { None, Won, Lost}