using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Battler
{
    [SerializeField] BattlerBase _base;
    [SerializeField] int level;

    public Battler(BattlerBase aBase, int alevel)
    {
        _base = aBase;
        level = alevel;
        Init();
    }

    public BattlerBase Base { get { return _base; } }
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

    public event Action OnStatusChanged;
    public event Action OnHPChanged;

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

            if (Moves.Count >= BattlerBase.MaxNumOfMoves)
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

    public BattlerSaveData GetSaveData()
    {
        var saveData = new BattlerSaveData()
        {
            name = Base.name,
            hp = HP,
            level = Level,
            exp = Exp,
            statusId = Status?.Id,
            moves = Moves.Select(m => m.GetSaveData()).ToList(),
        };

        return saveData;
    }

    public Battler(BattlerSaveData saveData)
    {
        _base = BattlerDB.GetObjectByName(saveData.name);
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
        Stats.Add(Stat.Attack, Mathf.FloorToInt((2 * Base.Attack + Base.IAttack + (Base.EAttack / 4f)) * Level / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((2 * Base.Defense + Base.IDefense + (Base.EDefense / 4f)) * Level / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((2 * Base.SpAttack + Base.ISpAttack + (Base.ESpAttack / 4f)) * Level / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((2 * Base.SpDefense + Base.ISpDefense + (Base.ESpDefense / 4f)) * Level / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((2 * Base.Speed + Base.ISpeed + (Base.ESpeed / 4f)) * Level / 100f) + 5);

        var oldMaxHp = MaxHp;
        MaxHp = Mathf.FloorToInt((2 * Base.MaxHp + Base.IMaxHp + (Base.EMaxHp / 4f)) * Level / 100f) + Level + 10;
        if (oldMaxHp != 0)
            HP += MaxHp - oldMaxHp;
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
        else if (boost < 0)
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
            if (StatBoosts[stat] >= 6 && boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} cannot rise more!");
            else if (StatBoosts[stat] <= -6 && boost < 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} cannot fall more!");
            else
            {
                StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
                if (boost > 0)
                {
                    if (boost > 2)
                        StatusChanges.Enqueue($"{Base.Name}'s {stat} tremendously rose!");
                    else if (boost > 1)
                        StatusChanges.Enqueue($"{Base.Name}'s {stat} sharply rose!");
                    else
                        StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
                }
                else
                {
                    if (boost < -2)
                        StatusChanges.Enqueue($"{Base.Name}'s {stat} tremendously fell!");
                    if (boost < -1)
                        StatusChanges.Enqueue($"{Base.Name}'s {stat} sharply fell!");
                    else
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
            CalculateStats();
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
        if (Moves.Count > BattlerBase.MaxNumOfMoves)
            return;

        Moves.Add(new Move(moveToLearn));
    }

    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;
    }

    public Morlen CheckForMorlenis()
    {
        return Base.Morleniss.FirstOrDefault(e => e.RequiredLevel <= level && e.RequiredItem == null);
    }

    public Morlen CheckForMorlenis(ItemBase item)
    {
        return Base.Morleniss.FirstOrDefault(e => e.RequiredItem == item && e.RequiredLevel <= level);
    }

    public void Morlen(Morlen morlenis)
    {
        _base = morlenis.MorlenInto;
        CalculateStats();
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

    public DamageDetails TakeDamage(Move move, Battler attacker, Condition weather)
    {
        float critical = 1f;
        float criticalHitRate = move.Base.HighCriticalHitRate ? 6.25f * 6f : 6.25f;
        if (UnityEngine.Random.value * 100f <= criticalHitRate && move.Base.Power > 0)
        {
            critical = 2f;
        }

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        float weatherMod = weather?.OnDamageModify?.Invoke(this, attacker, move) ?? 1f;

        var damageDetails = new DamageDetails
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false,
        };

        if(move.Base.InstaKill)
        {
            damageDetails.DamageDealt = HP;

            DecreaseHP(HP);

            return damageDetails;
        }

        if (move.Base.MirrorPain)
        {
            if(attacker.HP >= HP)
            {
                damageDetails.DamageDealt = 0;
                return damageDetails;
            }

            var targetHp = attacker.HP;
            var targetDamage = HP - targetHp;
            damageDetails.DamageDealt = targetDamage;

            DecreaseHP(targetDamage);

            return damageDetails;
        }

        var basePower = move.Base.Power;
        float attack = (move.Base.Category == AttackCategory.Physical) ? attacker.Attack : attacker.SpAttack;
        float defense = (move.Base.Category == AttackCategory.Physical) ? Defense : SpDefense;

        float powerMultiplier = move.Base.DoubleIfHalfOpponentHp && HP < MaxHp / 2 ? 2f : 1f;
        powerMultiplier = move.Base.ScaleOnAttackerHp ? powerMultiplier * attacker.HP / attacker.MaxHp : powerMultiplier;
        basePower = move.Base.ScaleOnTargetHp ? 48 * attacker.HP / attacker.MaxHp : basePower;
        powerMultiplier = move.Base.ScaleOnBPP && (Status.Id == ConditionID.brn || Status.Id == ConditionID.psn || Status.Id == ConditionID.par) ? powerMultiplier * 2 : powerMultiplier;

        float modifiers = UnityEngine.Random.Range(0.85f, 1f) * type * critical * weatherMod;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * (basePower * powerMultiplier) * (attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);
        if(damage <= 0)
            damage = 1;

        if (move.Base.Merciful)
        {
            if (damage >= HP)
                damage = HP - 1;
        }

        damageDetails.DamageDealt = damage;

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

    public void OnAfterTurn(int damageDealt)
    {
        Status?.OnAfterTurn?.Invoke(this, damageDealt);
        VolatileStatus?.OnAfterTurn?.Invoke(this, damageDealt);
    }

    public void Heal()
    {
        HP = MaxHp;
        foreach (var move in Moves)
        {
            move.UP = move.Base.UP;
        }
        Status = null;

        OnHPChanged?.Invoke();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }

    public float Critical { get; set; }

    public float TypeEffectiveness { get; set; }

    public int DamageDealt { get; set; }
}

[Serializable]
public class BattlerSaveData
{
    public string name;
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusId;
    public List<MoveSaveData> moves;
}