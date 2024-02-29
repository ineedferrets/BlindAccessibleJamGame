using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    [SerializeField] public Item itemAsset;

    [SerializeField] public TextMeshPro itemPickUpText;

    [SerializeField] public FMODUnity.EventReference pickUpItemSoundReference;
    private FMOD.Studio.EventInstance pickUpItemInstance;

    private void Start()
    {
        if (itemPickUpText && itemAsset)
        {
            itemPickUpText.text = itemPickUpText.text.Replace("pick up", "pick up " + itemAsset.name);
        }

        pickUpItemInstance = FMODUnity.RuntimeManager.CreateInstance(pickUpItemSoundReference);
    }

    public void OnPickUp(bool bDestroyItem = true)
    {
        InventoryManager inventoryManager = InventoryManager.Instance;

        if (inventoryManager == null)
            return;

        bool bWasSuccessful = inventoryManager.TryAndAdd(itemAsset);

        if (bWasSuccessful)
        {
            QuestManager questManager = QuestManager.Instance;
            if (questManager == null) { return; }
            questManager.UpdateObjectivesInformation();

            pickUpItemInstance.start();
        }

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
