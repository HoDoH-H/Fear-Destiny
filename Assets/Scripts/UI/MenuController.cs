using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menu;

    public event Action<int> OnMenuSelected;
    public event Action OnBack;

    List<TextMeshProUGUI> menuItems;

    int selectedItem = 0;

    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        UpdateItemSelection();
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;
        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Down))
            selectedItem++;
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Up))
            selectedItem--;

        selectedItem = GameController.Instance.RotateSelection(selectedItem, menuItems.Count - 1);

        if (prevSelection != selectedItem)
            UpdateItemSelection();

        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter))
        {
            OnMenuSelected?.Invoke(selectedItem);
            CloseMenu();
        }
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Back))
        {
            OnBack?.Invoke();
            CloseMenu();
        }
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; i++) 
        {
            if (i == selectedItem)
            {
                menuItems[i].color = GlobalSettings.Instance.HighlightedColor;
            }
            else
            {
                menuItems[i].color = GlobalSettings.Instance.BaseInvColor;
            }
        }
    }
}
