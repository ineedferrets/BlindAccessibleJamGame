using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerComponent : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        playerInput.SwitchCurrentActionMap("Shop");
        playerInput.actions["Submit"].performed += SkipText;
    }

    private void SkipText(InputAction.CallbackContext context)
    {
        DialogueBoxController dialogueBoxController = DialogueBoxController.instance;

        if (dialogueBoxController != null ) { DialogueBoxController.instance.SkipLine(); }
    }

    public void ToggleMenuControls()
    {
        if ( playerInput != null )
        {
            if (playerInput.currentActionMap == playerInput.actions.FindActionMap("UI"))
            {
                playerInput.SwitchCurrentActionMap("Shop");
            }
            else
            {
                playerInput.SwitchCurrentActionMap("UI");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
