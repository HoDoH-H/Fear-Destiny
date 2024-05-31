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
    public Sprite Sprite { get { return frontSprite; } }
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
    Fight,
    Flying,
    Poison,
    Ground,
    Rock,
    Bug,
    Ghost,
    Steel,
    Fire,
    Water,
    Grass,
    Electric,
    Psychic,
    Ice,
    Dragon,
    Dark,
    Sound,
    Bright,
    Magic
}