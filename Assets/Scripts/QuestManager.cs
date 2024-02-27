using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [Header("UI")]
    public TextMeshProUGUI objectiveBodyText;

    [Header("World")]
    public GameObject ghostObject;
    public GameObject cauldronObject;
    public GameObject recipeObject;

    private GameObject currentObjective;
    private List<GameObject> worldRequiredIngredients = new List<GameObject>();

    private int currentQuestIdx = 0;

    enum QuestState
    {
        NotStarted,
        Ongoing,
        ReadyToHandIn
    }

    private QuestState _state = QuestState.NotStarted;

    private void Start()
    {
        if (bStartInitialQuestInDialogue)
        {
            DialogueBoxController dialogueBoxController = DialogueBoxController.instance;

            if (dialogueBoxController == null || AllQuests.Count == 0) { return; }

            RunCurrentDialogue();
        }

        spriteRendererForGhost.sprite = AllQuests[0].GhostSprite;

        UpdateObjectivesInformation();
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
        if (currentQuest.EndQuestDialogue == null)
        {
            EndQuest();
            return;
        }

        foreach (Item item in currentQuest.GivenItems)
        {
            inventoryManager.TryAndAdd(item);
        }

        _state = QuestState.Ongoing;

        objectiveBodyText.text = "- Go to recipe book to find ingredients.";
        currentObjective = recipeObject;

        // This is really really bad and needs to be entirely rethought after the jam is complete.
        CauldronController cauldron = CauldronController.instance;
        if (cauldron)
        {
            Recipe requiredRecipe = null;
            foreach (Recipe recipe in cauldron.recipes)
            {
                if (currentQuest.RequiredItems.Contains(recipe.finalItem))
                {
                    requiredRecipe = recipe;
                    break;
                }
            }
            if (requiredRecipe == null) { return; }

            List<GameObject> itemSpawners = new List<GameObject>(GameObject.FindGameObjectsWithTag("ItemSpawner"));
            
            foreach (GameObject spawnerObj in itemSpawners)
            {
                ItemSpawner itemSpawner = spawnerObj.GetComponent<ItemSpawner>();
                if (!itemSpawner) continue;

                if (requiredRecipe.ingredients.Contains(itemSpawner.itemToSpawn))
                {
                    itemSpawner.SpawnItem();
                }
            }
        }
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
            SceneChanger sceneChanger = SceneChanger.Instance;
            if (sceneChanger)
            {
                sceneChanger.ChangeScene("Menu");
            }
            return;
        }

        _state = QuestState.NotStarted;

        spriteRendererForGhost.sprite = AllQuests[currentQuestIdx].GhostSprite;

        UpdateObjectivesInformation();
    }

    public void UpdateObjectivesInformation()
    {
        if (_state == QuestState.NotStarted)
        {
            objectiveBodyText.text = "- Speak to the ghost in the cemetery.";
            currentObjective = ghostObject;
            return;
        }
        
        if (_state == QuestState.Ongoing)
        {
            InventoryManager inventoryManager = InventoryManager.Instance;
            QuestAsset currentQuest = AllQuests[currentQuestIdx];
            if (inventoryManager == null || currentQuest == null) { return; }

            List<Item> requiredItems = currentQuest.RequiredItems;

            // First check if player has required items.
            bool bPlayerHasRequiredItems = inventoryManager.items.Count > 0;
            foreach (var item in requiredItems)
            {
                bPlayerHasRequiredItems &= inventoryManager.items.Contains(item);
            }

            if (bPlayerHasRequiredItems)
            {
                objectiveBodyText.text = "- Speak to the ghost to hand in the potion.";
                currentObjective = ghostObject;
                return;
            }

            // Then check if the player has the ingredients for the potion.
            CauldronController cauldronController = CauldronController.instance;
            if (cauldronController == null) { return; }

            // Get all items from recipes required for quest
            List<Item> missingItems = new List<Item>();
            foreach (var recipe in cauldronController.recipes)
            {
                if (requiredItems.Contains(recipe.finalItem))
                {
                    missingItems.AddRange(recipe.ingredients);
                }
            }

            // Remove inventory items from missing items list
            foreach (var item in inventoryManager.items)
            {
                missingItems.Remove(item);
            }

            if (missingItems.Count == 0)
            {
                objectiveBodyText.text = "- Go to cauldron to combine ingredients.";
                currentObjective = cauldronObject;
            }
            else
            {
                string objectiveString = "";
                foreach (var item in missingItems)
                {
                    objectiveString += "- Find and collect " + item.name + "\n";
                }
                objectiveBodyText.text = objectiveString;
                
                // Find the spawner for the first item.
                List<GameObject> itemSpawnerObjs = new List<GameObject>(GameObject.FindGameObjectsWithTag("ItemSpawner"));
                foreach (var itemSpawnerObj in itemSpawnerObjs)
                {
                    ItemSpawner itemSpawner = itemSpawnerObj.GetComponent<ItemSpawner>();
                    if (itemSpawner == null) { continue; }

                    if (itemSpawner.itemToSpawn == missingItems[0])
                    {
                        currentObjective = itemSpawnerObj;
                        break;
                    }
                }
            }
        }
    }
}
