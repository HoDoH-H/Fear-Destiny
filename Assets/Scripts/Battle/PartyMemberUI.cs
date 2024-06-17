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

    public void SetData(Anigma anigma)
    {
        _anigma = anigma;

        nameText.text = anigma.Base.Name;
        levelText.text = "Lvl" + anigma.Level;
        hpBar.SetHp((float)anigma.HP / anigma.MaxHp);
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            nameText.color = GlobalSettings.Instance.HighlightedColor;
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}
