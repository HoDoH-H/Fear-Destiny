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

    public IEnumerator CheckForMetamorphosis()
    {
        foreach (var battler in battlers)
        {
            var metamorphosis = battler.CheckForMetamorphosis();
            if (metamorphosis != null)
            {
                yield return DialogManager.Instance.ShowDialogText($"{battler.Base.Name} transformed into {metamorphosis.MetamorphosesInto.Name}!");
                battler.Metamorph(metamorphosis);
                OnUpdated?.Invoke();
            }
        }
    }

    public static BattlerParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<BattlerParty>();
    }
}
