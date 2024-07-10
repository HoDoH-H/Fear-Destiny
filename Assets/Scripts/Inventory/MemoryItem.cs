using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new memory")]
public class MemoryItem : ItemBase
{
    [SerializeField] MoveBase move;

    public MoveBase Move => move;

    public override bool Use(Anigma anigma)
    {
        // Learning move is handled from InventoryUI, If it was learned then return true
        return anigma.HasMove(move);
    }
}
