using UnityEngine;
using System.Collections;

public class Swipe : MonoBehaviour
{
    public float swipeSpeed = 400f;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private Rigidbody2D rb;
    private bool hasSwiped = false;

    public GameObject reactionObject; // Show this temporarily
    public GameObject mainObject;     // Turn this back on after 1 sec

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && !hasSwiped)
        {
            endTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 swipeDir = (endTouchPos - startTouchPos).normalized;

            rb.linearVelocity = swipeDir * swipeSpeed;
            hasSwiped = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            if (reactionObject != null)
            {
                StartCoroutine(ShowReaction());
            }
        }
    }

    IEnumerator ShowReaction()
    {
        // Hide the main object and show the reaction
        mainObject.SetActive(false);
        reactionObject.SetActive(true);

        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Revert back
        reactionObject.SetActive(false);
        mainObject.SetActive(true);

        // Now destroy this swiped object
        Destroy(gameObject);
    }
}


