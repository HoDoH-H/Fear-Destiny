using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("UP")]
    [SerializeField] int upAmount;
    [SerializeField] bool restoreMaxUP;

    [Header("Mana")]
    [SerializeField] int manaAmount;
    [SerializeField] bool restoreMaxMana;

    [Header("Status Conditions")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool boostedRevive;
    [SerializeField] bool maxRevive;

    [Header("Is poisonous for")]
    [SerializeField] bool humans;
    [SerializeField] bool anigmas;

    public override bool Use(Anigma anigma)
    {
        if (hpAmount > 0)
        {
            if (anigma.HP == anigma.MaxHp)
                return false;

            anigma.IncreaseHP(hpAmount);
        }

        return true;
    }
}
