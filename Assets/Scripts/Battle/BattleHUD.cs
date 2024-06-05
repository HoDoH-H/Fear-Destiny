using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HpBar hpBar;

    [Header("Status Colors")]
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Anigma _anigma;
    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Anigma anigma)
    {
        _anigma = anigma;

        nameText.text = anigma.Base.Name;
        levelText.text = "Lvl" + anigma.Level;
        hpBar.SetHp((float)anigma.HP / anigma.MaxHp);

        statusColors = new Dictionary<ConditionID, Color>() 
        {
            {ConditionID.psn, psnColor },
            {ConditionID.brn, brnColor },
            {ConditionID.slp, slpColor },
            {ConditionID.par, parColor },
            {ConditionID.frz, frzColor },
        };

        SetStatusText();
        _anigma.OnStatusChanged += SetStatusText;
    }

    void SetStatusText()
    {
        if (_anigma.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _anigma.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_anigma.Status.Id];
        }
    }

    public IEnumerator UpdateHP()
    {
        if (_anigma.HpChanged)
        {
            yield return hpBar.SetHpSmoothly((float)_anigma.HP / _anigma.MaxHp);
            _anigma.HpChanged = false;
        }
    }
}
