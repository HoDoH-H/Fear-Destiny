using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{
    static Dictionary<string, ItemBase> items;

    public static void Init()
    {
        items = new Dictionary<string, ItemBase>();

        var itemArray = Resources.LoadAll<ItemBase>("");
        foreach (var item in itemArray)
        {
            if (items.ContainsKey(item.name))
            {
                Debug.LogError($"There are two items with the same name {item.Name}.");
                continue;
            }
            items[item.name] = item;
        }
    }

    public static ItemBase GetItemByName(string name)
    {
        if (!items.ContainsKey(name))
        {
            Debug.LogError($"Item with name {name} not found in the database.");
            return null;
        }

        return items[name];
    }
}