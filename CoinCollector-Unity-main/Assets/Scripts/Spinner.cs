using UnityEngine;

// Ampliació: fa girar la moneda sobre l'eix Y.
public class Spinner : MonoBehaviour
{
    [SerializeField] private float degreesPerSecond = 120f;

    private void Update()
    {
        transform.Rotate(Vector3.up * degreesPerSecond * Time.deltaTime, Space.World);
    }
}
