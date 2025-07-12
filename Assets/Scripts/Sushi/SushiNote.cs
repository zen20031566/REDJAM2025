using UnityEngine;

public class SushiNote : Note
{
    private bool hasBeenHit = false;
    private bool wasMissed = false;

    // Allow SushiScript to check hit status through base class
    public override bool HasBeenHit => hasBeenHit;

    private float speed;

    public override void Setup(Conductor conductor, Transform spawnPoint, Transform hitPoint, NoteData noteData, float approachRate)
    {
        base.Setup(conductor, spawnPoint, hitPoint, noteData, approachRate);
        speed = Vector3.Distance(spawnPoint.position, hitPoint.position) / approachRate;
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
        Destroy(gameObject, 2.0f); // Give it 2 seconds to pass beyond hit point
    }

    public void MarkAsHit()
    {
        hasBeenHit = true;
        Destroy(gameObject); // Instantly destroy on hit
    }
}



