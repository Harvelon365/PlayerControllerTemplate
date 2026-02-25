using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // REMEMBER: Set player rigidbody mass to 3
    [Header("Movement")] 
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private Transform orientation;
    [SerializeField] private float groundDrag = 5f;
    
    [Header("Jumping")]
    [SerializeField] private bool enableJump = true;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private float airMultiplier = 0.4f;
    private bool readyToJump = true;
    private bool isSprinting = false;
    
    [Header("Ground Check")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask groundLayer;
    private bool grounded;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    
    private float horizontalInput;
    private float verticalInput;
    
    private Vector3 moveDir;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (enableJump) grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        isSprinting = Input.GetKey(sprintKey);

        if (enableJump && Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        
        var flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVel.magnitude > (isSprinting ? sprintSpeed : moveSpeed))
        {
            var limitedVel = flatVel.normalized * (isSprinting ? sprintSpeed : moveSpeed);
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }

        if (grounded) rb.linearDamping = groundDrag;
        else  rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        rb.AddForce(moveDir.normalized * (isSprinting ? sprintSpeed : moveSpeed) * 10  * (grounded ? 1 : airMultiplier), ForceMode.Force);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
