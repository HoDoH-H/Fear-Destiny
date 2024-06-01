using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;

    Anigma _anigma;

    public void SetData(Anigma anigma)
    {
        _anigma = anigma;

        nameText.text = anigma.Base.Name;
        levelText.text = "Lvl" + anigma.Level;
        hpBar.SetHp((float)anigma.HP / anigma.MaxHp);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHpSmoothly((float)_anigma.HP / _anigma.MaxHp);
    }
}
