using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "Quests/Create a new quest with new system")]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id {  get; private set; }

    [Header("General")]
    public string displayName;

    [Header("Requirements")]
    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefab;

    [Header("Rewards")]
    public int moneyReward;
    public ItemBase[] itemReward;
    public Battler[] battlerReward;

    // Ensure the id of the quest is the same as the name of the scriptable object
    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
