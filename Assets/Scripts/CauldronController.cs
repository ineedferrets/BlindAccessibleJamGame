using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CauldronController : MonoBehaviour
{
    public static CauldronController instance;

    public List<Image> ingredientMenuImages = new List<Image>();
    public List<Recipe> recipes = new List<Recipe>();
    [TextArea] public string textToReadOnLaunch = "";

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
        UpdateVisuals();

        ScreenReader.StaticReadText(textToReadOnLaunch);
    }

    public bool TryAddIngredient(Item item, out AddIngredientOutcome outcome)
    {
        if (ingredients.Contains(item))
        {
            ingredients.Remove(item);
            outcome = AddIngredientOutcome.AlreadyContainsIngredient;
            UpdateVisuals();
            return false;
        }
        else if (ingredients.Count == 3)
        {
            outcome = AddIngredientOutcome.IngredientsFull;
            return false;
        }

        ingredients.Add(item);
        UpdateVisuals();

        outcome = AddIngredientOutcome.Success;
        return true;
    }

    public void AttemptMix(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Recipe successfulRecipe = null;
        foreach (Recipe recipe in recipes)
        {
            bool bIngredientsMakeRecipe = ingredients.Count > 0 && ingredients.Count == recipe.ingredients.Count;
            if (!bIngredientsMakeRecipe)
            {
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
            Debug.Log("Ingredients do not match recipe.");
            // What do we do if it's unsuccessful?
            return;
        }

        InventoryManager inventoryManager = InventoryManager.Instance;
        foreach (Item item in ingredients)
        {
            inventoryManager.Remove(item);
        }

        ingredients.Clear();
        foreach (Image image in ingredientMenuImages)
        {
            image.sprite = null;
        }

        Debug.Log("Recipe successful");
        inventoryManager.TryAndAdd(successfulRecipe.finalItem);
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
}
