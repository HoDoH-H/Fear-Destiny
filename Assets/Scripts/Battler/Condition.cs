using System;
using UnityEngine;

public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    public string EffectMessage { get; set; }

    public Action<Battler> OnStart { get; set; }
    public Func<Battler, bool> OnBeforeMove { get; set; }
    public Action<Battler, int> OnAfterTurn {  get; set; }
    public Action<Battler> OnWeather {  get; set; }
    public Func<Battler, Battler, Move, float> OnDamageModify {  get; set; }
}
