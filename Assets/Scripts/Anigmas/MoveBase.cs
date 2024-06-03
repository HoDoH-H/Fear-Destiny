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
    [SerializeField] int usePoints;
    [SerializeField] AttackCategory style;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;

    //[Header("Status Self")]
    //[SerializeField] StatusEffectSelf[] statusSelf;
    //[UnityEngine.Range(0, 100)][SerializeField] int[] statusSelfAccuracy;

    //[Header("Status Opponent")]
    //[SerializeField] StatusEffectOpponent[] statusOpponent;
    //[UnityEngine.Range(0, 100)][SerializeField] int[] statusOpponentAccuracy;

    //[Header("Effect Self")]
    //[SerializeField] StatModifierSelf[] effectSelf;
    //[UnityEngine.Range(0, 100)][SerializeField] int EffectSelfAccuracy;
    //[SerializeField] StatModifierValues[] effectValuesSelf;

    //[Header("Effect Opponent")]
    //[SerializeField] StatModifierOpponent[] effectOpponent;
    //[UnityEngine.Range(0, 100)][SerializeField] int EffectOpponentAccuracy;
    //[SerializeField] StatModifierValues[] effectValuesOpponent;

    public string Name {  get { return name; } }
    public string Description { get { return description; } }
    public AnigmaType Type { get {  return type; } }
    public int Power { get { return power; } }
    public int Accuracy { get { return accuracy; } }
    public int UP { get {  return usePoints; } }
    public AttackCategory Category { get { return style; } }
    public MoveEffects Effects { get {  return effects; } }
    public MoveTarget Target { get { return target; } }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts { get { return boosts; } }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum StatusEffectSelf
{
    None,
    Paralyzed,
    Poisoned,
    BadlyPoisoned,
    Burned,
    Frozen,
    Flinch,
    Confused,
    Infatuation,
    LeechSeed,
    Bleed,
    Sleep,
    Deaf,
    Blind
}

public enum StatusEffectOpponent
{
    None,
    Paralyzed,
    Poisoned,
    BadlyPoisoned,
    Burned,
    Frozen,
    Flinch,
    Confused,
    Infatuation,
    LeechSeed,
    Bleed,
    Sleep,
    Deaf,
    Blind
}

public enum StatModifierSelf
{
    None,
    UpAttack,
    LowerAttack,
    UpDefense,
    LowerDefense,
    UpSpAttack,
    LowerSpAttack,
    UpSpDefense,
    LowerSpDefense,
    UpSpeed,
    LowerSpeed,
}

public enum StatModifierOpponent
{
    None,
    UpAttack,
    LowerAttack,
    UpDefense,
    LowerDefense,
    UpSpAttack,
    LowerSpAttack,
    UpSpDefense,
    LowerSpDefense,
    UpSpeed,
    LowerSpeed,
}

public enum StatModifierValues
{
    Normal,
    Sharply,
    Tremendous
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