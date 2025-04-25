using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<AnigmaEncounterRecord> wildEncounters;

    int totalChance = 0;

    private void Start()
    {
        int totalChance = 0;
        foreach(var record in wildEncounters)
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chancePourcentage;

            totalChance += record.chancePourcentage;
        }
    }

    public Battler GetRandomWildAnigma()
    {
        int randval = Random.Range(1, totalChance + 1);
        var battlerRecord = wildEncounters.First(b => randval >= b.chanceLower && randval <= b.chanceUpper);

        var levelRange = battlerRecord.levelRange;
        int level = levelRange.y == 0 ? levelRange.x == 0 ? 1 : levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);

        var wildAnigma = new Battler(battlerRecord.battler, level);
        wildAnigma.Init();
        return wildAnigma;
    }
}

[System.Serializable]
public class AnigmaEncounterRecord
{
    public BattlerBase battler;
    public Vector2Int levelRange;
    [Min(1f)] public int chancePourcentage = 50;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }
}
