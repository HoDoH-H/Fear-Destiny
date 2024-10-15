using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "Battler", menuName = "Battler/Create new battler")]
public class BattlerBase : ScriptableObject
{
    [Header("Informations")]
    [SerializeField] bool isPlayer;
    [SerializeField] bool isHuman;
    [SerializeField] bool isAnigma;
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [Space]
    [Header("Sprites")]
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [Space]
    [Header("Base Stats")]
    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int maxMp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;
    [SerializeField] int expYield;
    [SerializeField] int catchRate = 255;
    [SerializeField] GrowthRate growthRate = GrowthRate.MediumFast;

    [Space]
    [Header("Types")]
    [SerializeField] AnigmaType type1;
    [SerializeField] AnigmaType type2;

    [Space]
    [Header("Learnable Moves")]
    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> learnableByItems;

    [Space]
    [Header("Morlenis")]
    [SerializeField] List<Morlen> morleniss;

    [Space]
    [Header("Experience Power")]
    //EP (Experience Power)
    [SerializeField] int eMaxHp;
    [SerializeField] int eAttack;
    [SerializeField] int eDefense;
    [SerializeField] int eSpAttack;
    [SerializeField] int eSpDefense;
    [SerializeField] int eSpeed;

    [Space]
    [Header("Inherited Power")]
    //IP (Inherited Power)
    [SerializeField] int iMaxHp;
    [SerializeField] int iAttack;
    [SerializeField] int iDefense;
    [SerializeField] int iSpAttack;
    [SerializeField] int iSpDefense;
    [SerializeField] int iSpeed;

    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * Mathf.FloorToInt(Mathf.Pow(level, 3)) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3));
        }
        else if (growthRate == GrowthRate.MediumSlow)
        {
            return Mathf.FloorToInt(6f / 5f * Mathf.FloorToInt(Mathf.Pow(level, 3)) - (15 * Mathf.FloorToInt(Mathf.Pow(level, 2))) + (100 * level) - 140);
        }
        else if (growthRate == GrowthRate.Slow)
        {
            return 5 * Mathf.FloorToInt(Mathf.Pow(level, 3)) / 4;
        }
        else if (growthRate == GrowthRate.Heratic)
        {
            if (level < 50)
            {
                return (Mathf.FloorToInt(Mathf.Pow(level, 3)) * (100 - level)) / 50;
            }
            else if (level < 68)
            {
                return (Mathf.FloorToInt(Mathf.Pow(level, 3)) * (150 - level)) / 100;
            }
            else if (level < 98)
            {
                return (Mathf.FloorToInt(Mathf.Pow(level, 3) * ((1911 - 10 * level) / 3))) / 500;
            }
            else
            {
                return (Mathf.FloorToInt(Mathf.Pow(level, 3)) * (160 - level)) / 100;
            }
        }
        else
        {
            if (level < 15)
            {
                return Mathf.FloorToInt(Mathf.Pow(level, 3)) * ((level + 1) / 3 + 24) / 50;
            }
            else if (level < 36)
            {
                return Mathf.FloorToInt(Mathf.Pow(level, 3)) * (level + 14) / 50;
            }
            else
            {
                return (Mathf.FloorToInt(Mathf.Pow(level, 3)) - (level / 2 + 32)) / 50;
            }
        }
    }

    public void ChangeName(string value)
    {
        name = value;
    }

    //Properties
    public bool IsPlayer => isPlayer;
    public bool IsHuman => isHuman;
    public bool IsAnigma => isAnigma;
    public string Name => name;
    public string Description => description;
    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;
    public int MaxHp => maxHp;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;
    public int EMaxHp => eMaxHp;
    public int EAttack => eAttack;
    public int EDefense => eDefense;
    public int ESpAttack => eSpAttack;
    public int ESpDefense => eSpDefense;
    public int ESpeed => eSpeed;
    public List<LearnableMove> LearnableMoves => learnableMoves;
    public List<MoveBase> LearnableByItems => learnableByItems;
    public List<Morlen> Morleniss => morleniss;
    public int ExpYield => expYield;
    public AnigmaType Type1 => type1;
    public AnigmaType Type2 => type2;
    public int IMaxHp => iMaxHp;
    public int IAttack => iAttack;
    public int IDefense => iDefense;
    public int ISpAttack => iSpAttack;
    public int ISpDefense => iSpDefense;
    public int ISpeed => iSpeed;
    public int CatchRate => catchRate;
    public static int MaxNumOfMoves { get; set; } = 4;
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base => moveBase;
    public int Level => level;
}

[System.Serializable]
public class Morlen
{
    [SerializeField] BattlerBase morlenInto;
    [SerializeField] int requiredLevel;
    [SerializeField] MorlenisItem requiredItem;

    public BattlerBase MorlenInto => morlenInto;
    public int RequiredLevel => requiredLevel;
    public MorlenisItem RequiredItem => requiredItem;
}

public enum AnigmaType
{
    None,
    Neutralis,
    Verdure,
    Pyro,
    Hydro,
    Voltis,
    Glacio,
    Strife,
    Venomis,
    Terran,
    Zephyr,
    Mentis,
    Chitin,
    Crag,
    Spectra,
    Draconis,
    Umbra,
    Ferrum,
    Sonaris,
    Luminis,
    Arcanis,
    Plasmis
}

public enum GrowthRate
{
    Heratic,
    Fast,
    MediumFast,
    MediumSlow,
    Slow,
    Spasmodic,
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    //Used for move accuracy
    Accuracy,
    Evasiveness
}

public class TypeChart
{
    static float[][] chart =
    {
        //                    NOR   GRA   FIR   WAT   ELE   ICE   FIG   POI   GRO   FLY   PSY   BUG   ROC   GHO   DRA   DAR   STE   SOU   BRI   MAG   PLA
        /*NOR*/ new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.0f, 1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f},
        /*GRA*/ new float[] { 1.0f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f, 0.5f, 2.0f, 0.5f, 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f},
        /*FIR*/ new float[] { 1.0f, 2.0f, 0.5f, 0.5f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f},
        /*WAT*/ new float[] { 1.0f, 0.5f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f},
        /*ELE*/ new float[] { 1.0f, 0.5f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 0.5f, 2.0f, 2.0f},
        /*ICE*/ new float[] { 1.0f, 2.0f, 0.5f, 0.5f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f},
        /*FIG*/ new float[] { 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f, 1.0f, 0.5f, 0.5f, 0.5f, 2.0f, 0.0f, 1.0f, 2.0f, 2.0f, 0.0f, 1.0f, 2.0f, 1.0f},
        /*POI*/ new float[] { 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 2.0f, 1.0f, 0.5f},
        /*GRO*/ new float[] { 1.0f, 0.5f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 2.0f, 2.0f, 1.0f, 1.0f, 2.0f},
        /*FLY*/ new float[] { 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f},
        /*PSY*/ new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.5f, 1.0f, 1.0f, 2.0f, 1.0f},
        /*BUG*/ new float[] { 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f},
        /*ROC*/ new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f, 2.0f},
        /*GHO*/ new float[] { 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f, 2.0f},
        /*DRA*/ new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f, 0.5f, 1.0f, 2.0f, 1.0f},
        /*DAR*/ new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 0.5f, 2.0f},
        /*STE*/ new float[] { 1.0f, 0.5f, 0.5f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f},
        /*SOU*/ new float[] { 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f},
        /*BRI*/ new float[] { 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.0f},
        /*MAG*/ new float[] { 1.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.0f, 1.0f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f},
        /*PLA*/ new float[] { 1.0f, 2.0f, 0.5f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f},
    };

    public static float GetEffectiveness(AnigmaType attackType, AnigmaType defenseType)
    {
        if (attackType == AnigmaType.None || defenseType == AnigmaType.None)
        {
            return 1.0f;
        }

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}