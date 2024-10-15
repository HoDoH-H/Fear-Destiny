using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, MoveToForget, Busy}

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
    [SerializeField] MoveSelectionUI moveSelectionUI;

    Action<ItemBase> onItemUsed;

    int selectedItem = 0;
    int selectedCategory = 0;

    MoveBase moveToLearn;

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

            selectedCategory = GameController.Instance.RotateSelection(selectedCategory, Inventory.ItemCategories.Count - 2);
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
                StartCoroutine(ItemSelected());
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
        else if (state == InventoryUIState.MoveToForget)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);

        if (GameController.Instance.State == GameState.Battle)
        {
            // In Battle
            if (!item.CanUseInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"This item cannot be used in battle.");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            // Outside Battle
            if (!item.CanUseOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"This item cannot be used outside battle.");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }

        // Check if item is ring
        if (selectedCategory == ((int)ItemCategory.Rings))
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();

            if (item is MemoryItem)
                partyScreen.ShowIfMemoryUsable(item as  MemoryItem);
        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleMemoryItems();

        var item = inventory.GetItem(selectedItem, selectedCategory);
        var anigma = partyScreen.SelectedMember;

        // Handle Morlenis items
        if (item is MorlenisItem && GameController.Instance.State != GameState.Battle)
        {
            var morlenis = anigma.CheckForMorlenis(item);
            if (morlenis != null)
            {
                yield return MorlenisManager.Instance.Morlenis(anigma, morlenis);
            }
            else
            {
                ClosePartyScreen();
                yield break;
            }
        }

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);

        if (usedItem != null)
        {
            // If the item is a ring don't show the dialog in inventory
            if (usedItem is RecoveryItem)
            {
                yield return DialogManager.Instance.ShowDialogText($"You used {usedItem.Name}");
                var recItem = usedItem as RecoveryItem;

                if (recItem.IsPoisonousForAnigmas)
                {
                    partyScreen.SelectedMember.DecreaseHP(partyScreen.SelectedMember.MaxHp / 3);
                    partyScreen.SelectedMember.SetStatus(ConditionID.psn);
                    var message = partyScreen.SelectedMember.StatusChanges.Dequeue();
                    yield return DialogManager.Instance.ShowDialogText(message);
                }
            }
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if (selectedCategory == (int)ItemCategory.Items)
                yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
        }

        ClosePartyScreen();
    }

    IEnumerator HandleMemoryItems()
    {
        var memoryItem = inventory.GetItem(selectedItem, selectedCategory) as MemoryItem;
        if (memoryItem == null)
            yield break;

        var anigma = partyScreen.SelectedMember;

        if (anigma.HasMove(memoryItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{memoryItem.Name} has already been carved into its memories.");
            yield break;
        }

        if (!memoryItem.CanBeTaught(anigma))
        {
            yield return DialogManager.Instance.ShowDialogText($"{memoryItem.Name} isn't compatible with {anigma.Base.Name}");
            yield break;
        }

        if (anigma.Moves.Count < BattlerBase.MaxNumOfMoves)
        {
            anigma.LearnMove(memoryItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{anigma.Base.Name} learned {memoryItem.Move.Name}");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"{anigma.Base.Name} is attempting to acquire {memoryItem.Move.Name}");
            yield return DialogManager.Instance.ShowDialogText($"But an Anigma cannot learn more than {BattlerBase.MaxNumOfMoves} moves!");

            yield return ChooseMoveToForget(anigma, memoryItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        }
    }

    IEnumerator ChooseMoveToForget(Battler anigma, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialogText($"Do you want that {anigma.Base.Name} forget a move to learn this new one?", true, false);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(anigma.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = InventoryUIState.MoveToForget;
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
        partyScreen.ClearMembersSlotsMessage();
        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        state = InventoryUIState.Busy;

        DialogManager.Instance.CloseDialog();

        var anigma = partyScreen.SelectedMember;

        moveSelectionUI.gameObject.SetActive(false);
        if (moveIndex == BattlerBase.MaxNumOfMoves)
        {
            // Don't learn the new move
            yield return DialogManager.Instance.ShowDialogText($"{anigma.Base.Name} did not acquired {moveToLearn.Name}");
        }
        else
        {
            // Forget the selected move and learn the new one
            var selectedMove = anigma.Moves[moveIndex].Base;

            anigma.Moves[moveIndex] = new Move(moveToLearn);
            yield return DialogManager.Instance.ShowDialogText($"{anigma.Base.Name} forgot {selectedMove.Name} and successfully acquired {moveToLearn.Name}");
        }

        moveToLearn = null;
        state = InventoryUIState.PartySelection;
        ClosePartyScreen();
    }
}
