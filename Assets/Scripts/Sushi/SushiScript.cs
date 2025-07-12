using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class SushiScript : MonoBehaviour
{
    // Song setup
    public Conductor conductor;
    private float currentSongPosition;
    [SerializeField] private int bpm;
    [SerializeField] private AudioClip song;
    public List<NoteData> noteDataList = new();

    // Scoring
    [SerializeField] private float perfectWindow = 0.1f;
    //[SerializeField] private float goodWindow = 0.25f; // Optional
    [SerializeField] private float missWindow = 0.5f;
    [SerializeField] private TouchManager touchManager;

    // Note spawning
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private SushiNote tapNotePrefab;
    public float approachRate = 1f;
    public List<Note> activeNotesList = new();

    // UI
    [SerializeField] private TMP_Text scoreText;

    // Player visuals
    [SerializeField] private Transform player;
    [SerializeField] private Sprite playerSprite1;
    [SerializeField] private Sprite playerSprite2;
    private SpriteRenderer spriteRenderer;
    Vector3 originalPos;
    [SerializeField] private float bobAmount = 0.2f;

    // Conveyor belts
    public Transform belt1;
    public Transform belt2;
    public float moveAmount = 1f;
    public float beltWidth;
    private int spawnIndex = 0;
    float hittiming = 0f;

    private float nextHalfBeat;
    private float nextFullBeat;

    private void Start()
    {
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        beltWidth = belt1.GetComponent<SpriteRenderer>().bounds.size.x;
        conductor.Setup(song, bpm);

        originalPos = player.localPosition;

        for (int i = 0; i < 100; i++)
        {
            hittiming += conductor.crotchet;

            noteDataList.Add(new NoteData
            {
                type = NoteType.SwipeUp,
                hitTiming = hittiming,
            });
        }
    }

    private void OnEnable()
    {
        touchManager.OnTap += TouchManager_OnScreenTouched;
        touchManager.OnTapReleased += TouchManager_OnScreenReleased;
        touchManager.OnSwipeUp += TouchManager_OnSwipeUp;
        touchManager.OnSwipeDown += TouchManager_OnSwipeDown;
    }

    private void OnDisable()
    {
        touchManager.OnTap -= TouchManager_OnScreenTouched;
        touchManager.OnTapReleased -= TouchManager_OnScreenReleased;
        touchManager.OnSwipeUp -= TouchManager_OnSwipeUp;
        touchManager.OnSwipeDown -= TouchManager_OnSwipeDown;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            SceneManager.LoadScene("ZenYoungTest");

        if (conductor.currentSongPosition > nextHalfBeat + conductor.crotchet)
        {
            nextHalfBeat += conductor.crotchet / 2f;
            StartCoroutine(BobOnce());
        }

        if (conductor.currentSongPosition > nextFullBeat + conductor.crotchet)
        {
            nextFullBeat += conductor.crotchet;

            belt1.position += Vector3.right * moveAmount;
            belt2.position += Vector3.right * moveAmount;

            if (belt1.position.x >= beltWidth)
                belt1.position = new Vector3(belt2.position.x - beltWidth, belt1.position.y, belt1.position.z);

            if (belt2.position.x >= beltWidth)
                belt2.position = new Vector3(belt1.position.x - beltWidth, belt2.position.y, belt2.position.z);
        }

        ClearInactiveNotes();

        currentSongPosition = conductor.currentSongPosition;

        while (spawnIndex < noteDataList.Count && noteDataList[spawnIndex].hitTiming <= currentSongPosition + approachRate)
        {
            SpawnNote(noteDataList[spawnIndex]);
            spawnIndex++;
        }
    }

    private IEnumerator BobOnce()
    {
        player.localPosition = originalPos + Vector3.down * bobAmount;
        yield return new WaitForSeconds(0.2f);
        player.localPosition = originalPos;
    }

    public void SpawnNote(NoteData noteData)
    {
        SushiNote prefab = null;

        if (noteData.type == NoteType.Tap || noteData.type == NoteType.SwipeUp)
        {
            prefab = tapNotePrefab;
        }

        if (prefab != null)
        {
            SushiNote note = Instantiate(prefab);
            note.Setup(conductor, spawnPoint, hitPoint, noteData, approachRate);
            activeNotesList.Add(note);
        }
    }

    private void ClearInactiveNotes()
    {
        for (int i = activeNotesList.Count - 1; i >= 0; i--)
        {
            Note note = activeNotesList[i];
            if (note == null)
            {
                activeNotesList.RemoveAt(i);
                continue;
            }

            float timeSinceNote = currentSongPosition - note.hitTiming;

            // Only auto-miss if note has passed hit timing + missWindow
            if (!note.HasBeenHit && timeSinceNote > missWindow)
            {
                Debug.Log("AUTO MISS");

                scoreText.color = Color.red;
                scoreText.text = "MISS";

                // DO NOT destroy here directly
                note.MarkAsMissed(); // Let it continue its movement first
                activeNotesList.RemoveAt(i);
            }
        }
    }

    public Note GetClosestNote(NoteType type)
    {
        Note closestNote = null;
        float closestAbsOffset = float.MaxValue;

        foreach (var note in activeNotesList)
        {
            if (note.type != type) continue;

            float offset = currentSongPosition - note.hitTiming;
            float absOffset = Mathf.Abs(offset);

            if (absOffset <= perfectWindow && absOffset < closestAbsOffset)
            {
                closestAbsOffset = absOffset;
                closestNote = note;
            }
        }

        return closestNote;
    }

    public void CheckNoteHitTiming(NoteType type)
    {
        Note closestNote = GetClosestNote(type);
        if (closestNote == null) return;

        float offset = currentSongPosition - closestNote.hitTiming;
        float absOffset = Mathf.Abs(offset);

        if (absOffset <= perfectWindow)
        {
            Debug.Log("PERFECT");
            scoreText.color = Color.green;
            scoreText.text = "PERFECT";
        }
        /* else if (absOffset <= goodWindow) // Optional
        {
            Debug.Log("GOOD");
            scoreText.color = Color.yellow;
            scoreText.text = "GOOD";
        } */
        else
        {
            Debug.Log("MISS");
            scoreText.color = Color.red;
            scoreText.text = "MISS";
        }

        Destroy(closestNote.gameObject);
        activeNotesList.Remove(closestNote);
    }

    // Touch Input Events
    private void TouchManager_OnScreenTouched(object sender, System.EventArgs e)
    {
        Debug.Log("Touch detected");
        if (!touchManager.swipeJustDetected)
            CheckNoteHitTiming(NoteType.Tap);
    }

    private void TouchManager_OnScreenReleased(object sender, System.EventArgs e) { }

    private void TouchManager_OnSwipeUp(object sender, System.EventArgs e)
    {
        CheckNoteHitTiming(NoteType.SwipeUp);
    }

    private void TouchManager_OnSwipeDown(object sender, System.EventArgs e)
    {
        CheckNoteHitTiming(NoteType.SwipeDown);
    }
}
