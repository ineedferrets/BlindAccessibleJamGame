using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    [SerializeField] public Item itemAsset;

    [SerializeField] public TextMeshPro itemPickUpText;

    private void Awake()
    {
        if (itemPickUpText && itemAsset)
        {
            itemPickUpText.text = "Press (controls) to pick up " + itemAsset.name + ".";
        }
    }

    public void OnPickUp(bool bDestroyItem = true)
    {
        InventoryManager inventoryManager = InventoryManager.Instance;

        if (inventoryManager == null)
            return;

        bool bWasSuccessful = inventoryManager.TryAndAdd(itemAsset);

        if (bWasSuccessful && bDestroyItem)
        {
            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (itemAsset && spriteRenderer)
        {
            spriteRenderer.sprite = itemAsset.icon;
        }
    }
}
