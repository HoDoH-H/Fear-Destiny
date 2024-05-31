using System.Collections.Generic;
using UnityEngine;

public class Anigma
{
    public AnigmaBase Base {  get; set; }
    public int Level { get; set; }

    public int HP {  get; set; }

    public List<Move> Moves {  get; set; }

    public Anigma(AnigmaBase aBase, int aLevel)
    {
        Base = aBase;
        Level = aLevel;
        HP = MaxHp;

        //Geenerate the moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }

            if (Moves.Count >= 4)
            {
                break;
            }
        }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((2 * Base.Attack + Base.IAttack + (Base.EAttack / 4) * Level) / 100) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((2 * Base.Defense + Base.IDefense + (Base.EDefense / 4) * Level) / 100) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((2 * Base.SpAttack + Base.ISpAttack + (Base.ESpAttack / 4) * Level) / 100) + 5; }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt((2 * Base.SpDefense + Base.ISpDefense + (Base.ESpDefense / 4) * Level) / 100) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((2 * Base.Speed + Base.ISpeed + (Base.ESpeed / 4) * Level) / 100) + 5; }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt((2 * Base.MaxHp + Base.IMaxHp + (Base.EMaxHp / 4) * Level) / 100) + Level + 10; }
    }
}
