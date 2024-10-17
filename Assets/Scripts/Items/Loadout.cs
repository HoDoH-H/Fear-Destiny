using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Items/Create new loadout")]
public class Loadout : ScriptableObject
{
    [SerializeField] ArmorBase helmet;
    [SerializeField] ArmorBase torso;
    [SerializeField] ArmorBase legs;
    [SerializeField] ArmorBase shoes;

    public ArmorBase Helmet => helmet;
    public ArmorBase Torso => torso;
    public ArmorBase Legs => legs;
    public ArmorBase Shoes => shoes;

    public void SetLoadoutPiece(ArmorBase item)
    {
        if (item.Type == ArmorType.Helmet)
            helmet = item;
        else if (item.Type == ArmorType.Torso)
            torso = item;
        else if (item.Type == ArmorType.Legs)
            legs = item;
        else if (item.Type == ArmorType.Shoes)
            shoes = item;
    }
}
