using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
public class SushiScript : MonoBehaviour
{
    //song related
    public Conductor conductor;
    private float currentSongPosition;
    [SerializeField] int bpm;
    [SerializeField] private AudioClip song;
    public List<NoteData> noteDataList = new();

    //score windows
    [SerializeField] private float perfectWindow;
    [SerializeField] private float missWindow;
    [SerializeField] private TouchManager touchManager;

    //debug remvoe later
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Note tapNotePrefab;
    public List<Note> activeNotesList = new();
    public float approachRate;

    //visuals
    [SerializeField] Transform player;
    [SerializeField] Sprite playerSprite1;
    [SerializeField] Sprite playerSprite2;
    private SpriteRenderer spriteRenderer;
    public Transform belt1;
    public Transform belt2;
    public float moveAmount = 1f;
    public float beltWidth;
    private int spawnIndex = 0;
    float hittiming = 0f;
    Vector3 originalPos;
    [SerializeField] private float bobAmount = 0.2f;

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
        float lastBeat = conductor.currentSongPosition;
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

    float nextHalfBeat;
    float nextFullBeat;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SceneManager.LoadScene("ZenYoungTest");
        }

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
            {
                belt1.position = new Vector3(belt2.position.x - beltWidth, belt1.position.y, belt1.position.z);
            }

            if (belt2.position.x >= beltWidth)
            {
                belt2.position = new Vector3(belt1.position.x - beltWidth, belt2.position.y, belt2.position.z);
            }

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
        yield return null;
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
            note.Setup(conductor, spawnPoint, hitPoint, noteData, approachRate);
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

            if (hitTimingOffset < -perfectWindow) continue;

            if (hitTimingOffset < closestHitTiming)
            {
                closestHitTiming = hitTimingOffset;
                closestNote = note;
            }
        }
        return closestNote;
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
        }
        else
        {
            Debug.Log("MISS");
            scoreText.color = Color.red;
            scoreText.text = ("MISS");
            Destroy(closestNote.gameObject);
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


