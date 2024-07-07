using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HpBar hpBar;

    Anigma _anigma;

    public void Init(Anigma anigma)
    {
        _anigma = anigma;
        UpdateData();

        _anigma.OnHPChanged += UpdateData;
    }

    void UpdateData()
    {
        nameText.text = _anigma.Base.Name;
        levelText.text = "Lv." + _anigma.Level;
        hpBar.SetHp((float)_anigma.HP / _anigma.MaxHp);
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            nameText.color = GlobalSettings.Instance.HighlightedColor;
        }
        else
        {
            nameText.color = GlobalSettings.Instance.BaseInvColor;
        }
    }
}
