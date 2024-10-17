using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] bool isSellable;
    [SerializeField] float price;

    public virtual string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
    public float Price => price;
    public bool IsSellable => isSellable;

    public virtual bool Use(Battler anigma)
    {
        return false;
    }

    public virtual bool IsReusable => false;
    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;
}
