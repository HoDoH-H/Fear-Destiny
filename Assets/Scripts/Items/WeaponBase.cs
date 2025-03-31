using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new weapon")]
public class WeaponItem : ItemBase
{
    [SerializeField] WeaponType type;
    [SerializeField] int attackBoost;
    [SerializeField] Move signatureMove;
    public WeaponType Type => type;
    public int AttackBoost => attackBoost;
    public Move SignatureMove => signatureMove;
    public override bool Use(Battler anigma)
    {
        return false;
    }
}
public enum WeaponType { Sword }
