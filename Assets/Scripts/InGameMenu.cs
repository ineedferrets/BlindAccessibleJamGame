using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class InGameMenu : MonoBehaviour
{
    public void OnEscapePress(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadSceneAsync("Menu");
        }
    }
}
