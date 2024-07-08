using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    [Header("Is poisonous for")]
    [SerializeField] bool poisonForHumans;
    [SerializeField] bool poisonForAnigmas;

    public string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
    public bool IsPoisonousForAnigmas => poisonForAnigmas;
    public bool IsPoisonousForHumans => poisonForHumans;

    public virtual bool Use(Anigma anigma)
    {
        return false;
    }
}
