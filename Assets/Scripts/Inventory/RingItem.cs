using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new ring")]
public class RingItem : ItemBase
{
    public override bool Use(Anigma anigma)
    {
        return true;
    }
}
