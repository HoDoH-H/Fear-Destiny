using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, Busy}

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI category;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] PartyScreen partyScreen;

    Action<ItemBase> onItemUsed;

    int selectedItem = 0;
    int selectedCategory = 0;
    InventoryUIState state;

    const int itemsInViewport = 8;

    List<ItemSlotUI> slotUIList;
    Inventory inventory;
    RectTransform itemListRect;
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        ResetSelection();
        UpdateItemList();

        inventory.OnUpdated += UpdateItemList;
    }

    public void UpdateItemList()
    {
        // Clear all existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        category.text = Inventory.ItemCategories[selectedCategory];
        UpdateItemSelection();
    }

    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed=null)
    {
        this.onItemUsed = onItemUsed;

        if ( state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            int prevCat = selectedCategory;

            if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Down))
                selectedItem++;
            else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Up))
                selectedItem--;
            else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Right))
                selectedCategory++;
            else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Left))
                selectedCategory--;

            selectedCategory = GameController.Instance.RotateSelection(selectedCategory, Inventory.ItemCategories.Count - 1);
            selectedItem = GameController.Instance.RotateSelection(selectedItem, inventory.GetSlotsByCategory(selectedCategory).Count - 1);

            if (prevCat != selectedCategory)
            {
                ResetSelection();
                category.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList(); 
            }
            else if (prevSelection != selectedItem)
                UpdateItemSelection();

            if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter))
                ItemSelected();
            else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Back))
                onBack?.Invoke();
        }
        else if (state == InventoryUIState.PartySelection)
        {
            Action onSelected = () =>
            {
                StartCoroutine(UseItem());
            };

            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
    }

    void ItemSelected()
    {
        // Check if item is ring
        if (selectedCategory == ((int)ItemCategory.Rings))
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();
        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);

        if (usedItem != null)
        {
            // If the item is a ring don't show the dialog in inventory
            if (!(usedItem is RingItem))
                yield return DialogManager.Instance.ShowDialogText($"You used {usedItem.Name}");

            if (usedItem.IsPoisonousForAnigmas)
            {
                partyScreen.SelectedMember.DecreaseHP(partyScreen.SelectedMember.MaxHp / 3);
                partyScreen.SelectedMember.SetStatus(ConditionID.psn);
                var message = partyScreen.SelectedMember.StatusChanges.Dequeue();
                yield return DialogManager.Instance.ShowDialogText(message);
            }
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
        }

        ClosePartyScreen();
    }

    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);

        if (slots.Count == 0)
        {
            ResetSelection();
            return;
        }
        else
        {
            itemIcon.gameObject.SetActive(true);
            description.gameObject.SetActive(true);
        }

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

        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

        var item = slots[selectedItem].Item;
        itemIcon.sprite = item.Icon;
        description.text = item.Description;

        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewport)
            return;

        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport/2, 0, slotUIList.Count) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > itemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + itemsInViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }

    void ResetSelection()
    {
        selectedItem = 0;

        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.gameObject.SetActive(false);
        description.text = null;
        description.gameObject.SetActive(false);
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.gameObject.SetActive(false);
    }
}
