using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.U2D;

public class NoteManager : MonoBehaviour
{
    public Conductor conductor;
    private float currentSongPosition;

    [SerializeField] private float perfectWindow;
    [SerializeField] private float missWindow;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TouchManager touchManager;
    [SerializeField] private Note tapNotePrefab;
    [SerializeField] private AudioClip song;
    [SerializeField] int bpm;
    [SerializeField] private TextAsset hitTimingsTextAsset;
    [SerializeField] Transform player;

    public Transform bg1;
    public Transform bg2;
    public float scrollSpeed = 1f;
    public float bgHeight;

    public List<NoteData> noteDataList = new();
    public float approachRate;

    public List<Note> activeNotesList = new();
    private int spawnIndex = 0;

    float hittiming = 0f;
    private void Start()
    {
        bgHeight = bg1.GetComponent<SpriteRenderer>().bounds.size.y;
        conductor.Setup(song, bpm);

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
        ClearInactiveNotes();

        currentSongPosition = conductor.currentSongPosition;

        while (spawnIndex < noteDataList.Count && noteDataList[spawnIndex].hitTiming <= currentSongPosition + approachRate)
        {
            SpawnNote(noteDataList[spawnIndex]);
            spawnIndex++;
        }
    }

    public void SpawnNote(NoteData noteData)
    {
        Note prefab = null;
        if (noteData.type == NoteType.Tap)
        {
            prefab = tapNotePrefab;
        }
        if (noteData.type == NoteType.SwipeUp)
        {
            prefab = tapNotePrefab;
        }
        if (prefab != null)
        {
            Note note = Instantiate(prefab);
            note.Setup(conductor, this, spawnPoint, hitPoint, noteData);
            activeNotesList.Add(note);
        }

    }

    void ClearInactiveNotes()
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
            if (timeSinceNote > missWindow)
            {
                Debug.Log("AUTO MISS");
                scoreText.color = Color.red;
                scoreText.text = "MISS";
                Destroy(note.gameObject);
                activeNotesList.RemoveAt(i);
            }
        }
    }

    public Note GetClosestNote(NoteType type)
    {
        Note closestNote = null;
        float closestHitTiming = float.MaxValue;
        foreach (var note in activeNotesList)
        {
            if (note.type != type) continue;
            float hitTimingOffset = Mathf.Abs(currentSongPosition - note.hitTiming);

            if (hitTimingOffset < - perfectWindow) continue;

            if (hitTimingOffset < closestHitTiming)
            {
                closestHitTiming = hitTimingOffset;
                closestNote = note;
            }
        }
        return (closestHitTiming <= missWindow) ? closestNote : null;
    }

    public void CheckNoteHitTiming(NoteType type)
    {
        Note closestNote = GetClosestNote(type);
        if (closestNote == null) return;

        float hitTimingOffset = Mathf.Abs(currentSongPosition - closestNote.hitTiming);
        if (hitTimingOffset <= perfectWindow)
        {
            Debug.Log("PERFECT");
            scoreText.color = Color.green;
            scoreText.text = ("PERFECT");
            Destroy(closestNote.gameObject);
            Climb();

            
        }
        else
        {
            Debug.Log("MISS");
            scoreText.color = Color.red;
            scoreText.text = ("MISS");
            Destroy(closestNote.gameObject);
        }
    }

    private void Climb()
    {
        bg1.position += Vector3.down * scrollSpeed;
        bg2.position += Vector3.down * scrollSpeed;

        if (bg1.position.y <= -bgHeight)
        {
            bg1.position = new Vector3(bg1.position.x, bg2.position.y + bgHeight, bg1.position.z);
        }

        // If bg2 moves below the screen, move it above bg1
        if (bg2.position.y <= -bgHeight)
        {
            bg2.position = new Vector3(bg2.position.x, bg1.position.y + bgHeight, bg2.position.z);
        }
    }



private void TouchManager_OnScreenTouched(object sender, System.EventArgs e)
    {
        Debug.Log("Touch detected");

        if (!touchManager.swipeJustDetected)
        {
            CheckNoteHitTiming(NoteType.Tap);
        }
    }

    private void TouchManager_OnScreenReleased(object sender, System.EventArgs e)
    {
        //SpriteRenderer.sprite = defaultImage;
    }

    private void TouchManager_OnSwipeUp(object sender, System.EventArgs e)
    {
        CheckNoteHitTiming(NoteType.SwipeUp);
    }

    private void TouchManager_OnSwipeDown(object sender, System.EventArgs e)
    {
        CheckNoteHitTiming(NoteType.SwipeDown);
    }

}
