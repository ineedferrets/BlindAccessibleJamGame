using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    static public PauseMenuController Instance { get; private set; }

    [SerializeField, Header("Enable/Disable Menus")] CanvasGroup pauseMenuGroup;
    [SerializeField] CanvasGroup[] ignoreGroups;

    [SerializeField, Header("Sub Menus")] CanvasGroup exitConfirmationMenu;

    private List<CanvasGroup> allCanvasGroups;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        allCanvasGroups = new List<CanvasGroup>(gameObject.GetComponentsInChildren<CanvasGroup>());

        foreach (CanvasGroup ignore in ignoreGroups)
        {
            allCanvasGroups.Remove(ignore);
        }

        allCanvasGroups.Remove(pauseMenuGroup);
    }

    public void SetPauseMenu(bool bOpenMenu)
    {
        if (!bOpenMenu && exitConfirmationMenu.gameObject && exitConfirmationMenu.gameObject.activeSelf)
        {
            SetConfirmationOfExit(true);
        }

        foreach (CanvasGroup canvas in allCanvasGroups)
        {
            canvas.interactable = !bOpenMenu;
        }

        pauseMenuGroup.gameObject.SetActive(bOpenMenu);
        pauseMenuGroup.interactable = bOpenMenu;
    }

    public void SetConfirmationOfExit(bool bOpenMenu)
    {
        pauseMenuGroup.interactable = !bOpenMenu;
        foreach (CanvasGroup canvas in ignoreGroups)
        {
            canvas.interactable = !bOpenMenu;
        }

        exitConfirmationMenu.gameObject.SetActive(bOpenMenu);
        exitConfirmationMenu.interactable = bOpenMenu;

        if (bOpenMenu)
        {
            Transform yesTransform = exitConfirmationMenu.transform.Find("Yes");
            Selectable yesButton = yesTransform ? yesTransform.GetComponentInChildren<Selectable>() : null;

            if (yesButton)
            {
                yesButton.Select();
            }
        }
        else
        {
            Transform returnTransform = pauseMenuGroup.transform.Find("ReturnToGame");
            Selectable returnButton = returnTransform ? returnTransform.GetComponentInChildren<Selectable>() : null;

            if (returnButton)
            {
                returnButton.Select();
            }
        }
    }

    public void ExitToMenu()
    {
        SceneChanger sceneChanger = SceneChanger.Instance;
        if (sceneChanger == null) { return; }
        sceneChanger.ChangeScene("Menu");
    }
}
