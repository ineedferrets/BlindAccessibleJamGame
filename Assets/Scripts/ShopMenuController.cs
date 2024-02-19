using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuController : MonoBehaviour
{
    [SerializeField, Header("Shop Menu")] CanvasGroup shopMenuGroup;

    [SerializeField, Header("Inventory")] CanvasGroup inventoryMenuGroup;

    [SerializeField, Header("Dialogue")] DialogueAsset dialogueAsset;

    private void Start()
    {
        DialogueBoxController.OnDialogueStarted += HideMenu;
        DialogueBoxController.OnDialogueEnded += ShowMenu;
    }

    private void HideMenu()
    {
        shopMenuGroup.gameObject.SetActive(false);
    }

    private void ShowMenu()
    {
        shopMenuGroup.gameObject.SetActive(true);
    }

    public void TalkToCustomer()
    {
        DialogueBoxController dialogueBoxController = DialogueBoxController.instance;
        if (dialogueBoxController == null) { return; }

        dialogueBoxController.StartDialogue(dialogueAsset, 0);
    }

    public void GoOutside()
    {
        SceneChanger sceneChanger = SceneChanger.Instance;
        if (sceneChanger == null) { return; }

        sceneChanger.ChangeScene("Outdoor graveyard");
    }

    public void ToggleInventory()
    {
        if (inventoryMenuGroup == null) { return; }

        bool bIsActive = inventoryMenuGroup.gameObject.activeSelf;

        shopMenuGroup.interactable = bIsActive;
        inventoryMenuGroup.gameObject.SetActive(!bIsActive);

        if (!bIsActive)
        {
            InventoryManager inventoryManager = InventoryManager.Instance;
            if (inventoryManager == null) { return; }
            inventoryManager.ListItems();
        }
    }
}
