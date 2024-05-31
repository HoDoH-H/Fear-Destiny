using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;

    public void SetData(Anigma anigma)
    {
        nameText.text = anigma.Base.Name;
        levelText.text = "Lvl" + anigma.Level;
        hpBar.SetHp((float)anigma.HP / anigma.MaxHp);
    }
}
