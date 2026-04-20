using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        Vector3 movement = transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }
}