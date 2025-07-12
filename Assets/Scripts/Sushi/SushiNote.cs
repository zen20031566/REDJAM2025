using UnityEngine;

public class SushiNote : Note
{
    public override void Setup(Conductor conductor, Transform spawnPoint, Transform hitPoint, NoteData noteData, float approachRate)
    {
        base.Setup(conductor, spawnPoint, hitPoint, noteData, approachRate);

        // You can also do custom spawn behavior here if needed
    }

    protected override void Update()
    {
        if (isInitialized)
        {
            float songTime = conductor.currentSongPosition;
            float timeRemaining = hitTiming - songTime;
            float normalizedTime = 1f - (timeRemaining / approachRate);
            // Don't clamp normalizedTime

            Vector3 rightMovement = new Vector3(normalizedTime * 2f, 0f, 0f);
            transform.position = spawnPoint.position + rightMovement;
        }
    }
}


