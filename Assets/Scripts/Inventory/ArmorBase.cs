using UnityEngine;

public class ArmorBase : MonoBehaviour
{
    ArmorType type;

    public ArmorType Type => type;
}

public enum ArmorType { Helmet, Torso, Legs, Shoes}
