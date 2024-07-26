using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Battler> wildAnigmas;

    public Battler GetRandomWildAnigma()
    {
        var wildAnigma =  wildAnigmas[Random.Range(0, wildAnigmas.Count)];
        wildAnigma.Init();
        return wildAnigma;
    }
}
