using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Automatically adds CharacterController component to GameObject
[RequireComponent(typeof(CharacterController))]
public class FPMovement : MonoBehaviour
{
    public float speed = 10;
    float h, v;
    public float gravity = -9.8f;
    public float jumpStrength = 9.0f;
    float velocity;
    float gravityMultiplier = 3.0f;
    public bool canDoubleJump = false;
    public bool doubleJumpUsed = false;
    

    CharacterController controller;

    // Cache the controller before input callbacks run.
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = h * speed;
        float moveZ = v * speed;
        Vector3 movement = new Vector3(moveX, 0, moveZ);

        movement = Vector3.ClampMagnitude(movement, speed);

        if(IsGrounded() && velocity < 0)
        {
            velocity = -1;
            doubleJumpUsed = false;
        }
        else
        {
            velocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        if(canDoubleJump)
        {
            Debug.Log("FPMovemnt can jump true");

        }

        movement.y = velocity;

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        controller.Move(movement);
    }

    public void MoveInput(InputAction.CallbackContext ctx)
    {
        h = ctx.ReadValue<Vector2>().x;
        v = ctx.ReadValue<Vector2>().y;
    }

    //public void Jump(InputAction.CallbackContext ctx)
    //{

    //    if (!IsGrounded())
    //    {
    //        //doubleJumpUsed = true;
    //        return;
    //    }

    //    if (ctx.performed && !doubleJumpUsed)
    //    {
    //        velocity = jumpStrength;
    //    }

    //    if (ctx.performed && canDoubleJump && velocity >=0.1f && !controller.isGrounded /*&&  !IsGrounded*/ )
    //    {
    //        velocity += jumpStrength;
    //        //canDoubleJump = false;
    //        doubleJumpUsed = true;
    //    }
    //}

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)return;
            // i struglled with this for a while
            // had to
            if (IsGrounded())
        {
            velocity = jumpStrength;
        }
        else if (canDoubleJump && !doubleJumpUsed)
        {
            velocity = jumpStrength;
            doubleJumpUsed = true;
        }
    }

    bool IsGrounded()
    {
        return controller.isGrounded;
    }
}
