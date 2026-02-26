using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI speedText;
    
    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalDistanceText;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private Button restartButton;
    
    void Start()
    {
        // Hide game over panel at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Setup restart button
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
    }
    
    public void UpdateHUD(int score, int distance, float time, float speed)
    {
        if (scoreText != null)
        {
            scoreText.text = $"PUNTUACIÓN: {score}";
        }
        
        if (distanceText != null)
        {
            distanceText.text = $"DISTANCIA: {distance}m";
        }
        
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            timeText.text = $"TIEMPO: {minutes:00}:{seconds:00}";
        }
        
        if (speedText != null)
        {
            speedText.text = $"VELOCIDAD: {speed:F1}";
        }
    }
    
    public void ShowGameOver(int finalScore, int finalDistance, float finalTime)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (finalScoreText != null)
        {
            finalScoreText.text = $"PUNTUACIÓN FINAL: {finalScore}";
        }
        
        if (finalDistanceText != null)
        {
            finalDistanceText.text = $"DISTANCIA: {finalDistance} metros";
        }
        
        if (finalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(finalTime / 60f);
            int seconds = Mathf.FloorToInt(finalTime % 60f);
            finalTimeText.text = $"TIEMPO: {minutes:00}:{seconds:00}";
        }
    }
    
    void OnRestartClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}
