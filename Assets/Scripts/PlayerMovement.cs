using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("State")]
    public bool canMove = true;

    [Header("Movement")]
    public Transform orientation;
    public float moveSpeed;
    public float groundDrag;

    [Header("Stamina")]
    public float maxStamina;
    public float staminaDrain;
    public float staminaRegen;
    public Slider staminaBar;
    public float currentStamina;
    public bool isExhausted = false;

    [Header("Ground Detection")]
    public float playerHeight;
    public LayerMask ground;

    private bool isGrounded;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool isJumpReady;

    [Header("Sprinting")]
    public float sprintSpeed;
    public bool isSprinting;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        isJumpReady = true;

        currentStamina = maxStamina;
        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = maxStamina;
        }
    }

    void Update()
    {
        // Ground Detection
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * .5f + .2f, ground);

        MyInput();
        HandleStamina();

        // Drag handling
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        SpeedControl();
    }

    private void MyInput()
    {
        if (!canMove) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        bool isTryingToSprint = Input.GetKey(sprintKey);
        bool isMoving = horizontalInput != 0 || verticalInput != 0;

        if (isTryingToSprint && isMoving && isGrounded && currentStamina > 0 && !isExhausted)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        // When to jump
        if (Input.GetKey(jumpKey) && isJumpReady && isGrounded)
        {
            isJumpReady = false;
            Jump();
            Invoke(nameof(JumpReset), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // Calculates movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;

        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * targetSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * targetSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void HandleStamina()
    {
        if (isSprinting && canMove)
        {
            currentStamina -= staminaDrain * Time.deltaTime;
            if (currentStamina < 0)
            {
                currentStamina = 0;
                isExhausted = true;
            }
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegen * Time.deltaTime;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
                isExhausted = false;
            }
        }
        if (staminaBar != null)
        {
            staminaBar.value = currentStamina;
        }
    }

    private void SpeedControl()
    {
        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limits velocity if necessary
        if (flatVel.magnitude > targetSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * targetSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void JumpReset()
    {
        isJumpReady = true;
    }

    public void SetMovementState(bool isActive)
    {
        canMove = isActive;
        if (!isActive)
        {
            horizontalInput = 0;
            verticalInput = 0;
            isSprinting = false;
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }
}