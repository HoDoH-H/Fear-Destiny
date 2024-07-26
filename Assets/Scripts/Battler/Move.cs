using System;
using Unity.VisualScripting;
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

    public Move(MoveSaveData saveData)
    {
        Base = MoveDB.GetMoveByName(saveData.name);
        UP = saveData.UP;
    }

    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            name = Base.Name,
            UP = UP,
        };

        return saveData;
    }

    public void IncreaseUP(int amount)
    {
        UP = Mathf.Clamp(UP + amount, 0, Base.UP);
    }
}

[Serializable]
public class MoveSaveData
{
    public string name;
    public int UP;
}