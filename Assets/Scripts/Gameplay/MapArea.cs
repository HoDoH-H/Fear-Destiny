using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Anigma> wildAnigmas;

    public Anigma GetRandomWildAnigma()
    {
        var wildAnigma =  wildAnigmas[Random.Range(0, wildAnigmas.Count)];
        wildAnigma.Init();
        return wildAnigma;
    }
}
