using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [Header("NPCs")]
    public SpriteRenderer spriteRendererForGhost;
    public SpriteRenderer spriteRendererForHades;

    [Header("UI")]
    public TextMeshProUGUI objectiveBodyText;

    [Header("World")]
    public GameObject ghostObject;
    public GameObject cauldronObject;
    public GameObject recipeObject;
    public GameObject hadesObject;

    [Header("Ending")]
    public DialogueAsset finalDialogue;

    public GameObject currentObjective { get; private set; }
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

        // This is really really bad and needs to be entirely rethought after the jam is complete.
        CauldronController cauldron = CauldronController.instance;
        if (cauldron == null) { return; }
        
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

        // Update objective.
        string objectiveDescription = "Go to recipe book at home to find the ingredients for " + requiredRecipe.finalItem.name + ".";
        SetObjective(new List<string> { objectiveDescription }, recipeObject);

        List <GameObject> itemSpawners = new List<GameObject>(GameObject.FindGameObjectsWithTag("ItemSpawner"));
            
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
            if (finalDialogue != null)
            {
                StartEnding();
                return;
            }
            else
            {
                EndGame();
                return;
            }
        }

        _state = QuestState.NotStarted;

        QuestAsset questAsset = AllQuests[currentQuestIdx];
        if (questAsset == null) { return; }

        if (questAsset.isHades)
        {
            spriteRendererForHades.sprite = AllQuests[currentQuestIdx].GhostSprite;
        }

        spriteRendererForGhost.sprite = AllQuests[currentQuestIdx].GhostSprite;

        UpdateObjectivesInformation();
    }

    public void UpdateObjectivesInformation()
    {
        List<string> objectives = new List<string>();
        if (_state == QuestState.NotStarted)
        {
            QuestAsset currentQuest = AllQuests[currentQuestIdx];
            if (currentQuest == null) { return; }

            string questText = currentQuest.ObjectiveStartOverride;
            bool bGhostIsHades = currentQuest.isHades;

            print(questText);

            if (bGhostIsHades)
            {
                if (questText == string.Empty)
                {
                    questText = "- Speak to Hades in the garden";
                }
                hadesObject.SetActive(true);
                ghostObject.SetActive(false);
                currentObjective = hadesObject;
                spriteRendererForHades.sprite = currentQuest.GhostSprite;
            }
            else
            {
                if (questText == string.Empty)
                {
                    questText = "Speak to the figure in the cemetery.";
                }
                hadesObject.SetActive(false);
                ghostObject.SetActive(true);
                currentObjective = ghostObject;
                spriteRendererForGhost.sprite = currentQuest.GhostSprite;
            }

            SetObjective(new List<string> { questText }, ghostObject);
            //spriteRendererForGhost.sprite = currentQuest.GhostSprite;
            //return;
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
                objectives.Add("Speak to the ghost in the cemetery to hand in the potion.");
                SetObjective(objectives, ghostObject);
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
                objectives.Add("Go to the cauldron at home to combine ingredients.");
                SetObjective(objectives, cauldronObject);
                return;
            }
            else
            {
                foreach (var item in missingItems)
                {
                    objectives.Add("Find and collect " + item.name + ".");
                }
                
                // Find the spawner for the first item.
                List<GameObject> itemSpawnerObjs = new List<GameObject>(GameObject.FindGameObjectsWithTag("ItemSpawner"));
                GameObject itemObjective;
                foreach (var itemSpawnerObj in itemSpawnerObjs)
                {
                    ItemSpawner itemSpawner = itemSpawnerObj.GetComponent<ItemSpawner>();
                    if (itemSpawner == null) { continue; }

                    if (itemSpawner.itemToSpawn == missingItems[0])
                    {
                        itemObjective = itemSpawnerObj;
                        break;
                    }
                }

                SetObjective(objectives, currentObjective);
            }
        }
    }

    private void SetObjective(List<string> newObjectives, GameObject newObjectiveObject)
    {
        currentObjective = newObjectiveObject;

        string finalReaderString = "Current Objective: ";
        string finalUIString = "";

        finalReaderString += newObjectives[0];
        finalUIString += "- " + newObjectives[0];
        
        if (newObjectives.Count >1)
        {
            for (int objectiveIdx = 1; objectiveIdx < newObjectives.Count; ++objectiveIdx)
            {
                finalReaderString += " and " + newObjectives[objectiveIdx];
                finalUIString += "\n- " + newObjectives[objectiveIdx];
            }
        }

        objectiveBodyText.text = finalUIString;

        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            ScreenReader.StaticReadText(finalReaderString);
        }
    }

    private void StartEnding()
    {
        DialogueBoxController dialogueBoxController = DialogueBoxController.instance;

        if (dialogueBoxController == null) { return; }

        DialogueBoxController.OnBlackoutComplete += RunEndingDialogue;
        dialogueBoxController.StartBlackoutFadeIn();
    }

    private void RunEndingDialogue()
    {
        DialogueBoxController.OnBlackoutComplete -= RunEndingDialogue;

        DialogueBoxController dialogueBoxController = DialogueBoxController.instance;
        if (dialogueBoxController == null) { return; }

        DialogueBoxController.OnDialogueEnded += EndGame;
        dialogueBoxController.StartDialogue(finalDialogue, 0);
    }

    private void EndGame()
    {
        DialogueBoxController.OnDialogueEnded -= EndGame;

        SceneChanger sceneChanger = SceneChanger.Instance;
        if (sceneChanger == null) { return; }

        sceneChanger.ChangeScene("Menu");
    }
}
