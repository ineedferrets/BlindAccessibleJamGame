using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementTopDown : MonoBehaviour
{
    public Rigidbody rb;
    [Header("Movement")]
    public float moveSpeed = 5f;
    
    float xMovement;
    float zMovement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector3(xMovement * moveSpeed, 0.0f, zMovement * moveSpeed);
    }

    public void Move(InputAction.CallbackContext context)
    {
        xMovement = context.ReadValue<Vector2>().x;
        zMovement = context.ReadValue<Vector2>().y;
    }
}
