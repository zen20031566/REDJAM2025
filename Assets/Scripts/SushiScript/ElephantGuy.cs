using UnityEngine;
using System.Collections;

public class ElephantGuy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite eatSprite;
    public Sprite crySprite;
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
        StartCoroutine(StartBadFood());
    }

    public void Cry()
    {
        StartCoroutine(StartCry());
    }

    public void Eat()
    {
        StartCoroutine(StartEat());
    }
    private IEnumerator BobOnce()
    {
        transform.localPosition = originalPos + Vector3.down * bobAmount;
        yield return new WaitForSeconds(0.2f);
        transform.localPosition = originalPos;
        yield return null;
    }

    private IEnumerator StartBadFood()
    {
        spriteRenderer.sprite = missSprite;
        yield return new WaitForSeconds(0.4f);
        spriteRenderer.sprite = idleSprite;
        yield return null;
    }

    private IEnumerator StartCry()
    {
        spriteRenderer.sprite = crySprite;
        yield return new WaitForSeconds(0.4f);
        spriteRenderer.sprite = idleSprite;
        yield return null;
    }

    private IEnumerator StartEat()
    {
        spriteRenderer.sprite = eatSprite;
        yield return new WaitForSeconds(0.4f);
        spriteRenderer.sprite = idleSprite;
        yield return null;
    }

}
