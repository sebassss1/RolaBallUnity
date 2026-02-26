using UnityEngine;

/// <summary>
/// Enumeración de los vehículos disponibles para el jugador
/// </summary>
public enum VehicleType
{
    Ball,
    Car
}

/// <summary>
/// Singleton que guarda la selección de vehículo entre escenas
/// </summary>
public class VehicleSelector : MonoBehaviour
{
    public static VehicleSelector Instance { get; private set; }
    public VehicleType SelectedVehicle { get; set; } = VehicleType.Ball;

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
}
