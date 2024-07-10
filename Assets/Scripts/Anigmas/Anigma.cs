using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Anigma
{
    [SerializeField] AnigmaBase _base;
    [SerializeField] int level;

    public Anigma(AnigmaBase aBase, int alevel)
    {
        _base = aBase;
        level = alevel;
        Init();
    }

    public AnigmaBase Base { get { return _base; } }
    public int Level { get { return level; } }

    public int Exp { get; set; }
    public int HP {  get; set; }
    public List<Move> Moves {  get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; set; }
    public Condition VolatileStatus { get; set; }
    public int VolatileStatusTime { get; set; }
    public int StatusTime { get; set; }
    public event System.Action OnStatusChanged;
    public event System.Action OnHPChanged;

    public Queue<string> StatusChanges { get; private set; }

    public void Init()
    {
        //Generate moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }

            if (Moves.Count >= AnigmaBase.MaxNumOfMoves)
            {
                break;
            }
        }

        if (level > 1)
            Exp = Base.GetExpForLevel(Level);
        else
            Exp = 0;

        CalculateStats();
        HP = MaxHp;

        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    public AnigmaSaveData GetSaveData()
    {
        var saveData = new AnigmaSaveData()
        {
            name = Base.Name,
            hp = HP,
            level = Level,
            exp = Exp,
            statusId = Status?.Id,
            moves = Moves.Select(m => m.GetSaveData()).ToList(),
        };

        return saveData;
    }

    public Anigma(AnigmaSaveData saveData)
    {
        _base = AnigmaDB.GetAnigmaByName(saveData.name);
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;

        if (saveData.statusId != null)
            Status = ConditionDB.Conditions[saveData.statusId.Value];
        else
            Status = null;

        Moves = saveData.moves.Select(m => new Move(m)).ToList();

        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        VolatileStatus = null;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((2 * Base.Attack + Base.IAttack + (Base.EAttack / 4) * Level) / 100) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((2 * Base.Defense + Base.IDefense + (Base.EDefense / 4) * Level) / 100) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((2 * Base.SpAttack + Base.ISpAttack + (Base.ESpAttack / 4) * Level) / 100) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((2 * Base.SpDefense + Base.ISpDefense + (Base.ESpDefense / 4) * Level) / 100) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((2 * Base.Speed + Base.ISpeed + (Base.ESpeed / 4) * Level) / 100) + 5);

        MaxHp = Mathf.FloorToInt((2 * Base.MaxHp + Base.IMaxHp + (Base.EMaxHp / 4) * Level) / 100) + Level + 10;
    }


    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasiveness, 0}
        };
    }


    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //Apply stat modifiers
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost > 0)
        {
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        }
        else
        {
            statVal = Mathf.FloorToInt(statVal / boostValues[Mathf.Abs(boost)]);
        }

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
            {
                if (boost > 1)
                {
                    StatusChanges.Enqueue($"{Base.Name}'s {stat} tremendously rose!");
                }
                else
                {
                    StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
                }
                
            }
            else
            {
                if (boost < -1)
                {
                    StatusChanges.Enqueue($"{Base.Name}'s {stat} tremendously fell!");
                }
                else
                {
                    StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");
                }
            }
        }
    }

    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnMove(MoveBase moveToLearn)
    {
        if (Moves.Count > AnigmaBase.MaxNumOfMoves)
            return;

        Moves.Add(new Move(moveToLearn));
    }

    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHp
    {
        get; private set;
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }

    public DamageDetails TakeDamage(Move move, Anigma attacker)
    {
        float critical = 1f;
        if (UnityEngine.Random.value * 100f <= 6.25f && move.Base.Power > 0)
        {
            critical = 2f;
        }

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false,
        };

        float attack = 0;
        float defense = 0;

        attack = (move.Base.Category == AttackCategory.Physical) ? attacker.SpAttack : attacker.Attack;
        defense = (move.Base.Category == AttackCategory.Physical) ? SpDefense : Defense;

        float modifiers = UnityEngine.Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * (attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        DecreaseHP(damage);

        return damageDetails;
    }

    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void IncreaseHP(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void SetStatus(ConditionID status)
    {
        if (Status != null) { return; }

        Status = ConditionDB.Conditions[status];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID status)
    {
        if (VolatileStatus != null) { return; }

        VolatileStatus = ConditionDB.Conditions[status];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        var movesWithUP = Moves.Where(x => x.UP > 0).ToList();

        int r = UnityEngine.Random.Range(0, movesWithUP.Count);
        return movesWithUP[r];
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }
        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }

    public float Critical { get; set; }

    public float TypeEffectiveness { get; set; }
}

[Serializable]
public class AnigmaSaveData
{
    public string name;
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusId;
    public List<MoveSaveData> moves;
}