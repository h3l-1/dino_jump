using UnityEngine;

[RequireComponent(typeof(Collider))] 
[RequireComponent(typeof(Rigidbody))] 
public class Obstacle : MonoBehaviour
{
    [Header("General Settings")]
    public float speedMultiplier = 1f; // Allows some objects (like Birds) to be faster than ground
    private float destroyXPos = -20f;  // Destroy further off-screen so it doesn't pop out visibly
    
    [Header("Meteor Settings")]
    public bool isMeteor = false;      // CRITICAL: This lets meteors fly diagonally
    public float meteorDropSpeed = 5f; 

    private bool hasCollided = false;  // Prevents triggering Game Over twice

    void Start()
    {
        // Setup Rigidbody correctly for manual movement (Kinematic)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Objects controlled by script (transform.Translate) should be Kinematic
            rb.isKinematic = true; 
            rb.useGravity = false;
        }
    }

    void Update()
    {
        if (IsGameOverOrPaused()) return;

        // 1. Calculate Base Speed (Global Speed * Multiplier)
        float currentSpeed = GameManager.Instance.gameSpeed * speedMultiplier;

        // 2. Calculate Left Movement
        Vector3 movement = Vector3.left * currentSpeed * Time.deltaTime;

        // 3. Meteor Logic
        if (isMeteor)
        {
            // Adds downward movement to the left movement
            movement += Vector3.down * meteorDropSpeed * Time.deltaTime;
        }

        // 4. Apply Movement
        transform.Translate(movement, Space.World);

        // 5. Cleanup
        if (transform.position.x < destroyXPos)
        {
            Destroy(gameObject);
        }
    }

    private bool IsGameOverOrPaused()
    {
        return GameManager.Instance == null || 
               GameManager.Instance.isGameOver || 
               GameManager.Instance.isPaused;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check hasCollided to ensure we don't kill the player twice in one frame
        if (other.CompareTag("Player") && !hasCollided)
        {
            Debug.Log("Hit Player!");
            hasCollided = true; 
            GameManager.Instance.GameOver();
        }
    }
}