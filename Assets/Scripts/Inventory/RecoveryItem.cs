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

    public override bool Use(Anigma anigma)
    {
        // Revive
        if (revive || boostedRevive || maxRevive) 
        {
            if (anigma.HP > 0)
                return false;

            if (revive)
                anigma.IncreaseHP(anigma.MaxHp / 5);
            else if (boostedRevive)
                anigma.IncreaseHP(anigma.MaxHp / 2);
            else if (maxRevive)
                anigma.IncreaseHP(anigma.MaxHp);

            anigma.CureStatus();

            return true;
        }

        // We don't want to use non revive items on dead anigmas.
        if (anigma.HP <= 0)
            return false;

        // Restore HP
        if (restoreMaxHP || hpAmount > 0)
        {
            if (anigma.HP == anigma.MaxHp)
                return false;

            if (restoreMaxHP)
                anigma.IncreaseHP(anigma.MaxHp);
            else
                anigma.IncreaseHP(hpAmount);
        }

        // Recover Status
        if (recoverAllStatus || status != ConditionID.None)
        {
            if (anigma.Status == null && anigma.VolatileStatus == null)
                return false;

            if (recoverAllStatus)
            {
                anigma.CureStatus();
                anigma.CureVolatileStatus();
            }
            else
            {
                if (anigma.Status.Id == status)
                    anigma.CureStatus();
                else if (anigma.VolatileStatus.Id == status)
                    anigma.CureVolatileStatus();
                else
                    return false;
            }
        }

        // Restore PP
        if (restoreMaxUP)
        {
            anigma.Moves.ForEach(m => m.IncreaseUP(m.Base.UP));
        }
        else if (upAmount > 0)
        {
            anigma.Moves.ForEach(m => m.IncreaseUP(upAmount));
        }

        return true;
    }
}
