using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Anigma", menuName = "Anigma/Create new anigma")]
public class AnigmaBase : ScriptableObject
{
    [Header("Informations")]
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [Space]
    [Header("Sprites")]
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [Space]
    [Header("Types")]
    [SerializeField] AnigmaType type1;
    [SerializeField] AnigmaType type2;

    [Space]
    [Header("Base Stats")]
    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;
    [SerializeField] int catchRate = 255;

    [Space]
    [Header("Inherited Power")]
    //IP (Inherited Power)
    [SerializeField] int iMaxHp;
    [SerializeField] int iAttack;
    [SerializeField] int iDefense;
    [SerializeField] int iSpAttack;
    [SerializeField] int iSpDefense;
    [SerializeField] int iSpeed;

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
    [Header("Learnable Moves")]
    [SerializeField] List<LearnableMove> learnableMoves;



    //Properties
    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public Sprite FrontSprite { get { return frontSprite; } }
    public Sprite BackSprite { get { return backSprite; } }
    public AnigmaType Type1 { get { return type1; } }
    public AnigmaType Type2 { get { return type2; } }
    public int MaxHp { get { return maxHp; } }
    public int Attack { get { return attack; } }
    public int Defense { get { return defense; } }
    public int SpAttack { get { return spAttack; } }
    public int SpDefense { get { return spDefense; } }
    public int Speed { get { return speed; } }
    public int IMaxHp { get { return iMaxHp; } }
    public int IAttack { get { return iAttack; } }
    public int IDefense { get { return iDefense; } }
    public int ISpAttack { get { return iSpAttack; } }
    public int ISpDefense { get { return iSpDefense; } }
    public int ISpeed { get { return iSpeed; } }
    public int EMaxHp { get { return eMaxHp; } }
    public int EAttack { get { return eAttack; } }
    public int EDefense { get { return eDefense; } }
    public int ESpAttack { get { return eSpAttack; } }
    public int ESpDefense { get { return eSpDefense; } }
    public int ESpeed { get { return eSpeed; } }
    public List<LearnableMove> LearnableMoves{get{ return learnableMoves; } }
    public int CatchRate => catchRate;
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base { get { return moveBase; } }
    public int Level { get { return level; } }
}

public enum AnigmaType
{
    None,
    Normal,
    Grass,
    Fire,
    Water,
    Electric,
    Ice,
    Fight,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Sound,
    Bright,
    Magic
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
        /*ELE*/ new float[] { 1.0f, 0.5f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 0.5f, 2.0f, 1.0f},
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