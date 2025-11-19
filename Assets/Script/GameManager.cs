using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Speed Settings")]
    public float gameSpeed = 8f;           // Controls movement speed
    public float initialSpeed = 8f;        // Starting speed
    public float maxSpeed = 16f;           // Maximum speed
    
    [Header("Progression Settings")]
    public float progressionDuration = 240f;     // Time to reach max speed
    public float initialSlowdownDuration = 3f;   // Initial speed duration
    
    [Header("Gravity Settings")]
    public float initialGravity = 22f;     // Starting gravity 
    public float maxGravity = 38f;         // Maximum gravity 
    
    [Header("Parallax Settings")]
    public float initialParallaxMultiplier = 1f;  // Initial parallax speed
    public float maxParallaxMultiplier = 1.2f;    // Maximum parallax speed
    
    [Header("Game State")]
    public bool isGameOver = false;
    public bool isPaused = false;
    
    private float gameStartTime = 0f;
    public float GameDuration => Time.time - gameStartTime;
    
    public float CurrentGravity { get; private set; }
    public float CurrentParallaxMultiplier { get; private set; }
    public float ProgressionProgress { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameStartTime = Time.time;
        ResetGameValues();
    }

    private void Update()
    {
        if (isGameOver || isPaused) return;

        float gameDuration = GameDuration;
        
        if (gameDuration < initialSlowdownDuration)
        {
            ProgressionProgress = 0f;
            gameSpeed = initialSpeed * 0.8f;
            CurrentGravity = initialGravity * 0.9f;
            CurrentParallaxMultiplier = initialParallaxMultiplier * 0.9f;
        }
        else
        {
            float progressionTime = gameDuration - initialSlowdownDuration;
            ProgressionProgress = Mathf.Clamp01(progressionTime / progressionDuration);
            
            float easedProgress = EaseInOut(ProgressionProgress);
            
            gameSpeed = Mathf.Lerp(initialSpeed, maxSpeed, easedProgress);
            CurrentGravity = Mathf.Lerp(initialGravity, maxGravity, easedProgress);
            CurrentParallaxMultiplier = Mathf.Lerp(initialParallaxMultiplier, maxParallaxMultiplier, easedProgress);
        }
    }

    private float EaseInOut(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    private void ResetGameValues()
    {
        gameSpeed = initialSpeed * 0.8f;
        CurrentGravity = initialGravity * 0.9f;
        CurrentParallaxMultiplier = initialParallaxMultiplier * 0.9f;
        ProgressionProgress = 0f;
    }

    public void StartGame()
    {
        isGameOver = false;
        isPaused = false;
        gameStartTime = Time.time;
        ResetGameValues();
        Time.timeScale = 1f;
    }

    public void PauseGame() => isPaused = true;
    public void ResumeGame() => isPaused = false;

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
    }
}