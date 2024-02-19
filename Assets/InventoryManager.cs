using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> items = new List<Item>();

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

    public void Add(Item item)
    {
        items.Add(item);
    }

    public void Remove(Item item)
    {
        items.Remove(item);
    }

    public void ListItems()
    {
        foreach (Item item in items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            Text itemName = obj.transform.Find("Item/ItemName").GetComponent<Text>();
            Image itemIcon = obj.transform.Find("Item/ItemIcon").GetComponent<Image>();

            Debug.Log("Actual Item Name: " + item.name);
            Debug.Log("Actual Item Sprite: " + item.icon.ToString());
            Debug.Log("Found Item Text: " + itemName);
            Debug.Log("Found Item Sprite: " + itemIcon.ToString());

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;
        }
    }
}
