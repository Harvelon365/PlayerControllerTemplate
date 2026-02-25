using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintMult = 2f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float cameraSensX = 10f;
    [SerializeField] private float cameraSensY = 0.3f;
    [SerializeField] private float cameraXClamp = 85f;
    
    private CharacterController controller;
    private Transform cameraTransform;
    private Vector3 moveInput;
    private Vector3 velocity;
    private float mouseX;
    private float mouseY;
    private float cameraYRotation = 0f;
    private bool isSprinting = false;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();
        cameraTransform = transform.GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        var move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(transform.rotation * move * walkSpeed * (isSprinting ? sprintMult : 1) * Time.deltaTime);
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        transform.Rotate(Vector3.up, mouseX * Time.deltaTime);
        
        cameraYRotation -= mouseY;
        cameraYRotation = Mathf.Clamp(cameraYRotation, -cameraXClamp, cameraXClamp);
        var targetRotation = transform.eulerAngles;
        targetRotation.x = cameraYRotation;
        cameraTransform.eulerAngles = targetRotation;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void Look(InputAction.CallbackContext ctx)
    {
        mouseX = ctx.ReadValue<Vector2>().x * cameraSensX;
        mouseY = ctx.ReadValue<Vector2>().y * cameraSensY;
    }

    public void Sprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && controller.isGrounded) isSprinting = true;
        else isSprinting = false;
    }
}
