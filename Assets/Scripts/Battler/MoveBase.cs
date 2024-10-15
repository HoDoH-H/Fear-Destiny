using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Battler/Create new move")]
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
    [SerializeField] bool highCriticalHitRate;
    [SerializeField] bool doubleIfHalfOpponentHp;
    [SerializeField] bool scaleOnHp;
    [SerializeField] AttackCategory style;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] MoveTarget target;
    [SerializeField] Vector2Int hitRange = new Vector2Int(1, 1);

    public string Name => name;
    public string Description => description;
    public AnigmaType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public bool AlwaysHits => alwaysHits;
    public bool HighCriticalHitRate => highCriticalHitRate;
    public bool DoubleIfHalfOpponentHp => doubleIfHalfOpponentHp;
    public bool ScaleOnHp => scaleOnHp;
    public int UP => up;
    public int Priority => priority;
    public AttackCategory Category => style;
    public MoveEffects Effects => effects;
    public List<SecondaryEffects> Secondaries => secondaryEffects;
    public MoveTarget Target => target;
    public Vector2Int HitRange {  get { return hitRange; } set { hitRange = value; } }

    public int GetHitTimes()
    {
        if (hitRange == Vector2Int.zero)
            return 1;

        int hitCount = 1;
        if (hitRange.y == 0)
        {
            hitCount = hitRange.x;
        }
        else
        {
            hitCount = Random.Range(hitRange.x, hitRange.y + 1);
        }

        return hitCount;
    }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;
    [SerializeField] ConditionID weather;

    public List<StatBoost> Boosts => boosts;
    public ConditionID Status => status;
    public ConditionID VolatileStatus => volatileStatus;
    public ConditionID Weather => weather;
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance => chance;
    public MoveTarget Target => target;
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