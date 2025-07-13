using DG.Tweening;
using UnityEngine;

public class SushiNote : Note
{
    public SpriteRenderer sushiSpriteRenderer;
    [SerializeField] SpriteRenderer plateSpriteRenderer;
    [SerializeField] Sprite[] sushiSprites;
    [SerializeField] Sprite[] badfoodSprites;
    [SerializeField] Sprite[] plateSprites;
    [SerializeField] Transform sushiTransform;
    public bool bad = false;
    public void Setup(Conductor conductor, Transform spawnPoint, Transform hitPoint, NoteData noteData, float approachRate, bool bad = false)
    {
        base.Setup(conductor, spawnPoint, hitPoint, noteData, approachRate);

        plateSpriteRenderer.sprite = plateSprites[Random.Range(0, plateSprites.Length)];

        if (bad)
        {
            sushiSpriteRenderer.sprite = badfoodSprites[Random.Range(0, badfoodSprites.Length)];
        }
        else
        {
            sushiSpriteRenderer.sprite = sushiSprites[Random.Range(0, sushiSprites.Length)];
        }
    }

    protected override void Update()
    {
            float songTime = conductor.currentSongPosition;
            float timeRemaining = hitTiming - songTime;
            float normalizedTime = 1f - (timeRemaining / approachRate);
            //normalizedTime = Mathf.Clamp01(normalizedTime); //clamps between 0 and 1

            transform.position = Vector3.LerpUnclamped(spawnPoint.position, hitPoint.position, normalizedTime);
    }

    public void MoveSushi(float moveAmount)
    {
        transform.position += Vector3.right * moveAmount;
    }

    public void FlickDown(float distance = 5f, float duration = 0.4f)
    {
        sushiTransform.SetParent(null, true);
        sushiTransform.DOMoveY(transform.position.y - distance, duration).SetEase(Ease.InBack);
        sushiTransform.DORotate(new Vector3(0, 0, Random.Range(-90f, 90f)), duration, RotateMode.FastBeyond360).OnComplete(() => Destroy(sushiTransform.gameObject)); 
        //sushiTransform.DOScale(0f, duration).OnComplete(() => Destroy(sushiTransform.gameObject));
    }

    public void FlickUp(float distance = 5f, float duration = 0.4f)
    {
        sushiTransform.SetParent(null, true);
        sushiTransform.DOMoveY(transform.position.y + distance, duration).SetEase(Ease.OutBack);
        sushiTransform.DORotate(new Vector3(0, 0, Random.Range(-90f, 90f)), duration, RotateMode.FastBeyond360).OnComplete(() => Destroy(sushiTransform.gameObject)); 
        //sushiTransform.DOScale(0f, duration).OnComplete(() => Destroy(sushiTransform.gameObject));
    }
}





