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
    public List<QuestAsset> AllQuests;
    public bool bStartInitialQuestInDialogue;

    [Header("Ghost")]
    public SpriteRenderer spriteRendererForGhost;

    private int currentQuestIdx = 0;

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

            if (dialogueBoxController == null || AllQuests.Count == 0) { return; }

            RunCurrentDialogue();
        }
    }

    public void RunCurrentDialogue()
    {
        DialogueBoxController dialogueBoxController = DialogueBoxController.instance;

        if (dialogueBoxController == null || AllQuests.Count == 0) { return; }

        UpdateQuest();

        QuestAsset currentQuest = AllQuests[currentQuestIdx];

        DialogueAsset dialogueAsset = currentQuest.StartQuestDialogue;
        switch(_state)
        {
            case QuestState.NotStarted:
                dialogueAsset = currentQuest.StartQuestDialogue;
                DialogueBoxController.OnDialogueEnded += StartQuest;
                break;
            case QuestState.Ongoing:
                dialogueAsset = currentQuest.MidQuestDialogue;
                break;
            case QuestState.ReadyToHandIn:
                dialogueAsset = currentQuest.EndQuestDialogue;
                PreEndQuest();
                DialogueBoxController.OnDialogueEnded += EndQuest;
                break;
        }

        dialogueBoxController.StartDialogue(dialogueAsset, 0);
    }

    private void StartQuest()
    {
        DialogueBoxController.OnDialogueEnded -= StartQuest;

        InventoryManager inventoryManager = InventoryManager.Instance;
        if (inventoryManager == null) { return; }

        QuestAsset currentQuest = AllQuests[currentQuestIdx];
        foreach (Item item in currentQuest.GivenItems)
        {
            inventoryManager.TryAndAdd(item);
        }

        _state = QuestState.Ongoing;
    }

    private void UpdateQuest()
    {
        InventoryManager inventoryManager = InventoryManager.Instance;
        if (inventoryManager == null) { return; }

        QuestAsset currentQuest = AllQuests[currentQuestIdx];
        if (currentQuest.RequiredItems.Count == 0) { return; }

        bool bAllItemsGot = true;
        foreach (Item item in currentQuest.RequiredItems)
        {
            bAllItemsGot = bAllItemsGot && inventoryManager.InventoryContains(item);
        }

        if (bAllItemsGot)
        {
            _state = QuestState.ReadyToHandIn;
        }
    }

    private void PreEndQuest()
    {
        InventoryManager inventoryManager = InventoryManager.Instance;
        if (inventoryManager == null) { return; }

        QuestAsset currentQuest = AllQuests[currentQuestIdx];
        if (currentQuest.RequiredItems.Count == 0) { return; }

        foreach (Item item in currentQuest.RequiredItems)
        {
            inventoryManager.Remove(item);
        }
    }

    private void EndQuest()
    {
        DialogueBoxController.OnDialogueEnded -= EndQuest;

        currentQuestIdx++;
        if (currentQuestIdx == AllQuests.Count)
        {
            // Do things here to end the game.
            return;
        }

        _state = QuestState.NotStarted;

        spriteRendererForGhost.sprite = AllQuests[currentQuestIdx].GhostSprite;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("ItemSpawner");
        foreach (GameObject obj in gameObjects)
        {
            ItemSpawner itemSpawner = obj.GetComponent<ItemSpawner>();
            if (itemSpawner != null)
            {
                itemSpawner.SpawnItem();
            }
        }
    }
}
