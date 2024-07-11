using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new memory")]
public class MemoryItem : ItemBase
{
    [SerializeField] MoveBase move;
    [SerializeField] bool isAncientMemory;

    public override string Name => base.Name + $": {move.Name}";

    public MoveBase Move => move;

    public override bool IsReusable => true;

    public override bool Use(Anigma anigma)
    {
        // Learning move is handled from InventoryUI, If it was learned then return true
        return anigma.HasMove(move);
    }

    public bool CanBeTaught(Anigma anigma)
    {
        return anigma.Base.LearnableByItems.Contains(move);
    }

    public override bool CanUseInBattle => false;
    public bool IsAncientMemory => isAncientMemory;
}
