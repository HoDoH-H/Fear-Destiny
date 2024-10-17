using System;
using UnityEngine;

[Serializable]
public class ArmorBase
{
    ArmorType type;

    public ArmorType Type => type;
}

public enum ArmorType { Helmet, Torso, Legs, Shoes}
