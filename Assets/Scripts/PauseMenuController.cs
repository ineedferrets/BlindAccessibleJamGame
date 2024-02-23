using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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

    public void InputTogglePauseMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (exitConfirmationMenu.gameObject && exitConfirmationMenu.gameObject.activeSelf)
        {
            ToggleConfirmationOfExit();
        }

        bool bIsPauseMenuOpen = pauseMenuGroup.gameObject.activeSelf;

        foreach (CanvasGroup canvas in allCanvasGroups)
        {
            canvas.interactable = bIsPauseMenuOpen;
        }

        pauseMenuGroup.gameObject.SetActive(!bIsPauseMenuOpen);
    }

    public void ToggleConfirmationOfExit()
    {
        bool bIsConfirmationMenuOpen = exitConfirmationMenu.gameObject.activeSelf;

        pauseMenuGroup.interactable = bIsConfirmationMenuOpen;
        foreach (CanvasGroup canvas in ignoreGroups)
        {
            canvas.interactable = bIsConfirmationMenuOpen;
        }

        exitConfirmationMenu.gameObject.SetActive(!bIsConfirmationMenuOpen);
        exitConfirmationMenu.interactable = !bIsConfirmationMenuOpen;
    }

    public void ExitToMenu()
    {
        SceneChanger sceneChanger = SceneChanger.Instance;
        if (sceneChanger == null) { return; }
        sceneChanger.ChangeScene("Menu");
    }
}
