using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

public class BattlerParty : MonoBehaviour
{
    [SerializeField] List<Battler> battlers;

    public event Action OnUpdated;

    public List<Battler> Battlers {  
        get { 
            return battlers; 
        }set { 
            battlers = value;
            OnUpdated?.Invoke();
        } 
    }

    private void Awake()
    {
        foreach (var battler in battlers)
        {
            battler.Init();
        }
    }

    public Battler GetHealthyBattler()
    {
        return battlers.Where(x => x.HP > 0).FirstOrDefault();
    }

    public void AddBattler(Battler newBattler)
    {
        if (battlers.Count < 6)
        {
            battlers.Add(newBattler);
            OnUpdated?.Invoke();
        }
        else
        {
            // TODO - Add to the pc one that's implemented
        }
    }

    public IEnumerator CheckForMorlen()
    {
        foreach (var battler in battlers)
        {
            var morlen = battler.CheckForMorlenis();
            if (morlen != null)
            {
                yield return MorlenisManager.Instance.Morlenis(battler, morlen);
            }
        }
    }

    public static BattlerParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<BattlerParty>();
    }

    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }
}
