using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;
    [SerializeField] Color highlightedColor;

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
            nameText.color = highlightedColor;
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}
