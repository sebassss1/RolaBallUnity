using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float maxSpeed = 40f;
    [SerializeField] private float speedIncreaseRate = 1f;
    [SerializeField] private float horizontalSpeed = 20f;
    [SerializeField] private float horizontalDamping = 5f;
    
    [Header("Boundaries")]
    [SerializeField] private float boundaryX = 4f;
    
    private Rigidbody rb;
    private float currentSpeed;
    private bool isGameOver = false;
    private float targetHorizontalVelocity = 0f;
    private float ballRadius = 0.5f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = moveSpeed;
        
        Collider col = GetComponent<Collider>();
        if (col != null) ballRadius = col.bounds.extents.y;
        
        // Optimized physics settings
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.None; // Permite que la bola rote
        rb.mass = 1f;
        rb.drag = 0f;
        rb.angularDrag = 0.5f;
    }
    
    void Update()
    {
        if (isGameOver) return;
        
        // Check if player has fallen off the track
        if (transform.position.y < -2f)
        {
            SetGameOver();
            GameManager.Instance?.GameOver();
            return;
        }
        
        // Increase speed over time
        currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);
        
        // Get horizontal input
        float horizontalInput = Input.GetAxis("Horizontal");
        targetHorizontalVelocity = horizontalInput * horizontalSpeed;
    }
    
    void FixedUpdate()
    {
        if (isGameOver) return;
        
        // Smooth horizontal movement with damping
        float currentHorizontalVelocity = rb.velocity.x;
        float newHorizontalVelocity = Mathf.Lerp(currentHorizontalVelocity, targetHorizontalVelocity, horizontalDamping * Time.fixedDeltaTime);
        
        // Apply velocity
        rb.velocity = new Vector3(newHorizontalVelocity, rb.velocity.y, currentSpeed);
        
        // Calcular y aplicar efecto de rodaje (rolling effect)
        Vector3 movementDir = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (movementDir.sqrMagnitude > 0.001f)
        {
            // El eje de rotación es perpendicular al movimiento
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, movementDir.normalized);
            float angularSpeed = movementDir.magnitude / ballRadius;
            rb.angularVelocity = rotationAxis * angularSpeed;
        }
        
        // Clamp position to boundaries only if walls are present
        bool hasWalls = Mathf.FloorToInt(transform.position.z / 750f) % 2 == 0;
        if (hasWalls)
        {
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -boundaryX, boundaryX);
            transform.position = clampedPosition;
        }
    }
    
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    public void SetGameOver()
    {
        isGameOver = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            SetGameOver();
            GameManager.Instance?.GameOver();
        }
    }
}
