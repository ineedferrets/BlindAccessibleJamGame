using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField, Header("Enable/Disable Menus")] CanvasGroup pauseMenuGroup;
    [SerializeField] CanvasGroup[] ignoreGroups;

    [SerializeField, Header("Sub Menus")] CanvasGroup exitConfirmationMenu;

    private List<CanvasGroup> allCanvasGroups;

    private void Start()
    {
        allCanvasGroups = new List<CanvasGroup>(gameObject.GetComponentsInChildren<CanvasGroup>());

        foreach (CanvasGroup ignore in ignoreGroups)
        {
            allCanvasGroups.Remove(ignore);
        }

        allCanvasGroups.Remove(pauseMenuGroup);
    }

    public void TogglePauseMenu()
    {
        bool bIsPauseMenuOpen = pauseMenuGroup.gameObject.activeSelf;

        foreach (CanvasGroup canvas in allCanvasGroups)
        {
            canvas.interactable = bIsPauseMenuOpen;
        }

        pauseMenuGroup.gameObject.SetActive(!bIsPauseMenuOpen);
    }

    public void ToggleConfirmationOfExit()
    {
        bool bIsMenuOpen = exitConfirmationMenu.gameObject.activeSelf;

        pauseMenuGroup.interactable = bIsMenuOpen;
        foreach (CanvasGroup canvas in ignoreGroups)
        {
            canvas.interactable = bIsMenuOpen;
        }

        exitConfirmationMenu.gameObject.SetActive(!bIsMenuOpen);
        exitConfirmationMenu.interactable = !bIsMenuOpen;
    }

    public void ExitToMenu()
    {
        SceneChanger sceneChanger = SceneChanger.Instance;
        if (sceneChanger == null) { return; }
        sceneChanger.ChangeScene("Menu");
    }
}
