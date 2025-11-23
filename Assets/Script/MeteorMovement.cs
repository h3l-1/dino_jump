using UnityEngine;

public class MeteorMovement : MonoBehaviour
{
    [Header("Meteor Settings")]
    public float fallSpeed = 8f;
    public float horizontalSpeed = 2f;
    
    private Vector3 targetPosition;
    private bool hasTarget = false;
    private float leftEdge = -15f;

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        hasTarget = true;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.isGameOver || GameManager.Instance.isPaused) return;

        if (hasTarget)
        {
            // Move toward target
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 movement = new Vector3(
                direction.x * horizontalSpeed,
                -fallSpeed,
                0
            ) * Time.deltaTime;
            
            transform.position += movement;
            transform.Rotate(0, 0, 45 * Time.deltaTime);
        }
        else
        {
            // Fall straight down
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }

        // Destroy when off-screen
        if (transform.position.x < leftEdge || transform.position.y < -10f)
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