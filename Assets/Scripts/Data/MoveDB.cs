using System.Collections.Generic;
using UnityEngine;

public class MoveDB
{
    static Dictionary<string, MoveBase> moves;

    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();

        var moveArray = Resources.LoadAll<MoveBase>("");
        foreach (var move in moveArray)
        {
            if (moves.ContainsKey(move.name))
            {
                Debug.LogError($"There are two moves with the same name {move.Name}.");
                continue;
            }
            moves[move.name] = move;
        }
    }

    public static MoveBase GetMoveByName(string name)
    {
        if (!moves.ContainsKey(name))
        {
            Debug.LogError($"Move with name {name} not found in the database.");
            return null;
        }

        return moves[name];
    }
}