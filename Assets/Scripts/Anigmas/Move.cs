using UnityEngine;

public class Move
{
    public MoveBase Base { get; set; }
    public int UP { get; set; }

    public Move(MoveBase aBase)
    {
        Base = aBase;
        UP = aBase.UP;
    }
}
