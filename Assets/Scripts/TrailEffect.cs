using UnityEngine;

public class TrailEffect : MonoBehaviour
{
    private TrailRenderer trail;
    
    void Start()
    {
        // Add trail renderer
        trail = gameObject.AddComponent<TrailRenderer>();
        
        // Configure trail
        trail.time = 0.5f;
        trail.startWidth = 0.3f;
        trail.endWidth = 0.05f;
        trail.minVertexDistance = 0.1f;
        
        // Create gradient
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(0f, 0.8f, 1f, 1f), 0.0f),
                new GradientColorKey(new Color(0f, 0.4f, 1f, 1f), 1.0f)
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        trail.colorGradient = gradient;
        
        // Material settings
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.material.color = Color.cyan;
    }
}
