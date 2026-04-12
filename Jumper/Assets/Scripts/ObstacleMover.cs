using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}