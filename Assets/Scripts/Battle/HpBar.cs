using System.Collections;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public bool IsUpdating = false;

    public void SetHp(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHpSmoothly(float newHp)
    {
        IsUpdating = true;

        float curHp = health.transform.localScale.x;
        float changeAmt = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curHp, 1f);
            yield return null;
        }

        health.transform.localScale = new Vector3(newHp, 1f);

        IsUpdating = false;
    }
}
