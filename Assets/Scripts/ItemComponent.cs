using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    [SerializeField] public Item itemAsset;

    [SerializeField] public TextMeshPro itemPickUpText;

    private void Start()
    {
        if (itemPickUpText && itemAsset)
        {
            itemPickUpText.text = itemPickUpText.text.Replace("pick up", "pick up " + itemAsset.name);
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

        if (bWasSuccessful)
        {
            QuestManager questManager = QuestManager.Instance;
            if (questManager == null) { return; }
            questManager.UpdateObjectivesInformation();
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
