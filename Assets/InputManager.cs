using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerComponent : MonoBehaviour
{
    private PlayerInput playerInput;

    public DialogueAsset TestAsset;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        playerInput.SwitchCurrentActionMap("UI");
        playerInput.actions["Submit"].performed += SkipText;

        DialogueBoxController.instance.StartDialogue(TestAsset, 0);
    }

    private void SkipText(InputAction.CallbackContext context)
    {
        DialogueBoxController.instance.SkipLine();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
