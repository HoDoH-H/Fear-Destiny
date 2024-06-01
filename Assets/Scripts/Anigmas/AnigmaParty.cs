using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AnigmaParty : MonoBehaviour
{
    [SerializeField] List<Anigma> anigmas;

    private void Start()
    {
        foreach (var anigma in anigmas)
        {
            anigma.Init();
        }
    }

    public Anigma GetHealthyPokemon()
    {
        return anigmas.Where(x => x.HP > 0).FirstOrDefault();
    }
}
