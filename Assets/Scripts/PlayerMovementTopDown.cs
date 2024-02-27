using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerTopDownController : MonoBehaviour
{
    public Rigidbody2D rb;
    [Header("Movement")]
    public float moveSpeed = 5f;

    PlayerInput playerInput;
    
    float xMovement;
    float yMovement;

    TeleportTriggerVolumeComponent currentTriggerCollision = null;

    List<InteractableComponent> interactables = new List<InteractableComponent>();
    InteractableComponent closestInteractable = null;

    public enum ControlScheme
    {
        WorldControls,
        MenuControls,
        DialogueControls
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        SetControlScheme(ControlScheme.WorldControls);

        DialogueBoxController.OnDialogueStarted += EnterDialogueControls;
        DialogueBoxController.OnDialogueEnded += ExitDialogueControls;
    }

    private void OnDestroy()
    {
        DialogueBoxController.OnDialogueStarted -= EnterDialogueControls;
        DialogueBoxController.OnDialogueEnded -= ExitDialogueControls;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(xMovement * moveSpeed, yMovement * moveSpeed);

        if (interactables.Count > 0)
        {
            float closestDist = float.MaxValue;
            foreach (var interactable in interactables)
            {
                float interactDistance = Vector2.Distance(interactable.transform.position, transform.position);
                if (closestDist > interactDistance)
                {
                    closestDist = interactDistance;
                    closestInteractable = interactable;
                }
            }

            foreach (var interactable in interactables)
            {
                interactable.ToggleVisuals(interactable == closestInteractable);
            }
        }
    }

    private void OnValidate()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // INPUT CODE ----------------------------------------------
    public void Move(InputAction.CallbackContext context)
    {
        xMovement = context.ReadValue<Vector2>().x;
        yMovement = context.ReadValue<Vector2>().y;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (closestInteractable)
            {
                closestInteractable.Interact();
            }
            if (currentTriggerCollision)
            {
                TeleportPlayer();
            }
        }
    }

    public void SetToUIScheme()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    // PAUSE MENU CODE ----------------------------------------------
    public void PauseGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleMenu();
        }
    }
    
    public void ToggleMenu()
    {
        PauseMenuController pauseMenuController = PauseMenuController.Instance;

        if (pauseMenuController == null || playerInput == null)
            return;

        if (playerInput.currentActionMap == playerInput.actions.FindActionMap("UI"))
        {
            SetControlScheme(ControlScheme.WorldControls);
            pauseMenuController.SetPauseMenu(false);
        }
        else
        {
            SetControlScheme(ControlScheme.MenuControls);
            pauseMenuController.SetPauseMenu(true);
        }
    }

    // TELEPORTATION CODE ----------------------------------------------
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Teleporter")
        {
            currentTriggerCollision = collision.GetComponent<TeleportTriggerVolumeComponent>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Teleporter")
        {
            currentTriggerCollision = null;
        }
    }

    private void TeleportPlayer()
    {
        Debug.Log("Teleporting.");
        gameObject.transform.position = currentTriggerCollision.toTeleportTo;

        Camera.main.gameObject.SetActive(false);
        currentTriggerCollision.toChangeCameraTo.gameObject.SetActive(true);
    }

    // INTERACTION CODE ----------------------------------------------
    public void AddInteractable(InteractableComponent newComponent)
    {
        if (!interactables.Contains(newComponent))
        {
            interactables.Add(newComponent);
        }
    }

    public void RemoveInteractable(InteractableComponent interactable)
    {
        interactables.Remove(interactable);
        interactable.ToggleVisuals(false);
        if (closestInteractable == interactable)
            closestInteractable = null;
    }

    // DIALOGUE CODE ----------------------------------------------
    public void EnterDialogueControls() { SetControlScheme(ControlScheme.DialogueControls); }
    public void ExitDialogueControls() { SetControlScheme(ControlScheme.WorldControls); }

    public void SetControlScheme(ControlScheme control)
    {
        switch(control)
        {
            case ControlScheme.WorldControls:
                playerInput.SwitchCurrentActionMap("Player");
                break;
            case ControlScheme.MenuControls:
                playerInput.SwitchCurrentActionMap("UI");
                break;
            case ControlScheme.DialogueControls:
                playerInput.SwitchCurrentActionMap("Dialogue");
                break;
        }
    }
}
