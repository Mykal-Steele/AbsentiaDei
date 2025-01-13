using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float crouchSpeed = 5f;
    public float jumpHeight = 2f; // Jump height instead of force
    public float gravity = -9.81f; // Gravity force applied to the character

    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;
    public float maxLookAngle = 85f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1.2f; // Height when crouching
    public float normalHeight = 2f; // Normal standing height
    public float crouchTransitionSpeed = 10f;

    [Header("Ladder Settings")]
    public LayerMask ladderMask; // Layer for ladder objects
    public float climbSpeed = 5f;

    private CharacterController characterController;
    private Vector3 velocity; // Used for gravity and jump physics
    private float verticalLookRotation;
    private bool isCrouching = false;

    [Header("Ground Check")]
    public Transform groundCheck; // Empty object placed at the bottom of the player
    public float groundCheckRadius = 0.2f; // Radius of ground check sphere
    public LayerMask groundMask; // Layer for ground objects
    private bool isGrounded = false;

    private bool isOnLadder = false; // Tracks if player is on a ladder

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        GroundCheck();
        LadderCheck();
        if (isOnLadder)
        {
            HandleLadderMovement();
        }
        else
        {
            HandleMovement();
        }
        HandleMouseLook();
        HandleCrouch();
    }

    private void HandleMovement()
    {
        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;

        // Apply speed adjustments for crouching
        float speed = isCrouching ? crouchSpeed : moveSpeed;
        characterController.Move(moveDirection * speed * Time.deltaTime);

        // Jump logic
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Physics formula for jump velocity
        }

        // Apply consistent gravity
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Move character based on gravity and jump
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // Rotate the player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -maxLookAngle, maxLookAngle);

        playerCamera.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
        }

        // Adjust height smoothly
        float targetHeight = isCrouching ? crouchHeight : normalHeight;
        float currentHeight = characterController.height;
        characterController.height = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * crouchTransitionSpeed);
    }

    private void GroundCheck()
    {
        // Perform a sphere check to determine if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        // Reset vertical velocity when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep the player grounded
        }
    }

    private void LadderCheck()
{
    // Perform a sphere check to determine if the player is on a ladder
    isOnLadder = Physics.CheckSphere(groundCheck.position, groundCheckRadius, ladderMask);

    if(isOnLadder && !isGrounded && Input.GetKey(KeyCode.Space)){
        isOnLadder = false;
    }
    // Reset vertical velocity when on a ladder
    if (isOnLadder && !isGrounded)
    {
        velocity.y = 0f; // Disable gravity while on the ladder
    }
    else if (isOnLadder && isGrounded && Input.GetKey(KeyCode.S))
    {
        // Exit ladder when grounded and pressing S
        isOnLadder = false;
    }
}

private void HandleLadderMovement()
{
    // Check if the player presses Space to exit the ladder state
    if (Input.GetKeyDown(KeyCode.Space))
    {
        // Exit ladder state without any additional force or jump
        isOnLadder = false;

        // Return immediately to stop processing ladder movement
        return;
    }

    // Get input for vertical movement on the ladder
    float vertical = 0f;
    if (Input.GetKey(KeyCode.W))
    {
        vertical = 1f;
    }
    else if (Input.GetKey(KeyCode.S))
    {
        vertical = -1f;

        // If grounded and holding S, exit the ladder
        if (isGrounded)
        {
            isOnLadder = false;
            return;
        }
    }

    // Move the player up or down the ladder
    Vector3 climbDirection = transform.up * vertical;
    characterController.Move(climbDirection * climbSpeed * Time.deltaTime);
}



}