using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> itemsSlots;
    [SerializeField] List<ItemSlot> recoverySlots;
    [SerializeField] List<ItemSlot> ringSlots;
    [SerializeField] List<ItemSlot> memorySlots;

    List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>> { itemsSlots, recoverySlots, ringSlots, memorySlots};
    }

    public static List<string> ItemCategories { get; set; } = new List<string>() 
    { 
        "Items", "Recovery", "Rings", "Memories"
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase UseItem(int itemIndex, Anigma selectedAnigma, int selectedCategory)
    {
        var currSlot = GetSlotsByCategory(selectedCategory);

        var item = currSlot[itemIndex].Item;
        bool itemUsed = item.Use(selectedAnigma);

        if (itemUsed)
        {
            RemoveItem(item, selectedCategory);
            return item;
        }

        return null;
    }

    public void RemoveItem(ItemBase item, int selectedCategory)
    {
        var currSlot = GetSlotsByCategory(selectedCategory);

        var itemSlot = currSlot.First(slot => slot.Item == item);
        itemSlot.Count--;

        if (itemSlot.Count <= 0)
        {
            currSlot.Remove(itemSlot);
        }

        OnUpdated?.Invoke();
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase Item => item;
    public int Count 
    { 
        get { return count; }
        set { count = value; }
    }
}