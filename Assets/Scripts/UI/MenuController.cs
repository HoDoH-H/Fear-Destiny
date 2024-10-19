using DG.Tweening;
using System;
using System.Collections;
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

    bool isAnimating;
    Vector3 originalPosition;

    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        originalPosition = menu.transform.localPosition;
        var t = menu.GetComponent<RectTransform>();
        //menu.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y + t.rect.height * 1.25f);
        menu.transform.localPosition = new Vector3(originalPosition.x + t.rect.width * 1.25f, originalPosition.y);
    }

    public IEnumerator OpenMenu(bool needAnim = false)
    {
        isAnimating = true;

        menu.SetActive(true);
        
        if (needAnim)
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                menuItems[i].color = GlobalSettings.Instance.BaseInvColor;
            }

            var t = menu.GetComponent<RectTransform>();
            //menu.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y + t.rect.height * 1.25f);
            //yield return menu.transform.DOLocalMoveY(originalPosition.y, 0.3f).WaitForCompletion();
            menu.transform.localPosition = new Vector3(originalPosition.x + t.rect.width * 1.25f, originalPosition.y);
            yield return menu.transform.DOLocalMoveX(originalPosition.x, 0.3f).WaitForCompletion();
            yield return new WaitForSeconds(0.2f);
        }

        UpdateItemSelection();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isAnimating = false;
    }

    public IEnumerator CloseMenu(bool needAnim = false)
    {
        isAnimating = true;

        if (needAnim)
        {
            var t = menu.GetComponent<RectTransform>();
            //yield return menu.transform.DOLocalMoveY(originalPosition.y + t.rect.height * 1.25f, 0.3f).WaitForCompletion();
            yield return menu.transform.DOLocalMoveX(originalPosition.x + t.rect.width * 1.25f, 0.3f).WaitForCompletion();
        }

        menu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isAnimating = false;
    }

    public void HandleUpdate()
    {
        if (!isAnimating)
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
                StartCoroutine(CloseMenu());
            }
            else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Back))
            {
                OnBack?.Invoke();
                StartCoroutine(CloseMenu(true));
            }
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
