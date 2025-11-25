using UnityEngine;

public class MeteorMovement : MonoBehaviour
{
    [Header("Meteor Settings")]
    public float horizontalSpeed = 8f; // Speed moving left
    public float verticalSpeed = 10f;  // Speed moving down
    
    private float leftEdge = -15f;
    private float bottomEdge = -10f;
    
    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.isGameOver || GameManager.Instance.isPaused) 
            return;
        
        // Move diagonally (left and down)
        Vector3 movement = new Vector3(-horizontalSpeed, -verticalSpeed, 0) * Time.deltaTime;
        transform.position += movement;
        
        // Destroy when off-screen
        if (transform.position.x < leftEdge || transform.position.y < bottomEdge)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance != null && !GameManager.Instance.isGameOver)
        {
            Debug.Log("Meteor hit player!");
            GameManager.Instance.GameOver();
        }
    }
}