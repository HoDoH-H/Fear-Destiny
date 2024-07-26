using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI HPText;
    [SerializeField] TextMeshProUGUI MessageText;
    [SerializeField] HpBar hpBar;

    Battler _anigma;

    public void Init(Battler anigma)
    {
        _anigma = anigma;
        UpdateData();
        SetMessage("");

        _anigma.OnHPChanged += UpdateData;
    }

    void UpdateData()
    {
        nameText.text = _anigma.Base.Name;
        levelText.text = "Lv." + _anigma.Level;
        HPText.text = $"{_anigma.HP} / {_anigma.MaxHp}";
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

    public void SetMessage(string message)
    {
        if (message == "")
        {
            levelText.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(levelText.gameObject.transform.localPosition.x, -27.5f, 0);
        }
        else
        {
            levelText.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(levelText.gameObject.transform.localPosition.x, -2.5f, 0);
        }

        MessageText.text = message;
    }
}
