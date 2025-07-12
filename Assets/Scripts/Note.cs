using UnityEngine;

public class Note : MonoBehaviour
{
    public NoteType type;
    protected Conductor conductor;
    protected NoteManager noteManager;
    protected Transform spawnPoint;
    protected Transform hitPoint;
    protected bool isInitialized = false;
    public float hitTiming;

    public virtual void Setup(Conductor conductor, NoteManager noteManager, Transform spawnPoint, Transform hitPoint, NoteData noteData)
    {
        this.conductor = conductor;
        this.noteManager = noteManager;
        this.spawnPoint = spawnPoint; //where note spawns
        this.hitPoint = hitPoint; //where note gets killed
        transform.position = spawnPoint.position;

        this.hitTiming = noteData.hitTiming;
        type = noteData.type;

        isInitialized = true;
    }

    protected virtual void Update()
    {
        if (isInitialized)
        {
            float songTime = conductor.currentSongPosition;
            float timeRemaining = hitTiming - songTime;
            float normalizedTime = 1f - (timeRemaining / noteManager.approachRate);
            normalizedTime = Mathf.Clamp01(normalizedTime); //clamps between 0 and 1

            transform.position = Vector3.Lerp(spawnPoint.position, hitPoint.position, normalizedTime); //lerp expects a normalize time but i think use do tween later
        }
    }
}

