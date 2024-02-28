using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerTopDownController : MonoBehaviour
{
    public Rigidbody2D rb;
    [Header("Movement")]
    public float moveSpeed = 5f;
    float xMovement;
    float yMovement;

    [Header("Ping System")]
    public FMODUnity.EventReference fmodPingObjectiveSFXEvent;
    private FMOD.Studio.EventInstance objectivePingInstance;

    [Header("World Cameras"), Tooltip("This needs to be improved to a system that doesn't rely on hard references.")]
    public Camera cemeteryCamera;
    public Camera gardenCamera;
    public Camera homeCamera;

    PlayerInput playerInput;

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

    private void Start()
    {
        objectivePingInstance = FMODUnity.RuntimeManager.CreateInstance(fmodPingObjectiveSFXEvent);
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

    // PING CODE ----------------------------------------------
    public void ObjectivePing(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        QuestManager questManager = QuestManager.Instance;
        if (questManager == null) { return; }

        GameObject objective = questManager.currentObjective;
        if (objective == null) { return; }

        // Find screen objective is on.
        List<Camera> cameras = new List<Camera> { homeCamera, cemeteryCamera, gardenCamera };

        Camera closestObjectiveCamera = null;
        float closestObjectiveDist = float.MaxValue;
        Camera activeCamera = null;
        foreach (Camera camera in cameras)
        {
            float distanceToObjective = Vector2.Distance(camera.transform.position, objective.transform.position);
            if (distanceToObjective < closestObjectiveDist)
            {
                closestObjectiveDist = distanceToObjective;
                closestObjectiveCamera = camera;
            }

            if (camera.gameObject.activeSelf)
            {
                activeCamera = camera;
            }
        }

        // If the objective is in the same area as the player, just set the ping to the objective.
        if (closestObjectiveCamera == activeCamera)
        {
            PlayEmitterOnGameObject(objective, fmodPingObjectiveSFXEvent);
            return;
        }

        // If the objective is in a different area as the player, we path find through hard code.
        // This is extremely inelegant and lazy and since it's the end of the jam, I'm doing it
        // this way. Sorry future me.

        List<GameObject> teleporters = new List<GameObject>(GameObject.FindGameObjectsWithTag("Teleporter"));
        string teleporterStringToFind = "";
        // We can assume the player needs to go to the garden if in the cemetery or house.
        if (activeCamera == cemeteryCamera || activeCamera == homeCamera)
        {
            teleporterStringToFind = activeCamera == cemeteryCamera ? "CemToGar" : "IndToGar";
        }
        // Otherwise we are in the garden, find which teleporter to go to. I hate myself for this code.
        else
        {
            teleporterStringToFind = closestObjectiveCamera == cemeteryCamera ? "GarToCem" : "GarToInd";
        }

        GameObject teleporter = teleporters.Find((teleporter) => teleporter.name.Contains(teleporterStringToFind));
        Debug.Log(teleporter);
        PlayEmitterOnGameObject(teleporter, fmodPingObjectiveSFXEvent);
        return;
    }

    private void PlayEmitterOnGameObject(GameObject emitter, EventReference eventReference)
    {
        StudioEventEmitter eventEmitter = emitter.GetComponent<StudioEventEmitter>();
        if (eventEmitter != null)
        {
            eventEmitter.EventReference = eventReference;
        }
        else
        {
            eventEmitter = emitter.AddComponent<StudioEventEmitter>();
            eventEmitter.EventReference = eventReference;
        }
        eventEmitter.Play();
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
