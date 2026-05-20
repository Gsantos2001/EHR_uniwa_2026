using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.0f;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        Vector2 inputVector = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            inputVector.y += 1;

        if (Keyboard.current.sKey.isPressed)
            inputVector.y -= 1;

        if (Keyboard.current.dKey.isPressed)
            inputVector.x += 1;

        if (Keyboard.current.aKey.isPressed)
            inputVector.x -= 1;

        horizontalInput = inputVector.x;
        verticalInput = inputVector.y;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput
                      + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f,
                    ForceMode.Force);
    }
}
