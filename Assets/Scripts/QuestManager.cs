using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestManager : MonoBehaviour
{
    static public QuestManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("Quests")]
    public QuestAsset CurrentQuest;
    public QuestAsset InitialQuest;
    public bool bStartInitialQuestInDialogue;

    enum QuestState
    {
        NotStarted,
        Ongoing,
        ReadyToHandIn
    }

    private QuestState _state;

    private void Start()
    {
        if (bStartInitialQuestInDialogue)
        {
            DialogueBoxController dialogueBoxController = DialogueBoxController.instance;

            if (dialogueBoxController == null || InitialQuest == null) { return; }

            CurrentQuest = InitialQuest;
            RunCurrentDialogue();
        }
    }

    public void RunCurrentDialogue()
    {
        DialogueBoxController dialogueBoxController = DialogueBoxController.instance;

        if (dialogueBoxController == null) { return; }

        UpdateQuest();

        DialogueAsset dialogueAsset = CurrentQuest.StartQuestDialogue;
        switch(_state)
        {
            case QuestState.NotStarted:
                dialogueAsset = CurrentQuest.StartQuestDialogue;
                StartQuest();
                break;
            case QuestState.Ongoing:
                dialogueAsset = CurrentQuest.MidQuestDialogue;
                break;
            case QuestState.ReadyToHandIn:
                dialogueAsset = CurrentQuest.EndQuestDialogue;
                EndQuest();
                break;
        }

        dialogueBoxController.StartDialogue(dialogueAsset, 0);
    }

    private void StartQuest()
    {
        InventoryManager inventoryManager = InventoryManager.Instance;
        if (inventoryManager == null) { return; }

        foreach (Item item in CurrentQuest.GivenItems)
        {
            inventoryManager.TryAndAdd(item);
        }

        _state = QuestState.Ongoing;
    }

    private void UpdateQuest()
    {
        InventoryManager inventoryManager = InventoryManager.Instance;
        if (inventoryManager == null) { return; }

        if (CurrentQuest.RequiredItems.Count == 0) { return; }

        bool bAllItemsGot = true;
        foreach (Item item in CurrentQuest.RequiredItems)
        {
            bAllItemsGot = bAllItemsGot && inventoryManager.InventoryContains(item);
        }

        if (bAllItemsGot)
        {
            _state = QuestState.ReadyToHandIn;
        }
    }

    private void EndQuest()
    {

    }
}
