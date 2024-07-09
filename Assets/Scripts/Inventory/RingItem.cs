using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new ring")]
public class RingItem : ItemBase
{
    [Header("Catch Rate")]
    [SerializeField] float catchRateModifier = 1;
    [SerializeField] bool alwaysCatch = false;

    public override bool Use(Anigma anigma)
    {
        if (GameController.Instance.State == GameState.Battle)
            return true;

        return false;
    }

    public float CatchRateModifier => catchRateModifier;
    public bool AlwaysCatch => alwaysCatch;
}
