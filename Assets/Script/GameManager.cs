using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Speed Settings")]
    public float gameSpeed = 1.5f;           // Base speed for parallax/ground
    public float speedIncreaseRate = 0.01f; // Optional: speed up over time
    public float maxSpeed = 4f;

    [Header("Game State")]
    public bool isGameOver = false;
    public bool isPaused = false;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (isGameOver || isPaused) return;

        // Gradually increase speed (optional)
        if (gameSpeed < maxSpeed)
        {
            gameSpeed += speedIncreaseRate * Time.deltaTime;
        }
    }

    // === Game Control Methods ===

    public void StartGame()
    {
        isGameOver = false;
        isPaused = false;
        gameSpeed = 1f;
        Time.timeScale = 0.1f;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        Debug.Log("Game Over!");
        // Optionally reload scene after a short delay
        // StartCoroutine(ReloadScene());
    }

    private System.Collections.IEnumerator ReloadScene()
    {
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
