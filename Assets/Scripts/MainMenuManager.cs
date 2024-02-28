using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Play Game")]
    public string GameSceneName;

    [Header("Navigation")]
    public Selectable startSelectable;

    [Header("Menus")]
    public CanvasGroup optionsMenuGroup;
    public CanvasGroup controlsMenuGroup;
    public CanvasGroup creditsMenuGroup;

    [Header("Accessibility")]
    [TextArea] public string optionsMenuOpenRead;
    [TextArea] public string controlsMenuOpenRead;
    [TextArea] public string creditsMenuOpenRead;

    public void PlayGame()
    {
        SceneChanger sceneChanger = SceneChanger.Instance;
        if (sceneChanger == null)
            SceneManager.LoadSceneAsync(GameSceneName);
        else
            sceneChanger.ChangeScene(GameSceneName);
    }

    private void Start()
    {
        if (startSelectable)
        {
            startSelectable.Select();
        }
    }

    public void OpenOptions()
    {
        OpenMenuGroup(optionsMenuGroup, optionsMenuOpenRead);
    }

    public void OpenControls()
    {
        OpenMenuGroup(controlsMenuGroup, controlsMenuOpenRead);
    }

    public void OpenCredits()
    {
        OpenMenuGroup(creditsMenuGroup, creditsMenuOpenRead);
    }

    private void OpenMenuGroup(CanvasGroup menuGroup, string screenReaderText)
    {
        if (menuGroup == null)
            return;

        menuGroup.gameObject.SetActive(true);
        Selectable selectable = menuGroup.gameObject.GetComponentInChildren<Selectable>();

        if (selectable)
        {
            selectable.Select();
        }

        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            ScreenReader.StaticReadText(screenReaderText);
        }
    }

    public void InputCloseMenu(InputAction.CallbackContext context)
    {
        if (context.performed) { CloseMenu(); }
    }

    public void CloseMenu()
    {
        if (optionsMenuGroup && optionsMenuGroup.gameObject.activeSelf)
        {
            optionsMenuGroup.gameObject.SetActive(false);
        }
        else if (controlsMenuGroup && controlsMenuGroup.gameObject.activeSelf)
        {
            controlsMenuGroup.gameObject.SetActive(false);
        }
        else if (creditsMenuGroup && creditsMenuGroup.gameObject.activeSelf)
        {
            creditsMenuGroup.gameObject.SetActive(false);
        }

        startSelectable.Select();
    }
}
