using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecipeMenuController : MonoBehaviour
{
    [SerializeField] public List<CanvasGroup> allPages = new List<CanvasGroup>();
    [SerializeField, TextArea] public string textToReadOnOpen;

    private int m_currentPage = 0;

    // Start is called before the first frame update
    void Start()
    {
        string textToRead = textToReadOnOpen;
        for (int pageIdx = 0; pageIdx < allPages.Count; pageIdx++)
        {
            if (pageIdx == 0)
            {
                GameObject pageObj = allPages[pageIdx].gameObject;
                pageObj.SetActive(true);
                RecipePageComponent page = pageObj.GetComponent<RecipePageComponent>();
                textToReadOnOpen += page.pageInformationToRead;
            }
            else
            {
                allPages[pageIdx].gameObject.SetActive(false);
            }
        }

        ScreenReader.StaticReadText(textToRead);
    }

    public void OnPressPageLeft(InputAction.CallbackContext context)
    {
        if (!context.performed || m_currentPage == 0) { return; }

        allPages[m_currentPage].gameObject.SetActive(false);
        m_currentPage--;
        GameObject pageObj = allPages[m_currentPage].gameObject;
        pageObj.SetActive(true);
        RecipePageComponent page = pageObj.GetComponent<RecipePageComponent>();

        ScreenReader.StaticReadText(page.pageInformationToRead);
    }

    public void OnPressPageRight(InputAction.CallbackContext context)
    {
        if (!context.performed || m_currentPage == allPages.Count - 1) { return; }

        allPages[m_currentPage].gameObject.SetActive(false);
        m_currentPage++;
        GameObject pageObj = allPages[m_currentPage].gameObject;
        pageObj.SetActive(true);
        RecipePageComponent page = pageObj.GetComponent<RecipePageComponent>();

        ScreenReader.StaticReadText(page.pageInformationToRead);
    }
}
