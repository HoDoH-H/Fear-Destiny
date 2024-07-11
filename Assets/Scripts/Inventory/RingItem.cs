using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new ring")]
public class RingItem : ItemBase
{
    [Header("Catch Rate")]
    [SerializeField] float catchRateModifier = 1;
    [SerializeField] bool alwaysCatch = false;

    public override bool Use(Anigma anigma)
    {
        return true;
    }

    public override bool CanUseOutsideBattle => false;

    public float CatchRateModifier => catchRateModifier;
    public bool AlwaysCatch => alwaysCatch;
}
