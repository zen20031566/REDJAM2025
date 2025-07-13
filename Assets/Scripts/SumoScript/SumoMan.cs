using UnityEngine;
using System.Collections;

public class SumoMan : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite pushSprite;
    public Sprite legUpSprite;
    public Sprite stompSprite;
    public Sprite shoutSprite;

    [SerializeField] private float bobAmount = 0.2f;
    public Vector3 originalPos;

    [SerializeField] AudioSource audioSource;
    [SerializeField] private AudioClip shoutSound;

    private void Start()
    {
        spriteRenderer.sprite = idleSprite;
        originalPos = transform.position;
    }

    public void Bob()
    {
        StartCoroutine(BobOnce());
    }

    public void Shout()
    {
        audioSource.PlayOneShot(shoutSound);
        spriteRenderer.sprite = shoutSprite;
    }

    public void LegUp()
    {
        spriteRenderer.sprite = legUpSprite;
    }

    public void Stomp()
    {
        StartCoroutine(StompOnce());
    }

    public void Push()
    {
        StartCoroutine(PushOnce());
    }

    public void Idle()
    {
        spriteRenderer.sprite = idleSprite;
    }

    private IEnumerator BobOnce()
    {
        transform.localPosition = originalPos + Vector3.down * bobAmount;
        yield return new WaitForSeconds(0.2f);
        transform.localPosition = originalPos;
        yield return null;
    }

    private IEnumerator PushOnce()
    {
        spriteRenderer.sprite = pushSprite;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.sprite = idleSprite;
        yield return null;
    }

    private IEnumerator StompOnce()
    {
        spriteRenderer.sprite = stompSprite;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.sprite = idleSprite;
        yield return null;
    }
}
