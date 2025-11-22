using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController character;
    private Vector3 direction;
    private PlayerControls controls; // Assumed to be defined elsewhere/in project

    [Header("Movement Settings")]
    public float baseJumpForce = 25f; // Set to 25f for a higher jump
    public float dashSpeed = 22f;
    public float crouchHeight = 0.5f;
    public float crouchDuration = 0.5f; // How long the player stays crouched after a tap
    private float normalHeight;
    private Vector3 normalCenter;
    private Vector3 crouchCenter;
    
    [Header("Dash Settings")]
    public float dashDistance = 2f;
    public float dashDuration = 0.18f;
    public float returnDuration = 0.12f;
    
    // State Variables
    private bool jumpPressed;
    private bool isCrouching; 
    private bool isAttemptingToStand; 
    private bool isDashing;
    private bool isGrounded;
    private Vector3 originalPosition;
    
    private Coroutine dashCoroutine;
    private Coroutine crouchCoroutine;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        normalHeight = character.height;
        // Calculate centers based on heights
        normalCenter = character.center; 
        crouchCenter = new Vector3(normalCenter.x, crouchHeight / 2f, normalCenter.z);

        controls = new PlayerControls(); 
        originalPosition = transform.position;
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Jump.performed += OnJumpInput;
        controls.Player.Crouch.performed += OnCrouchInput; 
        controls.Player.Dash.performed += OnDashInput;
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Jump.performed -= OnJumpInput;
        controls.Player.Crouch.performed -= OnCrouchInput;
        controls.Player.Dash.performed -= OnDashInput;
    }

    private void Update()
    {
        if (GameManager.Instance != null && (GameManager.Instance.isGameOver || GameManager.Instance.isPaused))
            return;

        isGrounded = character.isGrounded;

        // 1. Handle Stand-Up Safety Check (Ensures crouch returns to normal)
        HandleStandUpCheck();
        
        // 2. Handle Vertical Movement (Jump and Gravity)
        if (isGrounded && !isDashing)
        {
            if (direction.y < 0)
                direction.y = -1f;

            if (jumpPressed && !isCrouching && !isAttemptingToStand)
            {
                direction.y = baseJumpForce;
                jumpPressed = false;
            }
        }
        else if (!isDashing)
        {
            if (GameManager.Instance != null)
            {
                 direction.y -= GameManager.Instance.CurrentGravity * Time.deltaTime;
            }
        }

        // 3. Horizontal Movement
        if (!isDashing)
        {
            direction.x = 0;
        }

        // 4. Apply Height and Center based on state
        bool isSmall = isCrouching || isAttemptingToStand;
        
        character.height = isSmall ? crouchHeight : normalHeight;
        character.center = isSmall ? crouchCenter : normalCenter;
        
        // 5. Update Original Position
        if (!isDashing && isGrounded)
        {
            originalPosition = transform.position;
        }
        
        // 6. Execute Move
        character.Move(direction * Time.deltaTime);
    }
    
    private void HandleStandUpCheck()
    {
        if (isAttemptingToStand)
        {
            float raycastDistance = normalHeight - crouchHeight;
            Vector3 rayOrigin = transform.position + Vector3.up * crouchHeight;
            
            // Check against all layers (using ~0) and ignore triggers
            if (!Physics.Raycast(rayOrigin, Vector3.up, raycastDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                isAttemptingToStand = false;
            }
        }
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isDashing && !isCrouching && !isAttemptingToStand && GameManager.Instance != null && !GameManager.Instance.isGameOver) 
            jumpPressed = true;
    }

    private void OnCrouchInput(InputAction.CallbackContext context) 
    { 
        if (!isDashing && isGrounded && GameManager.Instance != null && !GameManager.Instance.isGameOver) 
        {
            if (crouchCoroutine != null)
                StopCoroutine(crouchCoroutine);
            
            crouchCoroutine = StartCoroutine(CrouchRoutine());
        }
    }
    
    private IEnumerator CrouchRoutine()
    {
        isCrouching = true;
        isAttemptingToStand = false; 

        yield return new WaitForSeconds(crouchDuration);
        
        isCrouching = false;
        isAttemptingToStand = true; 

        crouchCoroutine = null;
    }

    private void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && isGrounded && GameManager.Instance != null && !GameManager.Instance.isGameOver)
        {
            if (dashCoroutine != null)
                StopCoroutine(dashCoroutine);
            dashCoroutine = StartCoroutine(DashRoutine());
        }
    }

    private System.Collections.IEnumerator DashRoutine()
    {
        isDashing = true;
        Vector3 dashStartPos = transform.position;
        Vector3 dashTargetPos = dashStartPos + Vector3.right * dashDistance;
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dashDuration;
            t = 1f - Mathf.Pow(1f - t, 2f);
            transform.position = Vector3.Lerp(dashStartPos, dashTargetPos, t); 
            yield return null;
        }

        transform.position = dashTargetPos;
        yield return new WaitForSeconds(0.06f);

        elapsedTime = 0f;
        Vector3 returnStartPos = transform.position;
        
        while (elapsedTime < returnDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / returnDuration;
            t = t * t;
            transform.position = Vector3.Lerp(returnStartPos, originalPosition, t);
            yield return null;
        }

        transform.position = originalPosition;
        isDashing = false;
        dashCoroutine = null;
    }
}