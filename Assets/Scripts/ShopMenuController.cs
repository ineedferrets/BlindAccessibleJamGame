using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenuController : MonoBehaviour
{
    [SerializeField] CanvasGroup shopMenuGroup;
    [SerializeField] DialogueAsset dialogueAsset;

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
}
