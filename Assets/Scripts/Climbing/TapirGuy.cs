using UnityEngine;
using System.Collections;

public class TapirGuy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite climbSprite;
    public Sprite missSprite;

    [SerializeField] private float bobAmount = 0.2f;
    public Vector3 originalPos;

    private void Start()
    {
        spriteRenderer.sprite = idleSprite;
        originalPos = transform.position;
    }

    public void Bob()
    {
        StartCoroutine(BobOnce());
    }

    public void Idle()
    {
        spriteRenderer.sprite = idleSprite;
    }

    public void Miss()
    {
        StartCoroutine(GettingHit());
    }

    private IEnumerator BobOnce()
    {
        transform.localPosition = originalPos + Vector3.down * bobAmount;
        yield return new WaitForSeconds(0.2f);
        transform.localPosition = originalPos;
        yield return null;
    }

    private IEnumerator GettingHit()
    {
        spriteRenderer.sprite = missSprite;
        yield return new WaitForSeconds(0.4f);
        spriteRenderer.sprite = idleSprite;
        yield return null;
    }
}
