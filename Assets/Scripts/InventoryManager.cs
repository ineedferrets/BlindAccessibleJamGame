using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> items = new List<Item>();

    public CanvasGroup inventoryUI;

    public Transform ItemContent;
    public GameObject InventoryItem;

    private void Awake()
    {
        if (InventoryManager.Instance == null)
        {
            InventoryManager.Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public bool TryAndAdd(Item item)
    {
        if (items.Contains(item))
        {
            return false;
        }
        if (items.Count == 3)
        {
            return false;
        }

        items.Add(item);

        if (inventoryUI.gameObject.activeSelf)
        {
            ListItems();
        }

        return true;
    }

    public void Remove(Item item)
    {
        items.Remove(item);
        if (inventoryUI.gameObject.activeSelf)
        {
            ListItems();
        }
    }

    public bool InventoryContains(Item item) { return items.Contains(item); }

    public void ToggleInventoryWithoutReturn()
    {
        ToggleInventory();
    }

    public bool ToggleInventory()
    {
        if (inventoryUI == null) { return false;  }

        bool bUIIsActive = inventoryUI.gameObject.activeSelf;

        inventoryUI.gameObject.SetActive(!bUIIsActive);

        if (!bUIIsActive)
            ListItems();

        Selectable selectable = inventoryUI.gameObject.GetComponentInChildren<Selectable>();
        if (selectable != null)
        {
            selectable.Select();
        }

        return !bUIIsActive;
    }

    public void ListItems()
    {
        foreach (Transform Item in ItemContent)
        {
            Destroy(Item.gameObject);
        }

        Button prevButton = null;
        Button secondPrevButton = null;

        foreach (Item item in items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);

            Button newButton = obj.GetComponent<Button>();
            if (newButton && prevButton)
            {
                Navigation customNav = new Navigation();
                customNav.mode = Navigation.Mode.Explicit;
                customNav.selectOnUp = secondPrevButton;
                customNav.selectOnDown = newButton;
                prevButton.navigation = customNav;
            }

            if (item == items.Last())
            {
                Navigation customNav = new Navigation();
                customNav.mode = Navigation.Mode.Explicit;
                customNav.selectOnUp = prevButton;
                newButton.navigation = customNav;
            }

            secondPrevButton = prevButton;
            prevButton = newButton;

            var itemName = obj.transform.Find("ItemName").GetComponent<TMP_Text>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var itemComponent = obj.GetComponent<InventoryItemComponent>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;
            itemComponent.itemAsset = item;
        }
    }
}
