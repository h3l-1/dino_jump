using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls controls;
    
    public event Action OnJumpPressed;
    public event Action OnCrouchStarted;
    
    public event Action OnDashPressed;
    public bool IsJumpHeld { get; private set; }

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        // Ensure the 'Player' map is enabled
        controls.Player.Enable();
        
        // Subscribe to input events using explicit methods
        controls.Player.Jump.started += OnJumpStart;     // Key Down
        controls.Player.Jump.canceled += OnJumpEnd;      // Key Up
        controls.Player.Crouch.started += OnCrouchStart; // Key Down (Only start is needed now)
        // Removed: controls.Player.Crouch.canceled += OnCrouchEnd; 
        controls.Player.Dash.performed += OnDash;
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        controls.Player.Jump.started -= OnJumpStart;
        controls.Player.Jump.canceled -= OnJumpEnd;
        controls.Player.Crouch.started -= OnCrouchStart;
        // Removed: controls.Player.Crouch.canceled -= OnCrouchEnd; 
        controls.Player.Dash.performed -= OnDash;
        
        controls.Player.Disable();
    }

    // Handlers for Jump (Start and End)
    private void OnJumpStart(InputAction.CallbackContext context)
    {
        // CRITICAL FIX: Set state immediately on press
        IsJumpHeld = true;
        OnJumpPressed?.Invoke(); // Fires the event for Player.cs to start the jump
    }
    
    private void OnJumpEnd(InputAction.CallbackContext context)
    {
        // CRITICAL FIX: Reset state immediately on release to cut the jump short
        IsJumpHeld = false;
    }

    private void OnCrouchStart(InputAction.CallbackContext context)
    {
        OnCrouchStarted?.Invoke();
    }


    private void OnDash(InputAction.CallbackContext context)
    {
        OnDashPressed?.Invoke();
    }

    private void OnDestroy()
    {
        controls?.Dispose();
    }
}