using System.Collections;
using TMPro;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public bool IsUpdating = false;

    public void SetHp(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHpSmoothly(float newHp, TextMeshProUGUI hpText, Battler battler)
    {
        IsUpdating = true;

        float curHp = health.transform.localScale.x;

        if (curHp > newHp)
        {
            float changeAmt = curHp - newHp;

            while (curHp - newHp > Mathf.Epsilon)
            {
                curHp -= changeAmt * Time.deltaTime;
                health.transform.localScale = new Vector3(curHp, 1f);
                if (hpText != null)
                    hpText.text = Mathf.CeilToInt(battler.HP - curHp) + "/" + battler.MaxHp;
                yield return null;
            }
        }
        else
        {
            float changeAmt = newHp - curHp;

            while (newHp - curHp > Mathf.Epsilon)
            {
                curHp += changeAmt * Time.deltaTime;
                health.transform.localScale = new Vector3(curHp, 1f);
                if (hpText != null)
                    hpText.text = Mathf.FloorToInt(battler.HP - curHp) + "/" + battler.MaxHp;
                yield return null;
            }
        }

        health.transform.localScale = new Vector3(newHp, 1f);

        IsUpdating = false;
    }
}
