using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AnigmaParty : MonoBehaviour
{
    [SerializeField] List<Anigma> anigmas;

    public List<Anigma> Anigmas {  get { return anigmas; } }

    private void Start()
    {
        foreach (var anigma in anigmas)
        {
            anigma.Init();
        }
    }

    public Anigma GetHealthyAnigma()
    {
        return anigmas.Where(x => x.HP > 0).FirstOrDefault();
    }
}
