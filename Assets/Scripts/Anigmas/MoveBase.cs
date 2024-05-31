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

    [Header("Status Self")]
    [SerializeField] StatusEffectSelf[] statusSelf;
    [Range(0, 100)][SerializeField] int[] statusSelfAccuracy;

    [Header("Status Opponent")]
    [SerializeField] StatusEffectOpponent[] statusOpponent;
    [Range(0, 100)][SerializeField] int[] statusOpponentAccuracy;

    [Header("Effect Self")]
    [SerializeField] StatModifierSelf[] effectSelf;
    [Range(0, 100)][SerializeField] int EffectSelfAccuracy;
    [SerializeField] StatModifierValues[] effectValuesSelf;

    [Header("Effect Opponent")]
    [SerializeField] StatModifierOpponent[] effectOpponent;
    [Range(0, 100)][SerializeField] int EffectOpponentAccuracy;
    [SerializeField] StatModifierValues[] effectValuesOpponent;

    public string Name {  get { return name; } }
    public string Description { get { return description; } }
    public AnigmaType Type { get {  return type; } }
    public int Power { get { return power; } }
    public int Accuracy { get { return accuracy; } }
    public int UsePoints { get {  return usePoints; } }
    public StatusEffectSelf[] StatusSelf { get {  return statusSelf; } }
    public int[] StatusSelfAccuracy { get { return statusSelfAccuracy; } }
    public StatusEffectOpponent[] StatusOpponent { get { return statusOpponent; } }
    public int[] StatusOpponentAccuracy { get { return statusOpponentAccuracy; } }
    public StatModifierSelf[] EffectSelf { get { return effectSelf; } }
    public StatModifierOpponent[] EffectOpponent { get { return effectOpponent; } }
    public StatModifierValues[] EffectValuesSelf { get { return effectValuesSelf; } }
    public StatModifierValues[] EffectValuesOpponent { get { return effectValuesOpponent;} }
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