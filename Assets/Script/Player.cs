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
    
    private float targetXPosition; 

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
        
        // crouch logic
        bool crouchInput = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        
        if (crouchInput && isGrounded)
        {
            if (!isCrouching) {
                character.height = crouchHeight;
                isCrouching = true;
            }
            
            // Set crouch animation
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
        
        if (isGrounded)
        {
            direction = Vector3.down;
            
            // Jump (space, W, Up Arrow)
            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (!isCrouching) {
                    direction = Vector3.up * jumpForce;
                    jumpCount = 1; // First jump used
                }
            }
        }
        else
        {
            // double jump
            if (allowDoubleJump && jumpCount < 2)
            {
                if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    direction = Vector3.up * jumpForce;
                    jumpCount = 2; // Second jump used
                }
            }
            
            // press S or Down Arrow to fall faster
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                direction.y -= fastFallSpeed * Time.deltaTime;
            }
        }
        
        // dash logic
        bool dashInput = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        float currentX = transform.position.x;
        
        if (dashInput)
        {
            // Set the target to positive infinity for continuous movement to the right
            targetXPosition = float.PositiveInfinity;
        }
        else
        {
            // If !hold, then go back to initial position
            targetXPosition = originalPosition.x;
        }

        // move towards x
        float nextX = Mathf.MoveTowards(currentX, targetXPosition, dashSpeed * Time.deltaTime);

        // calculate horizontal displacement
        Vector3 horizontalMove = new Vector3(nextX - currentX, 0, 0);

        // calculate vertical displacement
        Vector3 verticalMove = direction * Time.deltaTime;

        // apply movement to the Character Controller
        character.Move(horizontalMove + verticalMove);
        
        // Update Dash Animation
        if (animator != null) {
            animator.SetBool("isDashing", dashInput); 
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) {
            // Trigger death animation
            if (animator != null) {
                animator.SetTrigger("Death");
            }
            GameManager.Instance.GameOver();
        }
    }

}