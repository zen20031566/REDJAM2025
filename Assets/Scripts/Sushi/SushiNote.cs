using UnityEngine;

public class SushiNote : Note
{
    private bool hasBeenHit = false;
    private float speed;

    public bool HasBeenHit => hasBeenHit;

    public override void Setup(Conductor conductor, Transform spawnPoint, Transform hitPoint, NoteData noteData, float approachRate)
    {
        base.Setup(conductor, spawnPoint, hitPoint, noteData, approachRate);
        speed = Vector3.Distance(spawnPoint.position, hitPoint.position) / approachRate;
    }

    protected override void Update()
    {
        if (!isInitialized || hasBeenHit) return;

        float songTime = conductor.currentSongPosition;
        float timeSinceSpawn = songTime - (hitTiming - approachRate);
        float distance = timeSinceSpawn * speed;

        transform.position = spawnPoint.position + new Vector3(distance, 0f, 0f);
    }

    public void MarkAsHit()
    {
        hasBeenHit = true;
        Destroy(gameObject); // or play hit animation
    }

    public void MarkAsMissed()
    {
        hasBeenHit = true;
        Destroy(gameObject); // or play miss animation
    }
}



