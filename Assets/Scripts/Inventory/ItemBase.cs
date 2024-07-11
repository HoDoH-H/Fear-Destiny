using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    [Header("Is poisonous for")]
    [SerializeField] bool poisonForHumans;
    [SerializeField] bool poisonForAnigmas;

    public virtual string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
    public bool IsPoisonousForAnigmas => poisonForAnigmas;
    public bool IsPoisonousForHumans => poisonForHumans;

    public virtual bool Use(Anigma anigma)
    {
        return false;
    }

    public virtual bool IsReusable => false;
    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;
}
