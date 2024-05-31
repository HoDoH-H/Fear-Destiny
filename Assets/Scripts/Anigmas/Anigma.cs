using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Anigma
{
    AnigmaBase _base;
    int level;

    public int HP {  get; set; }

    public List<Move> Moves {  get; set; }

    public Anigma(AnigmaBase aBase, int aLevel)
    {
        _base = aBase;
        level = aLevel;
        HP = _base.MaxHp;

        //Geenerate the moves
        Moves = new List<Move>();
        foreach (var move in _base.LearnableMoves)
        {
            if (move.Level <= level)
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
        get { return Mathf.FloorToInt((2 * _base.Attack + _base.IAttack + (_base.EAttack / 4) * level) / 100) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((2 * _base.Defense + _base.IDefense + (_base.EDefense / 4) * level) / 100) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((2 * _base.SpAttack + _base.ISpAttack + (_base.ESpAttack / 4) * level) / 100) + 5; }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt((2 * _base.SpDefense + _base.ISpDefense + (_base.ESpDefense / 4) * level) / 100) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((2 * _base.Speed + _base.ISpeed + (_base.ESpeed / 4) * level) / 100) + 5; }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt((2 * _base.MaxHp + _base.IMaxHp + (_base.EMaxHp / 4) * level) / 100) + level + 10; }
    }
}
