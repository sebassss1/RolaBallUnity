using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private Material hitMaterial;
    private Material originalMaterial;
    private Renderer obstacleRenderer;
    private bool hasCollided = false;
    
    void Start()
    {
        obstacleRenderer = GetComponent<Renderer>();
        if (obstacleRenderer != null)
        {
            originalMaterial = obstacleRenderer.material;
        }
        
        // Add subtle rotation for visual interest
        float randomRotationSpeed = Random.Range(10f, 30f);
        Vector3 randomAxis = Random.insideUnitSphere.normalized;
        StartCoroutine(RotateObstacle(randomAxis, randomRotationSpeed));
    }
    
    System.Collections.IEnumerator RotateObstacle(Vector3 axis, float speed)
    {
        while (!hasCollided)
        {
            transform.Rotate(axis, speed * Time.deltaTime);
            yield return null;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasCollided)
        {
            hasCollided = true;
            
            // Visual feedback
            if (obstacleRenderer != null && hitMaterial != null)
            {
                obstacleRenderer.material = hitMaterial;
            }
            
            // Camera shake
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(0.3f, 0.5f);
            }
            
            // Create particle effect
            CreateImpactEffect(collision.contacts[0].point);
        }
    }
    
    void CreateImpactEffect(Vector3 position)
    {
        GameObject particleObj = new GameObject("ImpactParticles");
        particleObj.transform.position = position;
        
        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = new Color(1f, 0.3f, 0f);
        main.startSize = 0.2f;
        main.startSpeed = 5f;
        main.startLifetime = 0.5f;
        main.maxParticles = 20;
        
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });
        
        Destroy(particleObj, 1f);
    }
    
    void OnBecameInvisible()
    {
        if (Camera.main != null && transform.position.z < Camera.main.transform.position.z - 20f)
        {
            Destroy(gameObject);
        }
    }
}
