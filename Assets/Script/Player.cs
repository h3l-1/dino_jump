using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController character;
    private Vector3 direction;
    
    [Header("Jump Settings")]
    public float jumpForce = 18f;
    public float gravity = 20f;
    public bool allowDoubleJump = true;
    
    [Header("Fast Fall")]
    public float fastFallSpeed = 30f;
    
    [Header("Crouch Settings")]
    public float crouchHeight = 0.5f;
    private float normalHeight;
    
    [Header("Dash Settings")]
    public float dashDistance = 3f;
    public float dashSpeed = 15f;
    
    [Header("Animation")]
    public Animator animator;
    
    private Vector3 originalPosition;
    private bool isCrouching = false;
    private int jumpCount = 0; // Track number of jumps
    
    private void Awake()
    {
        character = GetComponent<CharacterController>();
        normalHeight = character.height;
        originalPosition = transform.position;
        
        // Auto-find animator if not assigned
        if (animator == null) {
            animator = GetComponentInChildren<Animator>();
        }
    }
    
    private void OnEnable()
    {
        direction = Vector3.zero;
        originalPosition = transform.position;
    }
    
    private void Update()
    {
        // Apply gravity
        direction += gravity * Time.deltaTime * Vector3.down;
        
        // Check if grounded
        bool isGrounded = character.isGrounded;
        
        // Reset jump count when grounded
        if (isGrounded) {
            jumpCount = 0;
        }
        
        // Update jump animation based on grounded state
        if (animator != null) {
            animator.SetBool("isJumping", !isGrounded);
        }
        
        if (isGrounded)
        {
            direction = Vector3.down;
            
            // Jump (Space, W, or Up Arrow)
            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (!isCrouching) {
                    direction = Vector3.up * jumpForce;
                    jumpCount = 1; // First jump used
                }
            }
            
            // Crouch (S or Down Arrow)
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                if (!isCrouching) {
                    character.height = crouchHeight;
                    isCrouching = true;
                }
                
                // Set crouch animation state
                if (animator != null) {
                    animator.SetBool("isCrouching", true);
                }
            }
            else if (isCrouching)
            {
                character.height = normalHeight;
                isCrouching = false;
                
                // Exit crouch animation
                if (animator != null) {
                    animator.SetBool("isCrouching", false);
                }
            }
        }
        else
        {
            // In air - make sure crouch animation is off
            if (animator != null) {
                animator.SetBool("isCrouching", false);
            }
            
            // DOUBLE JUMP - can jump once more while in air
            if (allowDoubleJump && jumpCount < 2)
            {
                if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    direction = Vector3.up * jumpForce;
                    jumpCount = 2; // Second jump used
                }
            }
            
            // FAST FALL - press S or Down Arrow to fall faster
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                direction.y -= fastFallSpeed * Time.deltaTime;
            }
        }
        
        // Handle Dash (D or Right Arrow - HOLD)
        Vector3 targetX = originalPosition;
        
        bool dashInput = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        
        if (dashInput)
        {
            targetX = originalPosition + Vector3.right * dashDistance;
        }
        
        // Set dash animation state
        if (animator != null) {
            animator.SetBool("isDashing", dashInput);
        }
        
        // Smooth horizontal movement
        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, targetX.x, dashSpeed * Time.deltaTime);
        transform.position = pos;
        
        // Apply vertical movement
        character.Move(direction * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Trigger death animation
            if (animator != null) {
                animator.SetTrigger("Death");
            }
            
            GameManager.Instance.GameOver();
        }
    }
}