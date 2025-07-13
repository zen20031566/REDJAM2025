using UnityEngine;

public class SushiNote : Note
{
    private bool hasBeenHit = false;
    private bool wasMissed = false;

    public override bool HasBeenHit => hasBeenHit;

    private float speed;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] possibleSprites; // Assign these in Inspector

    public override void Setup(Conductor conductor, Transform spawnPoint, Transform hitPoint, NoteData noteData, float approachRate)
    {
        base.Setup(conductor, spawnPoint, hitPoint, noteData, approachRate);
        speed = Vector3.Distance(spawnPoint.position, hitPoint.position) / approachRate;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Randomize the sprite on spawn
        if (possibleSprites != null && possibleSprites.Length > 0)
        {
            spriteRenderer.sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
        }
    }

    protected override void Update()
    {
        if (!isInitialized && !wasMissed) return;

        float songTime = conductor.currentSongPosition;
        float timeSinceSpawn = songTime - (hitTiming - approachRate);
        float distance = timeSinceSpawn * speed;

        transform.position = spawnPoint.position + new Vector3(distance, 0f, 0f);
    }

    public override void MarkAsMissed()
    {
        wasMissed = true;
        hasBeenHit = true;
        Destroy(gameObject, 2.5f);
    }

    public void MarkAsHit()
    {
        hasBeenHit = true;
        Destroy(gameObject);
    }
}
