using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecipeMenuController : MonoBehaviour
{
    [SerializeField] public CanvasGroup recipeMenuGroup;
    [SerializeField] public List<CanvasGroup> allPages = new List<CanvasGroup>();
    [SerializeField, TextArea] public string textToReadOnOpen;

    private int m_currentPage = 0;

    public void ToggleRecipeMenu(bool openMenu)
    {
        if (recipeMenuGroup == null) { return; }

        recipeMenuGroup.gameObject.SetActive(openMenu);
        if (openMenu)
        {
            SetupRecipe();
        }

        QuestManager questManager = QuestManager.Instance;
        if (questManager == null) { return; }

        // Not good way to check this.
        if (questManager.currentObjective.name.Contains("Recipe"))
            questManager.UpdateObjectivesInformation();
    }

    private void SetupRecipe()
    {
        string textToRead = textToReadOnOpen;
        for (int pageIdx = 0; pageIdx < allPages.Count; pageIdx++)
        {
            if (pageIdx == 0)
            {
                GameObject pageObj = allPages[pageIdx].gameObject;
                pageObj.SetActive(true);
                RecipePageComponent page = pageObj.GetComponent<RecipePageComponent>();
                textToRead += " " + page.pageInformationToRead;
            }
            else
            {
                allPages[pageIdx].gameObject.SetActive(false);
            }
        }

        if (Application.platform != RuntimePlatform.WebGLPlayer)
            ScreenReader.StaticReadText(textToRead);
    }

    public void OnPressPageLeft(InputAction.CallbackContext context)
    {
        if (!context.performed || m_currentPage == 0 || !recipeMenuGroup.gameObject.activeSelf) { return; }

        allPages[m_currentPage].gameObject.SetActive(false);
        m_currentPage--;
        GameObject pageObj = allPages[m_currentPage].gameObject;
        pageObj.SetActive(true);
        RecipePageComponent page = pageObj.GetComponent<RecipePageComponent>();

        if (Application.platform != RuntimePlatform.WebGLPlayer) 
            ScreenReader.StaticReadText(page.pageInformationToRead);
    }

    public void OnPressPageRight(InputAction.CallbackContext context)
    {
        if (!context.performed || m_currentPage == allPages.Count - 1 || !recipeMenuGroup.gameObject.activeSelf) { return; }

        allPages[m_currentPage].gameObject.SetActive(false);
        m_currentPage++;
        GameObject pageObj = allPages[m_currentPage].gameObject;
        pageObj.SetActive(true);
        RecipePageComponent page = pageObj.GetComponent<RecipePageComponent>();

        if (Application.platform != RuntimePlatform.WebGLPlayer) 
            ScreenReader.StaticReadText(page.pageInformationToRead);
    }
}
