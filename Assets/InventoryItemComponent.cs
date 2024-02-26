using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemComponent : MonoBehaviour
{
    public Item itemAsset;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        if (button == null) { return; }

        button.onClick.AddListener(ClickItem);
    }

    public void ClickItem()
    {
        CauldronController cauldron = CauldronController.instance;
        if (cauldron == null || itemAsset == null) { return; }

        CauldronController.AddIngredientOutcome outcome = CauldronController.AddIngredientOutcome.None;
        bool bWasAdded = cauldron.TryAddIngredient(itemAsset, out outcome);

        if (bWasAdded)
        {
            ScreenReader.StaticReadText(itemAsset.name + " was added to the cauldron.");
        }
        else if (outcome == CauldronController.AddIngredientOutcome.AlreadyContainsIngredient)
        {
            ScreenReader.StaticReadText(itemAsset.name + " was already in the pot and has been removed.");
        }
        else if (outcome == CauldronController.AddIngredientOutcome.IngredientsFull)
        {
            ScreenReader.StaticReadText("The pot is already full.");
        }
    }
}
