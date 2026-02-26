using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    
    private Vector3 originalPosition;
    private float shakeAmount = 0f;
    private float decreaseFactor = 1.0f;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        originalPosition = transform.localPosition;
    }
    
    void LateUpdate()
    {
        if (shakeAmount > 0)
        {
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;
            shakeAmount -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeAmount = 0f;
            transform.localPosition = originalPosition;
        }
    }
    
    public void Shake(float amount, float duration)
    {
        shakeAmount = amount;
        decreaseFactor = amount / duration;
    }
}
