using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class WalletUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;

    bool isAnimating;
    Vector3 originalPosition;

    public bool IsAnimating => isAnimating;

    private void Awake()
    {
        originalPosition = transform.localPosition;
        var t = GetComponent<RectTransform>();
        transform.localPosition = new Vector3(originalPosition.x, originalPosition.y - t.rect.height * 1.25f);
    }

    private void Start()
    {
        Wallet.Instance.OnMoneyChanged += SetMoneyText;
    }

    public IEnumerator Show()
    {
        isAnimating = true;

        gameObject.SetActive(true);
        SetMoneyText();

        var t = GetComponent<RectTransform>();
        transform.localPosition = new Vector3(originalPosition.x, originalPosition.y + t.rect.height * 1.25f);
        yield return transform.DOLocalMoveY(originalPosition.y, 0.3f).WaitForCompletion();

        isAnimating = false;
    }

    public IEnumerator Hide()
    {
        isAnimating = true;

        var t = GetComponent<RectTransform>();
        yield return transform.DOLocalMoveY(originalPosition.y + t.rect.height * 1.25f, 0.3f).WaitForCompletion();

        gameObject.SetActive(false);

        isAnimating = false;
    }

    void SetMoneyText()
    {
        moneyText.text = $"$ {Wallet.Instance.Money}";
    }
}
