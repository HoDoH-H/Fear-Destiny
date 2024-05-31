using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] AnigmaBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Anigma Anigma { get; set; }

    public void Setup()
    {
        Anigma = new Anigma(_base, level);
        if (isPlayerUnit && Anigma.Base.BackSprite != null)
        {
            GetComponent<Image>().sprite = Anigma.Base.BackSprite;
        }else
        {
            GetComponent<Image>().sprite = Anigma.Base.FrontSprite;
        }
    }
}
