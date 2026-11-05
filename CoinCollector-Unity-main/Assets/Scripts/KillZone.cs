using UnityEngine;

// Zona sota el nivell: si el jugador hi cau es reinicia l'escena.
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Player killed!");
        GameManager.Instance.RestartLevel();
    }
}
