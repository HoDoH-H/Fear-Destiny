using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AnigmaParty : MonoBehaviour
{
    [SerializeField] List<Anigma> anigmas;

    public event Action OnUpdated;

    public List<Anigma> Anigmas {  get { return anigmas; }set { anigmas = value; } }

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

    public void AddAnigma(Anigma newAnigma)
    {
        if (anigmas.Count < 6)
        {
            anigmas.Add(newAnigma);
            OnUpdated?.Invoke();
        }
        else
        {
            // TODO - Add to the pc one that's implemented
        }
    }

    public static AnigmaParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<AnigmaParty>();
    }
}
