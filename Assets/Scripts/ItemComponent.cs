using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    [SerializeField] public Item itemAsset;

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
