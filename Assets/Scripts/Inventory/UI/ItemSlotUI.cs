using TMPro;
using UnityEngine;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI countText;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public TextMeshProUGUI Name => nameText;
    public TextMeshProUGUI Count => countText;
    public float Height => rectTransform.rect.height;

    public void SetData(ItemSlot itemSlot)
    {
        nameText.text = itemSlot.Item.Name;
        if (!(itemSlot.Item.IsReusable))
        {
            countText.enabled = true;
            countText.text = $"X {itemSlot.Count}";
        }
        else
        {
            if (itemSlot.Count != 1)
                itemSlot.Count = 1;
            countText.enabled = false;
        }
    }
}
