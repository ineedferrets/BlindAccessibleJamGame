using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressControlTextComponent : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public string controls;

    private TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        PlayerInput playerInput = player ? player.GetComponent<PlayerInput>() : null;
        if (!playerInput || !textMesh)
        {
            return;
        }

        InputAction interactAction = playerInput.actions.FindAction(controls);
        if (interactAction == null)
        {
            return;
        }

        string controlBindingsStr = interactAction.GetBindingDisplayString(InputBinding.MaskByGroup(playerInput.currentControlScheme));
 
        textMesh.text = textMesh.text.Replace("(controls)", controlBindingsStr);
    }
}
