using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] HpBar hpBar; 
    [SerializeField] GameObject expBar;

    [Header("Status Colors")]
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Battler _anigma;
    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Battler anigma)
    {
        ClearData();

        _anigma = anigma;

        nameText.text = anigma.Base.Name;
        SetLevel();
        if(hpText != null )
            hpText.text = anigma.HP + "/" + anigma.MaxHp;
        hpBar.SetHp((float)anigma.HP / anigma.MaxHp);
        SetExp();

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
        _anigma.OnHPChanged += UpdateHP;
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

    public void SetLevel()
    {
        levelText.text = "Lv." + _anigma.Level;
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3 (normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if (expBar == null) yield break;

        if (reset)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currentLevelExp = _anigma.Base.GetExpForLevel(_anigma.Level);
        int nextLevelExp = _anigma.Base.GetExpForLevel(_anigma.Level + 1);

        float normalizedExp = (float)(_anigma.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHpSmoothly((float)_anigma.HP / _anigma.MaxHp, hpText, _anigma);
        yield return WaitForHPUpdate();
        if (hpText != null)
            hpText.text = _anigma.HP + "/" + _anigma.MaxHp;
    }

    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }

    public void ClearData()
    {
        if (_anigma != null)
        {
            _anigma.OnStatusChanged -= SetStatusText;
            _anigma.OnHPChanged -= UpdateHP;
        }
    }
}
