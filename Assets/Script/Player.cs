using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour, PlayerControls.IPlayerActions
{
    private CharacterController character;
    private Vector3 direction;
    private PlayerControls controls;

    [Header("Movement Settings")]
    public float jumpForce = 10f;
    public float gravity = 12f;
    public float dashSpeed = 8f;
    public float crouchHeight = 0.5f;
    private float normalHeight;

    private bool jumpPressed;
    private bool isCrouching;
    private bool isDashing;
    private bool isGrounded;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        normalHeight = character.height;

        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Update()
    {
        isGrounded = character.isGrounded;

        if (isGrounded)
        {
            // Reset Y when touching ground
            if (direction.y < 0)
                direction.y = -1f;

            // Jump trigger
            if (jumpPressed)
            {
                direction.y = jumpForce;
                jumpPressed = false;
            }
        }
        else
        {
            // Apply gravity only when airborne
            direction.y -= gravity * Time.deltaTime;
        }

        // Handle dash (short burst forward)
        if (isDashing)
        {
            direction.x = dashSpeed;
        }
        else
        {
            direction.x = 0;
        }

        // Handle crouch
        character.height = isCrouching ? crouchHeight : normalHeight;

        // Move player
        character.Move(direction * Time.deltaTime);
    }

    // === Input Callbacks ===
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            jumpPressed = true;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
            isCrouching = true;
        else if (context.canceled)
            isCrouching = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
            StartCoroutine(DashRoutine());
    }

    private System.Collections.IEnumerator DashRoutine()
    {
        isDashing = true;
        yield return new WaitForSeconds(0.25f);
        isDashing = false;
    }
}
