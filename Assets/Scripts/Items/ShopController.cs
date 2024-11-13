using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ShopState { Menu, Buying, Selling, Busy }

public class ShopController : MonoBehaviour
{
    [SerializeField] Vector2 cameraOffset;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;
    [SerializeField] ShopUI shopUI;

    public event Action OnStart;
    public event Action OnFinish;

    Merchant merchant;

    public static ShopController Instance { get; private set; }

    ShopState state;

    Inventory inventory;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public IEnumerator StartTrading(Merchant merchant)
    {
        OnStart?.Invoke();
        this.merchant = merchant;
        yield return StartMenuState();
    }

    IEnumerator StartMenuState(bool instant = false)
    {
        state = ShopState.Menu;
        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialogText("How may I serve you?", 
            waitForInput: false, 
            choices: new List<string>() { "Buy", "Sell", "Leave" }, 
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex, showTextNoDelay: instant);

        if (selectedChoice == 0)
        {
            // Buy
            StartCoroutine(GameController.Instance.MoveCamera(cameraOffset));
            StartCoroutine(walletUI.Show());
            yield return shopUI.Show(merchant.AvailableItems, (item) => StartCoroutine(BuyItem(item)), () => StartCoroutine(OnBackFromBuying()));
            state = ShopState.Buying;
        }
        else if (selectedChoice == 1)
        {
            // Sell
            state = ShopState.Selling;
            inventoryUI.gameObject.SetActive(true);
        }
        else if (selectedChoice == 2 || selectedChoice == -1)
        {
            // Quit
            OnFinish?.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if (state == ShopState.Selling)
        {
            inventoryUI.HandleUpdate(OnBackFromSelling, (selectedItem) => { StartCoroutine(SellItem(selectedItem)); });
        }
        else if (state == ShopState.Buying)
        {
            shopUI.HandleUpdate();
        }
    }

    void OnBackFromSelling()
    {
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState(instant:true));
    }

    IEnumerator SellItem(ItemBase item)
    {
        state = ShopState.Busy;

        if (!item.IsSellable)
        {
            yield return DialogManager.Instance.ShowDialogText("You can't sell that!");
            state = ShopState.Selling;
            yield break;
        }

        yield return walletUI.Show();

        float sellingPrice = Mathf.Round(item.Price / 2);
        var countToSell = 1;

        var itemCount = inventory.GetItemCount(item);
        if (itemCount > 1)
        {
            yield return DialogManager.Instance.ShowDialogText($"How many {item.Name}s do you want to sell?", waitForInput:false, autoClose:false);

            yield return countSelectorUI.ShowSelector(itemCount, sellingPrice, (selectedCount) => countToSell = selectedCount);

            DialogManager.Instance.CloseDialog();

            if (countToSell == 0)
            {
                yield return walletUI.Hide();
                state = ShopState.Selling;
                yield break;
            }
        }

        sellingPrice *= countToSell;

        int selectedChoice = 0;
        if(countToSell == 1)
            yield return DialogManager.Instance.ShowDialogText($"Do you really want to sell this {item.Name}?", 
                waitForInput: false, 
                choices: new List<string>() { "Confirm", "Back" }, 
                onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
        else if (countToSell > 1)
            yield return DialogManager.Instance.ShowDialogText($"Do you really want to sell {countToSell} {item.Name}s?",
                waitForInput: false,
                choices: new List<string>() { "Confirm", "Back" },
                onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            // Sell
            inventory.RemoveItem(item, countToSell);
            // Add item price into player's wallet
            Wallet.Instance.AddMoney(sellingPrice);
            if (countToSell == 1)
                yield return DialogManager.Instance.ShowDialogText($"You sold this {item.Name} and received {sellingPrice} lumis.");
            else if (countToSell > 1)
                yield return DialogManager.Instance.ShowDialogText($"You sold {countToSell} {item.Name}s and received {sellingPrice} lumis.");
        }

        yield return walletUI.Hide();

        state = ShopState.Selling;
    }

    IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.Busy;

        if (!Wallet.Instance.HasMoney(item.Price))
        {
            yield return DialogManager.Instance.ShowDialogText($"You don't have enough money for that!");
            state = ShopState.Buying;
            yield break;
        }

        yield return DialogManager.Instance.ShowDialogText($"How many {item.Name}s do you want to buy?", waitForInput: false, autoClose: false);

        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(100, item.Price, (selectedCount) => countToBuy = selectedCount);

        DialogManager.Instance.CloseDialog();

        float totalPrice = item.Price * countToBuy;

        if (Wallet.Instance.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            if (countToBuy == 1)
                yield return DialogManager.Instance.ShowDialogText($"Do you really want to buy this {item.Name} for {totalPrice} lumis?",
                    waitForInput: false,
                    choices: new List<string>() { "Buy", "Cancel" },
                    onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
            else if (countToBuy > 1)
                yield return DialogManager.Instance.ShowDialogText($"Do you really want to buy {countToBuy} {item.Name}s for {totalPrice} lumis?",
                    waitForInput: false,
                    choices: new List<string>() { "Buy", "Cancel" },
                    onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

            if (selectedChoice == 0)
            {
                // User confirmed the purchase
                inventory.AddItem(item, countToBuy);
                Wallet.Instance.TakeMoney(totalPrice);
                yield return DialogManager.Instance.ShowDialogText($"Thank you for your purchase!");
            }
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"You don't have enough money for that!");
        }

        state = ShopState.Buying;
    }

    IEnumerator OnBackFromBuying()
    {
        StartCoroutine(GameController.Instance.MoveCamera(-cameraOffset));
        StartCoroutine(shopUI.Hide());
        yield return walletUI.Hide();
        yield return StartMenuState();
    }
}
