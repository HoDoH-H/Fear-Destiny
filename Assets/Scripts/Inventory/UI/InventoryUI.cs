using System;
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

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] PartyScreen partyScreen;

    int selectedItem = 0;
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
        UpdateItemList();
    }

    public void UpdateItemList()
    {
        // Clear all existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.Slots)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    public void HandleUpdate(Action onBack)
    {
        if ( state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            if (Input.GetKeyDown(GlobalSettings.Instance.DownKeys[0]) || Input.GetKeyDown(GlobalSettings.Instance.DownKeys[1]))
            {
                if (selectedItem < inventory.Slots.Count-1)
                {
                    selectedItem++;
                }
                else
                {
                    selectedItem = 0;
                }
            }
            else if (Input.GetKeyDown(GlobalSettings.Instance.UpKeys[0]) || Input.GetKeyDown(GlobalSettings.Instance.UpKeys[1]))
            {
                if (selectedItem > 0)
                {
                    selectedItem--;
                }
                else
                {
                    selectedItem = inventory.Slots.Count-1;
                }
            }

            if (prevSelection != selectedItem)
                UpdateItemSelection();

            if (Input.GetKeyDown(GlobalSettings.Instance.EnterKeys[0]) || Input.GetKeyDown(GlobalSettings.Instance.EnterKeys[1]))
                OpenPartyScreen();
            else if (Input.GetKeyDown(GlobalSettings.Instance.BackKeys[0]) || Input.GetKeyDown(GlobalSettings.Instance.BackKeys[1]))
                onBack?.Invoke();
        }
        else if (state == InventoryUIState.PartySelection)
        {
            Action onSelected = () =>
            {
                // TODO - Use item on selected Anigma
            };

            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
    }

    void UpdateItemSelection()
    {
        if (inventory.Slots.Count == 0)
        {
            itemIcon.gameObject.SetActive(false);
            description.gameObject.SetActive(false);
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

        var item = inventory.Slots[selectedItem].Item;
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
