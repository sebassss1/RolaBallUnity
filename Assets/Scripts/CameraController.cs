using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 5, -8);
    public float smoothSpeed = 0.1f;
    public bool lookAtPlayer = true;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (player == null)
            StartCoroutine(FindPlayerDelayed());
        else
            Debug.Log("CameraController: Siguiendo a " + player.name);
    }

    private IEnumerator FindPlayerDelayed()
    {
        // Esperar 2 frames para que PlayerSpawner cree el jugador
        yield return null;
        yield return null;

        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO.transform;
            Debug.Log("CameraController: Player encontrado automaticamente -> " + player.name);
        }
        else
        {
            Debug.LogWarning("CameraController: No se encontro ningun objeto con tag 'Player'.");
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        if (lookAtPlayer)
        {
            Vector3 lookTarget = player.position + Vector3.up * 0.5f;
            transform.LookAt(lookTarget);
        }
    }
}
