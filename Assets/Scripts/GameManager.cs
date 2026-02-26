using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    private bool isGameOver = false;
    private float gameTime = 0f;
    private float distanceTraveled = 0f;
    private int score = 0;

    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private UIManager uiManager;

    [Header("Difficulty Settings")]
    [SerializeField] private float difficultyIncreaseRate = 1f;
    private float currentDifficulty = 1f;

    [Header("Skybox Settings")]
    [SerializeField] private Material normalSkybox;
    [SerializeField] private Material darkSkybox;
    private bool isCurrentlyDark = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource gameOverMusic;

    // Referencia al coche (alternativa a PlayerController)
    private CarController carController;
    // Transform generico del jugador activo
    private Transform playerTransform;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        gameTime = 0f;
        score = 0;
        currentDifficulty = 1f;
        isCurrentlyDark = false;
        
        if (normalSkybox != null)
        {
            RenderSettings.skybox = normalSkybox;
            DynamicGI.UpdateEnvironment();
        }

        // Start background music, ensure Game Over music is stopped
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
        if (gameOverMusic != null && gameOverMusic.isPlaying)
        {
            gameOverMusic.Stop();
        }

        // Buscar el jugador automaticamente si no esta asignado
        StartCoroutine(FindPlayerDelayed());
    }

    // Esperamos un frame para que el PlayerSpawner haya creado el jugador
    private System.Collections.IEnumerator FindPlayerDelayed()
    {
        yield return null; // esperar 1 frame

        if (player == null)
        {
            // Intentar encontrar la bola
            GameObject playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
            {
                player = playerGO.GetComponent<PlayerController>();
                carController = playerGO.GetComponent<CarController>();
                playerTransform = playerGO.transform;
                Debug.Log("[GameManager] Jugador encontrado automaticamente: " + playerGO.name);
            }
        }
        else
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (isGameOver) return;

        gameTime += Time.deltaTime;
        currentDifficulty = 1f + (gameTime * difficultyIncreaseRate / 60f);

        // Actualizar distancia con cualquier tipo de jugador
        if (playerTransform != null)
            distanceTraveled = playerTransform.position.z;
        else if (player != null)
            distanceTraveled = player.transform.position.z;

        score = Mathf.FloorToInt(distanceTraveled + (gameTime * 10f));

        // Obtener velocidad del vehiculo activo
        float speed = GetActiveSpeed();

        // Change Skybox based on distance (every 750m)
        bool shouldBeDark = Mathf.FloorToInt(distanceTraveled / 750f) % 2 != 0;
        if (shouldBeDark != isCurrentlyDark)
        {
            isCurrentlyDark = shouldBeDark;
            Material skyboxToSet = isCurrentlyDark ? darkSkybox : normalSkybox;
            
            if (skyboxToSet != null)
            {
                RenderSettings.skybox = skyboxToSet;
                DynamicGI.UpdateEnvironment(); // IMPORTANT: updates ambient lighting based on new skybox
            }
        }

        if (uiManager != null)
            uiManager.UpdateHUD(score, Mathf.FloorToInt(distanceTraveled), gameTime, speed);
    }

    private float GetActiveSpeed()
    {
        // Si es coche
        if (carController != null)
            return carController.GetSpeed();

        // Si es bola
        if (player != null)
            return player.GetCurrentSpeed();

        // Buscar en la escena si aun no tenemos referencia
        if (playerTransform == null)
        {
            GameObject pg = GameObject.FindWithTag("Player");
            if (pg != null)
            {
                playerTransform = pg.transform;
                player = pg.GetComponent<PlayerController>();
                carController = pg.GetComponent<CarController>();
            }
        }

        return 0f;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        // Stop background music and play game over music
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }
        if (gameOverMusic != null && !gameOverMusic.isPlaying)
        {
            gameOverMusic.Play();
        }

        if (uiManager != null)
            uiManager.ShowGameOver(score, Mathf.FloorToInt(distanceTraveled), gameTime);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public float GetCurrentDifficulty() => currentDifficulty;
    public bool IsGameOver() => isGameOver;
}
