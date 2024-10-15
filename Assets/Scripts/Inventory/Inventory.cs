using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { Items, Recovery, Rings, Memories}
public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemSlot> itemsSlots;
    [SerializeField] List<ItemSlot> ringSlots;
    [SerializeField] List<ItemSlot> memorySlots;

    List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>> { itemsSlots, ringSlots, memorySlots};
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

    public ItemBase UseItem(int itemIndex, Battler selectedAnigma, int selectedCategory)
    {
        var item = GetItem(itemIndex, selectedCategory);
        bool itemUsed = item.Use(selectedAnigma);

        if (itemUsed)
        {
            if (!item.IsReusable)
                RemoveItem(item);

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
            itemSlot.Count = Mathf.Clamp(itemSlot.Count, 0, 999);
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

    public void RemoveItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currSlot = GetSlotsByCategory(category);

        var itemSlot = currSlot.First(slot => slot.Item == item);
        itemSlot.Count--;

        if (itemSlot.Count <= 0)
        {
            currSlot.Remove(itemSlot);
        }

        OnUpdated?.Invoke();
    }

    public bool HasItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currSlot = GetSlotsByCategory(category);

        return currSlot.Exists(slot => slot.Item == item);
    }

    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem || item is MorlenisItem)
            return ItemCategory.Items;
        else if (item is RingItem)
            return ItemCategory.Rings;
        else
            return ItemCategory.Memories;
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData() 
        { 
            items = itemsSlots.Select(i => i.GetSaveData()).ToList(),
            rings = ringSlots.Select(i => i.GetSaveData()).ToList(),
            memories = memorySlots.Select(i => i.GetSaveData()).ToList(),
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;

        itemsSlots = saveData.items.Select(i => new ItemSlot(i)).ToList();
        ringSlots = saveData.rings.Select(i => new ItemSlot(i)).ToList();
        memorySlots = saveData.memories.Select(i => new ItemSlot(i)).ToList();

        allSlots = new List<List<ItemSlot>> { itemsSlots, ringSlots, memorySlots };

        OnUpdated?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemSlot()
    {

    }

    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemDB.GetObjectByName(saveData.name);
        count = saveData.count;
    }

    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            name = item.name,
            count = count
        };

        return saveData;
    }

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

[Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items;
    public List<ItemSaveData> recoveries;
    public List<ItemSaveData> rings;
    public List<ItemSaveData> memories;
}