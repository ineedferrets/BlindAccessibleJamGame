using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CauldronController : MonoBehaviour
{
    public static CauldronController instance;

    public CanvasGroup cauldronCanvasGroup;
    public List<Image> ingredientMenuImages = new List<Image>();
    public List<Recipe> recipes = new List<Recipe>();

    [Header("SFX")]
    public FMODUnity.EventReference successfulMixRecipeSFXReference;
    private FMOD.Studio.EventInstance successfulMixRecipeSFXInstance;

    [Header("Accessibility")]
    [TextArea] public string textToReadOnLaunch = "";
    [TextArea] public string textToReadOnItemAdded = "(Ingredient) added to cauldron.";
    [TextArea] public string textToReadOnItemRemoved = "(Ingredient) was removed from cauldron.";
    [TextArea] public string textToReadOnCauldronFull = "(Ingredient) was not added as cauldron is full.";
    [TextArea] public string textToReadOnWrongRecipe = "The (Ingredients) in cauldron do not match recipe.";
    [TextArea] public string textToReadOnRightRecipe = "The (Ingredients) in the cauldron produce a (Result).";

    private List<Item> ingredients = new List<Item>();

    public enum AddIngredientOutcome
    {
        None,
        Success,
        AlreadyContainsIngredient,
        IngredientsFull
    }

    private void Awake()
    {
        if (CauldronController.instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        successfulMixRecipeSFXInstance = FMODUnity.RuntimeManager.CreateInstance(successfulMixRecipeSFXReference);
    }

    public void OpenMenu()
    {
        if (cauldronCanvasGroup)
        {
            cauldronCanvasGroup.gameObject.SetActive(true);
        }

        UpdateVisuals();
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            ScreenReader.StaticReadText(textToReadOnLaunch);
    }

    public bool TryAddIngredient(Item item, out AddIngredientOutcome outcome)
    {
        if (ingredients.Contains(item))
        {
            ingredients.Remove(item);
            outcome = AddIngredientOutcome.AlreadyContainsIngredient;
            UpdateVisuals();

            if (Application.platform != RuntimePlatform.WebGLPlayer)
                ScreenReader.StaticReadText(ParseTextForSpeech(textToReadOnItemRemoved, item));

            return false;
        }
        else if (ingredients.Count == 3)
        {
            outcome = AddIngredientOutcome.IngredientsFull;
            if (Application.platform != RuntimePlatform.WebGLPlayer)
                ScreenReader.StaticReadText(ParseTextForSpeech(textToReadOnCauldronFull, item));
            return false;
        }

        ingredients.Add(item);
        UpdateVisuals();
        
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            ScreenReader.StaticReadText(ParseTextForSpeech(textToReadOnItemAdded, item));

        outcome = AddIngredientOutcome.Success;
        return true;
    }

    public void AttemptMix(InputAction.CallbackContext context)
    {
        if (!context.performed || !cauldronCanvasGroup.gameObject || !cauldronCanvasGroup.gameObject.activeSelf)
            return;

        Recipe successfulRecipe = null;
        foreach (Recipe recipe in recipes)
        {
            bool bIngredientsMakeRecipe = ingredients.Count > 0 && ingredients.Count == recipe.ingredients.Count;
            if (!bIngredientsMakeRecipe)
            {
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                    ScreenReader.StaticReadText(ParseTextForSpeech(textToReadOnWrongRecipe));
                
                Debug.Log("Not enough ingredients.");
                // What do we do if mismatch of ingredients?
                return;
            }

            foreach (Item item in ingredients)
            {
                bIngredientsMakeRecipe &= recipe.ingredients.Contains(item);
            }
            if (bIngredientsMakeRecipe)
            {
                successfulRecipe = recipe;
                break;
            }
        }
        
        if (!successfulRecipe)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
                ScreenReader.StaticReadText(ParseTextForSpeech(textToReadOnWrongRecipe));

            Debug.Log("Ingredients do not match recipe.");
            // What do we do if it's unsuccessful?
            return;
        }

        MixRecipe(successfulRecipe);
    }

    private void MixRecipe(Recipe successfulRecipe)
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            ScreenReader.StaticReadText(ParseTextForSpeech(textToReadOnRightRecipe, null, successfulRecipe.finalItem));

        InventoryManager inventoryManager = InventoryManager.Instance;
        foreach (Item item in ingredients)
        {
            inventoryManager.Remove(item);
        }

        ingredients.Clear();
        foreach (Image image in ingredientMenuImages)
        {
            image.sprite = null;
            var textMP = image.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (textMP)
                textMP.text = "";
        }

        inventoryManager.TryAndAdd(successfulRecipe.finalItem);

        successfulMixRecipeSFXInstance.start();

        QuestManager questManager = QuestManager.Instance;
        if (questManager == null) { return; }
        questManager.UpdateObjectivesInformation();
    }

    private void UpdateVisuals()
    {
        for (int itemIdx = 0; itemIdx < ingredientMenuImages.Count; ++itemIdx)
        {
            Image image = ingredientMenuImages[itemIdx];
            TextMeshProUGUI textMP = image.GetComponentInChildren<TextMeshProUGUI>();

            if (itemIdx >= ingredients.Count)
            {
                image.sprite = null;
                textMP.text = "";
                continue;
            }

            Item item = ingredients[itemIdx];
            image.sprite = item.icon;

            if (textMP == null) { continue; }

            textMP.text = item.itemName;
        }
    }

    private string ParseTextForSpeech(string text, Item inItem = null, Item resultItem = null)
    {
        string returnString = text;
        if (inItem)
        {
            returnString = returnString.Replace("(Ingredient)", inItem.name);
        }

        if (text.Contains("(Ingredients)"))
        {
            string listOfIngredientsInPot = "";
            if (ingredients.Count == 0)
            {
                listOfIngredientsInPot = "Nothing";
            }
            else if (ingredients.Count == 1)
            {
                listOfIngredientsInPot = ingredients[0].name;
            }
            else if (ingredients.Count == 2)
            {
                listOfIngredientsInPot = ingredients[0].name + " and " + ingredients[1].itemName;
            }
            else if (ingredients.Count == 3)
            {
                listOfIngredientsInPot = ingredients[0].name + ", " + ingredients[1].itemName + ", and " + ingredients[2].name;
            }

            returnString = returnString.Replace("(Ingredients)", listOfIngredientsInPot);
        }

        if (resultItem)
        {
            returnString = returnString.Replace("(Result)", resultItem.name);
        }

        return returnString;
    }
}
