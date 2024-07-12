using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Anigma", menuName = "Anigma/Create new anigma")]
public class AnigmaBase : BattlerBase
{

    [Space]
    [Header("Types")]
    [SerializeField] AnigmaType type1;
    [SerializeField] AnigmaType type2;

    [Space]
    [Header("Catch Rate")]
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

    //Properties
    public AnigmaType Type1 => type1;
    public AnigmaType Type2 => type2;
    public int IMaxHp => iMaxHp;
    public int IAttack => iAttack;
    public int IDefense => iDefense;
    public int ISpAttack => iSpAttack;
    public int ISpDefense => iSpDefense;
    public int ISpeed => iSpeed;
    public int CatchRate => catchRate;
}