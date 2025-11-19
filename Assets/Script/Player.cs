using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController character;
    private Vector3 direction;
    private PlayerControls controls;

    [Header("Movement Settings")]
    public float baseJumpForce = 18f;      // Jump height
    public float dashSpeed = 22f;          // Dash speed
    public float crouchHeight = 0.5f;      // Height when crouching
    private float normalHeight;

    [Header("Dash Settings")]
    public float dashDistance = 2f;        // Distance covered when diashing
    public float dashDuration = 0.18f;     // Time to complete dash forward
    public float returnDuration = 0.12f;   // Time to return from dash
    
    private bool jumpPressed;
    private bool isCrouching;
    private bool isDashing;
    private bool isGrounded;
    private Vector3 originalPosition;
    private Coroutine dashCoroutine;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        normalHeight = character.height;
        controls = new PlayerControls();
        originalPosition = transform.position;
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Jump.performed += OnJumpInput;
        controls.Player.Crouch.performed += OnCrouchInput;
        controls.Player.Crouch.canceled += OnCrouchCanceled;
        controls.Player.Dash.performed += OnDashInput;
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Jump.performed -= OnJumpInput;
        controls.Player.Crouch.performed -= OnCrouchInput;
        controls.Player.Crouch.canceled -= OnCrouchCanceled;
        controls.Player.Dash.performed -= OnDashInput;
    }

    private void Update()
    {
        isGrounded = character.isGrounded;

        if (isGrounded && !isDashing)
        {
            if (direction.y < 0)
                direction.y = -1f;

            if (jumpPressed)
            {
                direction.y = baseJumpForce;
                jumpPressed = false;
            }
        }
        else if (!isDashing)
        {
            direction.y -= GameManager.Instance.CurrentGravity * Time.deltaTime;
        }

        if (!isDashing)
        {
            direction.x = 0;
        }

        character.height = isCrouching ? crouchHeight : normalHeight;
        
        if (!isDashing && isGrounded)
        {
            originalPosition = transform.position;
        }
        
        character.Move(direction * Time.deltaTime);
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isDashing) 
            jumpPressed = true;
    }

    private void OnCrouchInput(InputAction.CallbackContext context) 
    { 
        if (!isDashing) isCrouching = true; 
    }
    
    private void OnCrouchCanceled(InputAction.CallbackContext context) 
    { 
        if (!isDashing) isCrouching = false; 
    }

    private void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && isGrounded)
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