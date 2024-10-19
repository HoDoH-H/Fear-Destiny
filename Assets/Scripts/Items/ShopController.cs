using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ShopState { Menu, Buying, Selling, Busy }

public class ShopController : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;

    public event Action OnStart;
    public event Action OnFinish;

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
        yield return StartMenuState();
    }

    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;
        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialogText("How may I serve you?", 
            waitForInput: false, 
            choices: new List<string>() { "Buy", "Sell", "Leave" }, 
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            // Buy
        }
        else if (selectedChoice == 1)
        {
            // Sell
            state = ShopState.Selling;
            inventoryUI.gameObject.SetActive(true);
        }
        else if (selectedChoice == 2)
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
    }

    void OnBackFromSelling()
    {
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState());
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

        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialogText("Do you really want to sell this item?", 
            waitForInput: false, 
            choices: new List<string>() { "Confirm", "Back" }, 
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            // Sell
            inventory.RemoveItem(item);
            // Add item price into player's wallet
            Wallet.Instance.AddMoney(sellingPrice);
            yield return DialogManager.Instance.ShowDialogText($"You sold {item.Name} and received {sellingPrice} lumis.");
        }

        yield return walletUI.Hide();

        state = ShopState.Selling;
    }
}
