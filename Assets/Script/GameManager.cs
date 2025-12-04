using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiscoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button retryButton;
    
    [Header("Score")]
    public float score;
    
    [Header("Game Speed")]
    public float initialGameSpeed = 8f;
    public float gameSpeedIncrease = 0.5f;
    public float maxGameSpeed = 16f;
    public float gameSpeed { get; private set; }
    
    [Header("Gravity")]
    public float playerGravity = 20f;
    
    [Header("Game State")]
    public bool isGameOver = false;
    public bool isPaused = false;
    
    private Player player;
    private Spawner spawner;
    
    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }
    
    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        spawner = FindAnyObjectByType<Spawner>();
        
        // Connect the retry button to the restart function
        if (retryButton != null) {
            retryButton.onClick.AddListener(RestartGame);
        }
        
        NewGame();
    }
    
    public void NewGame()
    {
        // Clear all obstacles
        Obstacle[] obstacles = FindObjectsByType<Obstacle>(FindObjectsSortMode.None);
        foreach (var obstacle in obstacles) {
            Destroy(obstacle.gameObject);
        }
        
        // Clear all meteors
        MeteorMovement[] meteors = FindObjectsByType<MeteorMovement>(FindObjectsSortMode.None);
        foreach (var meteor in meteors) {
            Destroy(meteor.gameObject);
        }
        
        // Reset game state
        score = 0;
        gameSpeed = initialGameSpeed;
        isGameOver = false;
        enabled = true;
        
        // Hide game over UI
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);
        if (retryButton != null) retryButton.gameObject.SetActive(false);
        
        // Update UI
        if (scoreText != null) scoreText.text = "00000";
        UpdateHiscore();
        
        // Enable game objects
        if (player != null) player.gameObject.SetActive(true);
        if (spawner != null) spawner.gameObject.SetActive(true);
    }
    
    public void GameOver()
    {
        if (isGameOver) return; // Prevent multiple calls
        
        gameSpeed = 0f;
        isGameOver = true;
        
        // Update high score before stopping
        UpdateHiscore();
        
        // Show game over UI
        if (gameOverText != null) gameOverText.gameObject.SetActive(true);
        if (retryButton != null) retryButton.gameObject.SetActive(true);
        
        // Stop spawning new obstacles
        if (spawner != null) spawner.gameObject.SetActive(false);
        
        // Keep player visible but disabled (they stay in the scene)
        // Removed: if (player != null) player.gameObject.SetActive(false);
        
        Debug.Log("GAME OVER - Score: " + Mathf.FloorToInt(score));
    }
    
    private void Update()
    {
        // Only update game during active gameplay
        if (isGameOver) return;
        
        // Gradually increase speed
        gameSpeed += gameSpeedIncrease * Time.deltaTime;
        gameSpeed = Mathf.Min(gameSpeed, maxGameSpeed);
        
        // Update score
        score += gameSpeed * Time.deltaTime;
        
        // Update score display
        if (scoreText != null) {
            scoreText.text = Mathf.FloorToInt(score).ToString("D5");
        }
    }
    
    private void UpdateHiscore()
    {
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);
        
        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }
        
        if (hiscoreText != null) {
            hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");
        }
    }
    
    // Called by the Retry Button
    public void RestartGame()
    {
        // Reload scene for clean restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}