using System;
using UnityEngine;

public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }

    public Action<Battler> OnStart { get; set; }
    public Func<Battler, bool> OnBeforeMove { get; set; }
    public Action<Battler> OnAfterTurn {  get; set; }
}