using System.Collections.Generic;
using UnityEngine;

public class BattlerDB
{
    static Dictionary<string, BattlerBase> anigmas;

    public static void Init()
    {
        anigmas = new Dictionary<string, BattlerBase>();

        var anigmaArray = Resources.LoadAll<BattlerBase>("");
        foreach (var anigma in anigmaArray)
        {
            if (anigmas.ContainsKey(anigma.name))
            {
                Debug.LogError($"There are two anigmas with the same name {anigma.Name}.");
                continue;
            }
            anigmas[anigma.name] = anigma;
        }
    }

    public static BattlerBase GetBattlerByName( string name)
    {
        if ( !anigmas.ContainsKey(name))
        {
            Debug.LogError($"Anigma with name {name} not found in the database.");
            return null;
        }
        
        return anigmas[name];
    }
}
