using UnityEngine;

// Moneda. Quan el jugador la toca suma punts i desapareix.
public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(value);
            Destroy(gameObject);
        }
    }
}
