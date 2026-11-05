using UnityEngine;

// Controla la bola del jugador amb física.
// L'input es llegeix a Update i la velocitat s'aplica a FixedUpdate.
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 6f;

    private Rigidbody body;
    private float inputX;
    private float inputZ;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        // ClampMagnitude perquè en diagonal no vagi més ràpid
        Vector3 move = Vector3.ClampMagnitude(new Vector3(inputX, 0f, inputZ), 1f) * speed;

        // Es manté la Y actual perquè la gravetat continuï actuant
        body.linearVelocity = new Vector3(move.x, body.linearVelocity.y, move.z);
    }
}
