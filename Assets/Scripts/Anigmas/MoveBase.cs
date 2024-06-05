using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Anigma/Create new move")]
public class MoveBase : ScriptableObject
{
    [Header("Informations")]
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;

    [Space]
    [Header("Attack Properties")]
    [SerializeField] AnigmaType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int up;
    [SerializeField] int priority;
    [SerializeField] AttackCategory style;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] MoveTarget target;

    public string Name {  get { return name; } }
    public string Description { get { return description; } }
    public AnigmaType Type { get {  return type; } }
    public int Power { get { return power; } }
    public int Accuracy { get { return accuracy; } }
    public bool AlwaysHits { get { return alwaysHits; } }
    public int UP { get {  return up; } }
    public int Priority { get { return priority; } }
    public AttackCategory Category { get { return style; } }
    public MoveEffects Effects { get {  return effects; } }
    public List<SecondaryEffects> Secondaries { get { return secondaryEffects; } }
    public MoveTarget Target { get { return target; } }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatBoost> Boosts { get { return boosts; } }
    public ConditionID Status { get { return status; } }
    public ConditionID VolatileStatus { get { return volatileStatus; } }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance { get { return chance; } }
    public MoveTarget Target { get { return target; } }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum AttackCategory
{
    Physical,
    Special,
    Status
}

public enum MoveTarget
{
    Foe, Self
}