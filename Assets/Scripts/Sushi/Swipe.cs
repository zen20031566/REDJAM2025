using UnityEngine;

public class Swipe : MonoBehaviour
{
    public float swipeSpeed = 400f;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private Rigidbody2D rb;
    private bool hasSwiped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Detect swipe on mouse or touch
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && !hasSwiped)
        {
            endTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 swipeDir = (endTouchPos - startTouchPos).normalized;

            rb.linearVelocity = swipeDir * swipeSpeed;
            hasSwiped = true; // prevent multiple swipes
        }

        // Only trigger swipe if moved enough
        if ((endTouchPos - startTouchPos).magnitude > 0.3f)
        {
            Vector2 swipeDir = (endTouchPos - startTouchPos).normalized;
            rb.linearVelocity = swipeDir * swipeSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            Debug.Log("Swiped into target!");
            Destroy(gameObject); // Or setActive(false)
        }
    }
}

