using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CauldronController : MonoBehaviour
{
    public static CauldronController instance;

    public List<Image> ingredientMenuImages = new List<Image>();

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
