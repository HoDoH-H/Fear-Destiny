using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions {  get; set; } = new Dictionary<ConditionID, Condition>() 
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned!",
                OnAfterTurn = (Battler anigma, int damageDealt) =>
                {
                    anigma.DecreaseHP(anigma.MaxHp / 8);
                    anigma.StatusChanges.Enqueue($"{anigma.Base.Name} is hurt by poison");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned!",
                OnAfterTurn = (Battler anigma, int damageDealt) =>
                {
                    anigma.DecreaseHP(anigma.MaxHp / 16);
                    anigma.StatusChanges.Enqueue($"{anigma.Base.Name} is hurt by its burn");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyze",
                StartMessage = "has been paralyzed!",
                OnBeforeMove = (Battler anigma) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        anigma.StatusChanges.Enqueue($"{anigma.Base.Name} is paralyzed and can't move...");
                        return false;
                    }

                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been frozen!",
                OnBeforeMove = (Battler anigma) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        anigma.CureStatus();
                        anigma.StatusChanges.Enqueue($"{anigma.Base.Name} is not frozen anymore!");
                        return true;
                    }

                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep!",
                OnStart = (Battler anigma) =>
                {
                    // Sleep for 1-3 turns
                    anigma.StatusTime = Random.Range(1, 4);
                },
                OnBeforeMove = (Battler anigma) =>
                {
                    if (anigma.StatusTime <= 0)
                    {
                        anigma.CureStatus();
                        anigma.StatusChanges.Enqueue($"{anigma.Base.Name} woke up!");
                        return true;
                    }

                    anigma.StatusTime--;
                    anigma.StatusChanges.Enqueue($"{anigma.Base.Name} is sleeping...");

                    return false;
                }
            }
        },

        //Volatile Statuses
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "Confusion",
                StartMessage = "has been confused!",
                OnStart = (Battler anigma) =>
                {
                    // confuse for 1-4 turns
                    anigma.VolatileStatusTime = Random.Range(1, 5);
                },
                OnBeforeMove = (Battler anigma) =>
                {
                    if (anigma.VolatileStatusTime <= 0)
                    {
                        anigma.CureVolatileStatus();
                        anigma.StatusChanges.Enqueue($"{anigma.Base.Name} snap out of its confusion!");
                        return true;
                    }
                    anigma.VolatileStatusTime--;

                    anigma.StatusChanges.Enqueue($"{anigma.Base.Name} is confused.");

                    //50% chance to do a move
                    if (Random.Range(1, 3) == 1)
                        return true;

                    anigma.StatusChanges.Enqueue($"{anigma.Base.Name} hurt itself in its confusion.");
                    return false;
                }
            }
        },
        {
            ConditionID.flinch,
            new Condition()
            {
                Name = "Flinch",
                StartMessage = "flinched!",
                OnBeforeMove = (Battler anigma) =>
                {
                    anigma.CureVolatileStatus();
                    return false;
                }
            }
        },
        {
            ConditionID.recoil,
            new Condition()
            {
                Name = "Recoil",
                StartMessage = "hurt itself while striking.",
                OnAfterTurn = (Battler anigma, int damageDealt) =>
                {
                    anigma.DecreaseHP(damageDealt / 3);
                    anigma.CureVolatileStatus();
                }
            }
        },
        {
            ConditionID.recoverHp,
            new Condition()
            {
                Name = "Recover HP",
                StartMessage = "is recovering its HPs.",
                OnAfterTurn = (Battler anigma, int damageDealt) =>
                {
                    anigma.IncreaseHP(damageDealt / 2);
                    anigma.CureVolatileStatus();
                }
            }
        },
        {
            ConditionID.recover,
            new Condition()
            {
                Name = "Recover",
                StartMessage = "is exhausted.",
                OnStart = (Battler anigma) =>
                {
                    // confuse for 1-4 turns
                    anigma.VolatileStatusTime = Random.Range(1, 3);
                },
                OnBeforeMove = (Battler anigma) =>
                {
                    if (anigma.VolatileStatusTime <= 0)
                    {
                        anigma.CureVolatileStatus();
                        anigma.StatusChanges.Enqueue($"{anigma.Base.Name} recovered enough!");
                        return true;
                    }
                    anigma.VolatileStatusTime--;

                    anigma.StatusChanges.Enqueue($"{anigma.Base.Name} is recovering.");
                    return false;
                }
            }
        }
    };

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
            return 2f;
        else if (condition.Id == ConditionID.par || condition.Id == ConditionID.psn || condition.Id == ConditionID.brn)
            return 1.5f;

        return 1f;
    }
}

public enum ConditionID
{
    None,
    psn,
    brn,
    slp,
    par,
    frz,
    bpsn,
    flinch,
    confusion,
    recoil,
    recoverHp,
    recover,
    infatuation,
    leechSeed,
    bleed,
    deaf,
    blnd
}