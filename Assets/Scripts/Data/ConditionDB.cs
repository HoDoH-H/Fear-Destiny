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
                OnAfterTurn = (Anigma anigma) =>
                {
                    anigma.UpdateHP(anigma.MaxHp / 8);
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
                OnAfterTurn = (Anigma anigma) =>
                {
                    anigma.UpdateHP(anigma.MaxHp / 16);
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
                OnBeforeMove = (Anigma anigma) =>
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
                OnBeforeMove = (Anigma anigma) =>
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
                OnStart = (Anigma anigma) =>
                {
                    // Sleep for 1-3 turns
                    anigma.StatusTime = Random.Range(1, 4);
                },
                OnBeforeMove = (Anigma anigma) =>
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
                OnStart = (Anigma anigma) =>
                {
                    // confuse for 1-4 turns
                    anigma.VolatileStatusTime = Random.Range(1, 5);
                },
                OnBeforeMove = (Anigma anigma) =>
                {
                    if (anigma.VolatileStatusTime <= 0)
                    {
                        anigma.CureVolatileStatus();
                        anigma.StatusChanges.Enqueue($"{anigma.Base.Name} snap out of its confusion!");
                        return true;
                    }
                    anigma.VolatileStatusTime--;

                    anigma.StatusChanges.Enqueue($"{anigma.Base.Name} is confused");

                    //50% chance to do a move
                    if (Random.Range(1, 3) == 1)
                        return true;

                    anigma.StatusChanges.Enqueue($"{anigma.Base.Name} hurt itself in its confusion");
                    anigma.UpdateHP(anigma.MaxHp / 8);
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
    infatuation,
    leechSeed,
    bleed,
    deaf,
    blnd
}