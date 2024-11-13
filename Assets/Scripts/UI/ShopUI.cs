using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject itemListBg;
    [SerializeField] GameObject itemList;
    [SerializeField] GameObject itemDesc;
    [SerializeField] GameObject itemIconObj;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    List<ItemBase> availableItems;
    Action<ItemBase> onItemSelected;
    Action onBack;

    List<ItemSlotUI> slotUIList;

    int selectedItem;

    const int itemsInViewport = 8;
    RectTransform itemListRect;

    Vector2 itemListOriginalPosition;
    Vector2 itemDescOriginalPosition;
    Vector2 itemIconOriginalPosition;

    bool isAnimating;

    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();

        itemListOriginalPosition = itemListBg.transform.localPosition;
        itemDescOriginalPosition = itemDesc.transform.localPosition;
        itemIconOriginalPosition = itemIconObj.transform.localPosition;

        var t = itemListBg.GetComponent<RectTransform>();
        itemListBg.transform.localPosition = new Vector3(itemListOriginalPosition.x + t.rect.width * 2f, itemListOriginalPosition.y);
        t = itemDesc.GetComponent<RectTransform>();
        itemDesc.transform.localPosition = new Vector3(itemDescOriginalPosition.x - t.rect.width * 2f, itemDescOriginalPosition.y);
        t = itemIconObj.GetComponent<RectTransform>();
        itemIconObj.transform.localPosition = new Vector3(itemIconOriginalPosition.x - t.rect.width * 2f, itemIconOriginalPosition.y);
    }

    public IEnumerator Show(List<ItemBase> availableItems, Action<ItemBase> onItemSelected, Action onBack)
    {
        this.availableItems = availableItems;
        this.onItemSelected = onItemSelected;
        this.onBack = onBack;

        isAnimating = true;
        gameObject.SetActive(true);
        UpdateItemList();

        #region Show Animation


        // Item list anim
        var t = itemListBg.GetComponent<RectTransform>();
        itemListBg.transform.localPosition = new Vector3(itemListOriginalPosition.x + t.rect.width * 1.25f, itemListOriginalPosition.y);
        yield return itemListBg.transform.DOLocalMoveX(itemListOriginalPosition.x, 0.3f);
        yield return new WaitForSeconds(0.1f);

        // Item desc anim
        t = itemDesc.GetComponent<RectTransform>();
        itemDesc.transform.localPosition = new Vector3(itemDescOriginalPosition.x - t.rect.width * 1.25f, itemDescOriginalPosition.y);
        yield return itemDesc.transform.DOLocalMoveX(itemDescOriginalPosition.x, 0.3f);

        // Item icon anim
        t = itemIconObj.GetComponent<RectTransform>();
        itemIconObj.transform.localPosition = new Vector3(itemIconOriginalPosition.x - t.rect.width * 1.25f, itemIconOriginalPosition.y);
        yield return itemIconObj.transform.DOLocalMoveX(itemIconOriginalPosition.x, 0.3f).WaitForCompletion();
        yield return new WaitForSeconds(0.1f);


        #endregion Show Animation

        isAnimating = false;
    }

    public IEnumerator Hide()
    {
        isAnimating = true;

        #region Hide Animation


        // Item list anim
        var t = itemListBg.GetComponent<RectTransform>();
        yield return itemListBg.transform.DOLocalMoveX(itemListOriginalPosition.x + t.rect.width * 1.25f, 0.3f);
        yield return new WaitForSeconds(0.1f);

        // Item desc anim
        t = itemDesc.GetComponent<RectTransform>();
        yield return itemDesc.transform.DOLocalMoveX(itemDescOriginalPosition.x - t.rect.width * 1.25f, 0.3f);

        // Item icon anim
        t = itemIconObj.GetComponent<RectTransform>();
        yield return itemIconObj.transform.DOLocalMoveX(itemIconOriginalPosition.x - t.rect.width * 1.25f, 0.3f).WaitForCompletion();
        yield return new WaitForSeconds(0.1f);


        #endregion Hide Animation

        gameObject.SetActive(false);
        isAnimating = false;
    }

    public void HandleUpdate()
    {
        if (!isAnimating)
        {
            var prevSelection = selectedItem;

            if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Down))
                selectedItem++;
            else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Up))
                selectedItem--;

            selectedItem = GameController.Instance.RotateSelection(selectedItem, slotUIList.Count - 1);

            if (prevSelection != selectedItem)
            {
                UpdateItemSelection();
            }

            if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter))
                onItemSelected?.Invoke(availableItems[selectedItem]);
            else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Back))
                onBack?.Invoke();
        }
    }

    void UpdateItemList()
    {
        // Clear all existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var item in availableItems)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetDataForShop(item);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    void UpdateItemSelection()
    {
        selectedItem = GameController.Instance.RotateSelection(selectedItem, availableItems.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].Name.color = GlobalSettings.Instance.HighlightedColor;
            }
            else
            {
                slotUIList[i].Name.color = GlobalSettings.Instance.BaseInvColor;
            }
        }

        var item = availableItems[selectedItem];
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Description;

        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewport)
            return;

        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport / 2, 0, slotUIList.Count) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > itemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + itemsInViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }
}
