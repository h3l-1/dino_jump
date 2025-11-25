using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
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
        
        gameSpeed = initialGameSpeed;
        isGameOver = false;
        enabled = true;
        
        if (player != null) player.gameObject.SetActive(true);
        if (spawner != null) spawner.gameObject.SetActive(true);
    }
    
    public void GameOver()
    {
        if (isGameOver) return; // Prevent multiple calls
        
        gameSpeed = 0f;
        isGameOver = true;
        enabled = false;
        
        if (player != null) player.gameObject.SetActive(false);
        if (spawner != null) spawner.gameObject.SetActive(false);
        
        Debug.Log("GAME OVER");
    }
    
    private void Update()
    {
        // Gradually increase speed
        gameSpeed += gameSpeedIncrease * Time.deltaTime;
        gameSpeed = Mathf.Min(gameSpeed, maxGameSpeed);
    }
}