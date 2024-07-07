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
        if (Input.GetKeyDown(GlobalSettings.Instance.DownKeys[0]) || Input.GetKeyDown(GlobalSettings.Instance.DownKeys[1]))
        {
            if (selectedItem < menuItems.Count)
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
                selectedItem = menuItems.Count;
            }
        }

        if (prevSelection != selectedItem)
            UpdateItemSelection();

        if (Input.GetKeyDown(GlobalSettings.Instance.EnterKeys[0]) || Input.GetKeyDown(GlobalSettings.Instance.EnterKeys[1]))
        {
            OnMenuSelected?.Invoke(selectedItem);
            CloseMenu();
        }
        else if (Input.GetKeyDown(GlobalSettings.Instance.BackKeys[0]) || Input.GetKeyDown(GlobalSettings.Instance.BackKeys[1]))
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
