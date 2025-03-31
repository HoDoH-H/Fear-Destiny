using System;
using UnityEngine;

[Serializable]
public class ArmorBase : ScriptableObject
{
    ArmorType type;

    public ArmorType Type => type;
}

public enum ArmorType { Helmet, Torso, Legs, Shoes}
