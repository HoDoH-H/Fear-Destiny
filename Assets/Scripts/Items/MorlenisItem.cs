using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new morlenis item")]
public class MorlenisItem : ItemBase
{
    public override bool Use(Battler anigma)
    {
        if (GameController.Instance.State != GameState.Battle)
            return true;
        return false;
    }
}
