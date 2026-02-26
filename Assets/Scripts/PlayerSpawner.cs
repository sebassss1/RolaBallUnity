using UnityEngine;
using System.Reflection;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefabs de Vehiculos")]
    public GameObject ballPrefab;
    public GameObject carPrefab;

    [Header("Posicion de spawn")]
    public Vector3 spawnPosition = new Vector3(0, 0.5f, 0);

    [Header("Referencias opcionales")]
    public CameraController cameraController;
    public GameManager gameManager;

    public static GameObject SpawnedPlayer { get; private set; }

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        VehicleType selected = VehicleType.Ball;
        if (VehicleSelector.Instance != null)
            selected = VehicleSelector.Instance.SelectedVehicle;

        GameObject prefabToUse = (selected == VehicleType.Car) ? carPrefab : ballPrefab;

        if (prefabToUse == null)
        {
            Debug.LogError("[PlayerSpawner] Prefab no asignado para " + selected);
            return;
        }

        SpawnedPlayer = Instantiate(prefabToUse, spawnPosition, Quaternion.identity);
        SpawnedPlayer.name = (selected == VehicleType.Car) ? "Player_Car" : "Player_Ball";
        SpawnedPlayer.tag = "Player";

        Debug.Log("[PlayerSpawner] Vehiculo instanciado: " + SpawnedPlayer.name);

        // Usar coroutine para asignar referencias en el siguiente frame
        StartCoroutine(AssignReferencesNextFrame(selected));
    }

    private IEnumerator AssignReferencesNextFrame(VehicleType selected)
    {
        yield return null; // esperar 1 frame

        // ── Camara ──────────────────────────────────────────────
        CameraController cc = cameraController != null
            ? cameraController
            : FindObjectOfType<CameraController>();

        if (cc != null)
        {
            cc.player = SpawnedPlayer.transform;
            cc.offset = (selected == VehicleType.Car)
                ? new Vector3(0, 4f, -10f)
                : new Vector3(0, 5f, -8f);
            Debug.Log("[PlayerSpawner] Camara asignada a: " + SpawnedPlayer.name);
        }
        else
        {
            Debug.LogWarning("[PlayerSpawner] CameraController no encontrado en la escena.");
        }

        // ── CorridorManager ─────────────────────────────────────
        CorridorManager cm = FindObjectOfType<CorridorManager>();
        if (cm != null)
        {
            FieldInfo field = typeof(CorridorManager).GetField("player",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                field.SetValue(cm, SpawnedPlayer.transform);
        }

        // ── GameManager ─────────────────────────────────────────
        if (selected == VehicleType.Ball)
        {
            PlayerController pc = SpawnedPlayer.GetComponent<PlayerController>();
            if (pc != null)
            {
                GameManager gm = gameManager != null
                    ? gameManager
                    : FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    FieldInfo field = typeof(GameManager).GetField("player",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                        field.SetValue(gm, pc);
                }
            }
        }
    }
}
