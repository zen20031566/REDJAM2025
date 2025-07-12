using UnityEngine;

public class MoveSushiBelt : MonoBehaviour
{
    public float speed = 2f;
    public float resetX = -10f;  // Start from here
    public float endX = 10f;     // When to reset

    void Update()
    {
        // Move right
        transform.position += Vector3.right * speed * Time.deltaTime;

        // Reset to left when it goes too far
        if (transform.position.x > endX)
        {
            transform.position = new Vector3(resetX, transform.position.y, transform.position.z);
        }
    }
}

