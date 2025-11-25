using UnityEngine;

public class MeteorMovement : MonoBehaviour
{
    [Header("Meteor Settings")]
    public float moveSpeed = 12f; // Speed moving toward target
    
    // **FIXED:** Define the fixed coordinates you want the meteor to aim for.
    // If you want the player's actual start position, use public Vector3 and set it in the Inspector.
    private readonly Vector3 TargetPosition = new Vector3(-1f, -1.29f, 0f); 
    
    private Vector3 moveDirection;
    
    // Using MainCamera.cs static properties is cleaner, but if you want simple constants:
    private const float LeftEdge = -15f;
    private const float BottomEdge = -10f;
    
    void Start()
    {
        // Calculate direction toward the fixed target position when meteor spawns
        // The '.normalized' part is essential to get a unit vector for smooth movement.
        moveDirection = (TargetPosition - transform.position).normalized;
    }
    
    void Update()
    {
        // Simple and robust check: return if the game is not active
        if (GameManager.Instance == null || GameManager.Instance.isGameOver || GameManager.Instance.isPaused) 
            return;
        
        // Move in the fixed direction calculated in Start()
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        
        // Destroy when off-screen (X or Y falls below the defined threshold)
        if (transform.position.x < LeftEdge || transform.position.y < BottomEdge)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // We use a safety check to ensure GameManager exists before calling methods
        if (other.CompareTag("Player") && GameManager.Instance != null && !GameManager.Instance.isGameOver)
        {
            Debug.Log("Meteor hit player!");
            GameManager.Instance.GameOver();
        }
    }
}