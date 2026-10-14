using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxis
    {
        MouseXAndMouseY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxis axis = RotationAxis.MouseXAndMouseY;

    public float sensitivityX = 0.8f;
    public float sensitivityY = 0.8f;
    // Stick turning speed in degrees per second at full deflection.
    public float controllerTurnSpeed = 120f;
    public float maxVerticalRotation = 45.0f;
    public float minVerticalRotation = -45.0f;

    float mouseX;
    float mouseY;
    float verticalRotation = 0;
    bool isStickInput;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Rigidbody rb = GetComponent<Rigidbody>();

        if(rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Pointer delta is a distance; stick input requests a turning speed.
        float rotationX = mouseX * sensitivityX;
        float rotationY = mouseY * sensitivityY;
        if (isStickInput)
        {
            rotationX = mouseX * controllerTurnSpeed * Time.deltaTime;
            rotationY = mouseY * controllerTurnSpeed * Time.deltaTime;
        }

        if (axis == RotationAxis.MouseX)
        {
            //Just rotation X
            transform.Rotate(0, rotationX, 0);
        }
        else if (axis == RotationAxis.MouseY)
        {
            //Just Rotation Y
            verticalRotation -= rotationY;
            verticalRotation = Mathf.Clamp(verticalRotation, minVerticalRotation, maxVerticalRotation);

            float horizontalRotation = transform.localEulerAngles.y;

            transform.localEulerAngles = new Vector3(verticalRotation,horizontalRotation, 0);

        }
        else
        {
            //Both Rotation X and Y
            verticalRotation -= rotationY;
            verticalRotation = Mathf.Clamp(verticalRotation, minVerticalRotation, maxVerticalRotation);

            float deltaRotation = rotationX;
            float horizontalRotation = transform.localEulerAngles.y + deltaRotation;

            transform.localEulerAngles = new Vector3(verticalRotation, horizontalRotation, 0);
        }
    }
    
    public void LookValues(InputAction.CallbackContext ctx)
    {
        isStickInput = ctx.control.device is Gamepad || ctx.control.device is Joystick;
        mouseX = ctx.ReadValue<Vector2>().x;
        mouseY = ctx.ReadValue<Vector2>().y;
    }

   
}
