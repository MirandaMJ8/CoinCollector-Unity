using UnityEngine;

// Ampliació: la moneda puja i baixa amb Mathf.PingPong.
public class CoinFloat : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.4f;
    [SerializeField] private float frequency = 0.8f;

    private float baseY;

    private void Start()
    {
        baseY = transform.position.y;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.y = baseY + Mathf.PingPong(Time.time * frequency, amplitude);
        transform.position = pos;
    }
}
