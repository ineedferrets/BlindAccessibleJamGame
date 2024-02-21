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

      // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(xMovement * moveSpeed, yMovement * moveSpeed);
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
            if (currentTriggerCollision)
            {
                TeleportPlayer();
            }
        }
    }

    // PAUSE MENU CODE ----------------------------------------------
    public void ToggleMenuControls()
    {
        if (playerInput != null)
        {
            if (playerInput.currentActionMap == playerInput.actions.FindActionMap("UI"))
            {
                playerInput.SwitchCurrentActionMap("Player");

            }
            else
            {
                playerInput.SwitchCurrentActionMap("UI");
            }
        }
    }

    // TELEPORTATION CODE ----------------------------------------------
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Teleporter")
        {
            currentTriggerCollision = collision.GetComponent<TeleportTriggerVolumeComponent>();
            Debug.Log("Teleporter trigger found.");
        }
    }

    private void TeleportPlayer()
    {
        gameObject.transform.position = currentTriggerCollision.toTeleportTo;

        Camera.main.gameObject.SetActive(false);
        currentTriggerCollision.toChangeCameraTo.gameObject.SetActive(true);
    }
}
