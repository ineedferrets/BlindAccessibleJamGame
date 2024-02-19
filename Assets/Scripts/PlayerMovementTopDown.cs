using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementTopDown : MonoBehaviour
{
    public Rigidbody2D rb;
    [Header("Movement")]
    public float moveSpeed = 5f;

    PlayerInput playerInput;
    
    float xMovement;
    float yMovement;

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

    public void Move(InputAction.CallbackContext context)
    {
        xMovement = context.ReadValue<Vector2>().x;
        yMovement = context.ReadValue<Vector2>().y;
    }

    private void OnValidate()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void ToggleMenuControls()
    {
        if (playerInput != null)
        {
            if (playerInput.currentActionMap == playerInput.actions.FindActionMap("UI"))
            {
                playerInput.SwitchCurrentActionMap("Outdoor");
            }
            else
            {
                playerInput.SwitchCurrentActionMap("UI");
            }
        }
    }
}
