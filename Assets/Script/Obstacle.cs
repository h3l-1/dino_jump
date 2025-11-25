using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float leftEdge = -20f; // x position outside screen
    
    private void Start()
    {
        // calculate based on camera, but use default if camera not found
        if (Camera.main != null)
        {
            leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 10f;
        }
    }
    
    private void Update()
    {
        // don't move if GameManager doesn't exist 
        if (GameManager.Instance == null) return;
        
        // move obstacle left
        transform.position += GameManager.Instance.gameSpeed * Time.deltaTime * Vector3.left;
        
        // destroy when outside screen to the left
        if (transform.position.x < leftEdge) {
            Destroy(gameObject);
        }
    }
}