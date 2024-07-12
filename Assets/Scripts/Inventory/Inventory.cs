using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { Items, Recovery, Rings, Memories}
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
        ItemCategory.Items.ToString(), ItemCategory.Recovery.ToString(), ItemCategory.Rings.ToString(), ItemCategory.Memories.ToString()
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currSlot = GetSlotsByCategory(categoryIndex);
        return currSlot[itemIndex].Item;
    }

    public ItemBase UseItem(int itemIndex, Anigma selectedAnigma, int selectedCategory)
    {
        var item = GetItem(itemIndex, selectedCategory);
        bool itemUsed = item.Use(selectedAnigma);

        if (itemUsed)
        {
            if (!item.IsReusable)
                RemoveItem(item, selectedCategory);

            return item;
        }

        return null;
    }

    
    public void AddItem(ItemBase item, int count=1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }

        OnUpdated?.Invoke();
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

    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem)
            return ItemCategory.Recovery;
        else if (item is RingItem)
            return ItemCategory.Rings;
        else
            return ItemCategory.Memories;
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

    public ItemBase Item
    {
        get => item;
        set => item = value;
    }
    public int Count 
    { 
        get { return count; }
        set { count = value; }
    }
}