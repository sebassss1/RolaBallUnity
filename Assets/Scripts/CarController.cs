using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movimiento (Auto-Drive)")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float maxSpeed = 40f;
    [SerializeField] private float speedIncreaseRate = 1f;

    [Header("Fisica del coche")]
    public float horizontalStrafeSpeed = 20f; // Velocidad de desplazamiento lateral
    public float horizontalDamping = 5f;

    [Header("Visuales (Ruedas y Chasis)")]
    [Tooltip("Maximo angulo de giro visual de las ruedas delanteras")]
    public float maxWheelTurnAngle = 30f;
    [Tooltip("Inclinacion del chasis al girar")]
    public float bodyTiltAngle = 5f;
    private Transform[] frontWheels;
    private Transform[] backWheels;
    private Transform carBody;
    
    [Header("Estado (solo lectura)")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private bool isGrounded;
    private bool isGameOver = false;

    private Rigidbody _rb;
    private float _steerInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.mass = 1.5f;
        
        // Removemos el drag para que no se frene al dar pequeños saltitos en las uniones
        _rb.drag = 0f;
        _rb.angularDrag = 0.5f;
        
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rb.centerOfMass = new Vector3(0, -0.4f, 0);
        
        // Activar interpolación para suavizar el movimiento de cámara y eliminar el "lageo" visual
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // PARCHE DEFINITIVO MODO ENDLESS RUNNER:
        // Como el suelo son bloques continuos, físicamente a 200km/h cualquier pequeño rebote 
        // de colisión puede lanzar el coche. Vamos a anular su gravedad y movimiento Y 
        // a través de físicas (velocity.y = 0) para que NUNCA salte por las uniones.
        _rb.useGravity = false;
    }

    private void Start()
    {
        currentSpeed = moveSpeed;
        
        // Buscar ruedas y chasis dinamicamente si no están asignados
        FindVisualComponents();
    }
    
    private void FindVisualComponents()
    {
        // Buscar mallas/hijos para rotarlos visualmente
        var wheelsListF = new System.Collections.Generic.List<Transform>();
        var wheelsListB = new System.Collections.Generic.List<Transform>();
        
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            string colName = child.name.ToLower();
            if (colName.Contains("wheel") || colName.Contains("rueda") || colName.Contains("tire"))
            {
                // Si esta en la mitad delantera (local Z > 0), es delantera
                if (child.localPosition.z > 0) wheelsListF.Add(child);
                else wheelsListB.Add(child);
            }
            else if (child != transform && carBody == null && child.GetComponent<MeshRenderer>() != null)
            {
                // Asumir que el primer hijo con MeshRenderer grande es el chasis principal
                carBody = child;
            }
        }
        
        frontWheels = wheelsListF.ToArray();
        backWheels = wheelsListB.ToArray();
        
        // Tratar de buscar un nodo "Body" o "Chassis" especificamente
        Transform bodyNode = transform.Find("Body");
        if (bodyNode == null) bodyNode = transform.Find("Chassis");
        if (bodyNode != null) carBody = bodyNode;
    }

    private void Update()
    {
        if (isGameOver) return;
        
        // Check if player has fallen off the track
        if (transform.position.y < -2f)
        {
            SetGameOver();
            GameManager.Instance?.GameOver();
            return;
        }
        
        // Auto-accelerate just like the ball
        currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);
        
        _steerInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (isGameOver) return;
        
        ApplyMovement();
        ClampPosition();
        
        AnimateVisuals();
    }

    private void ApplyMovement()
    {
        // Smooth horizontal movement with damping (strafe)
        float currentHorizontalVelocity = _rb.velocity.x;
        float targetHorizontalVelocity = _steerInput * horizontalStrafeSpeed;
            
        float newHorizontalVelocity = Mathf.Lerp(currentHorizontalVelocity, targetHorizontalVelocity, horizontalDamping * Time.fixedDeltaTime);
        
        // Definimos la velocidad Y a 0 para que no salte.
        float currentYVelocity = 0f;

        // Si ya superó el borde de la pista (boundaries sin paredes), dejamos caer el coche
        if (Mathf.Abs(transform.position.x) > 4.5f)
        {
            _rb.useGravity = true;
            currentYVelocity = _rb.velocity.y; // Dejamos que caiga
        }
        else
        {
            // Forzamos la altura a 0.5f (o la inicial) para que siempre esté pegado al suelo
            Vector3 pos = transform.position;
            pos.y = 0.5f;
            transform.position = pos;
        }
        
        // Apply velocity: X for strafing, Y frozen unless falling, Z for continuous auto-drive
        _rb.velocity = new Vector3(newHorizontalVelocity, currentYVelocity, currentSpeed);

        // Asegurar que el rigidbody JAMAS rote en Y para evitar tirones y peleas con la camara
        _rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity; 
    }

    private void AnimateVisuals()
    {
        // 1. Girar ruedas delanteras
        float targetWheelAngle = _steerInput * maxWheelTurnAngle;
        
        foreach (Transform wheel in frontWheels)
        {
            if (wheel == null) continue;
            // Solo rotar en el eje Y local
            Vector3 euler = wheel.localEulerAngles;
            euler.y = Mathf.LerpAngle(euler.y, targetWheelAngle, Time.fixedDeltaTime * 10f);
            
            // Simular rotacion de rodad (eje X)
            euler.x += (_rb.velocity.z / 0.5f) * Mathf.Rad2Deg * Time.fixedDeltaTime;
            
            wheel.localEulerAngles = euler;
        }
        
        // Ruedas traseras (solo ruedan, no giran)
        foreach (Transform wheel in backWheels)
        {
            if (wheel == null) continue;
            Vector3 euler = wheel.localEulerAngles;
            euler.x += (_rb.velocity.z / 0.5f) * Mathf.Rad2Deg * Time.fixedDeltaTime;
            wheel.localEulerAngles = euler;
        }

        // 2. Inclinar levemente el chasis (Body Tilt)
        if (carBody != null)
        {
            // Inclinacion lateral basada en el giro (roll)
            float targetRoll = -_steerInput * bodyTiltAngle;
            
            Quaternion targetRot = Quaternion.Euler(0, 0, targetRoll);
            carBody.localRotation = Quaternion.Slerp(carBody.localRotation, targetRot, Time.fixedDeltaTime * 5f);
        }
    }



    private void ClampPosition()
    {
        // Clamp position to boundaries only if walls are present
        float boundaryX = 4f; // Mismo boundary que la bola
        bool hasWalls = Mathf.FloorToInt(transform.position.z / 750f) % 2 == 0;
        if (hasWalls)
        {
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -boundaryX, boundaryX);
            transform.position = clampedPosition;
        }
    }

    public void SetGameOver()
    {
        isGameOver = true;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            SetGameOver();
            GameManager.Instance?.GameOver();
        }
    }

    public float GetSpeed() => currentSpeed;
}
