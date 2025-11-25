using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float leftEdge = -20f; // Default destroy position far off-screen
    
    private void Start()
    {
        // Try to calculate based on camera, but use default if camera not found
        if (Camera.main != null)
        {
            leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 10f;
        }
    }
    
    private void Update()
    {
        // Safety check - don't move if GameManager doesn't exist
        if (GameManager.Instance == null) return;
        
        // Move obstacle left
        transform.position += GameManager.Instance.gameSpeed * Time.deltaTime * Vector3.left;
        
        // Destroy when far off-screen to the left
        if (transform.position.x < leftEdge) {
            Destroy(gameObject);
        }
    }
}