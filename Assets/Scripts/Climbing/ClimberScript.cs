using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ClimberScript : MonoBehaviour
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
    Sprite playerSprite1;
    Sprite playerSprite2;
    private SpriteRenderer spriteRenderer;
    float lastBeat;
    public Transform bg1;
    public Transform bg2;
    public float scrollSpeed = 1f;
    public float bgHeight;
    private int spawnIndex = 0;
    float hittiming = 0f;
    Vector3 originalPos;
    [SerializeField] private float bobAmount = 0.2f;
    [SerializeField] TapirGuy tapir;

    private void Start()
    {
        spriteRenderer = tapir.spriteRenderer;
        playerSprite1 = tapir.idleSprite;
        playerSprite2 = tapir.climbSprite;
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

    private void Update()
    {
        currentSongPosition = conductor.currentSongPosition;

        if (Input.GetKeyDown(KeyCode.W))
        {
            SceneManager.LoadScene("Sushi");
        }
        //Move head bob 
        if (conductor.currentSongPosition > lastBeat + conductor.crotchet)
        {
            lastBeat += conductor.crotchet / 2;

            tapir.Bob();

        }

        ClearInactiveNotes();

        
        //spawning 
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
                tapir.Miss();
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
            Climb();  
        }
        else
        {
            Debug.Log("MISS");
            scoreText.color = Color.red;
            scoreText.text = ("MISS");
            tapir.Miss();
            Destroy(closestNote.gameObject);
        }
    }

    bool spriteA;
    private void Climb()
    {
        spriteRenderer.sprite = spriteA ? playerSprite1 : playerSprite2;
        spriteA = !spriteA;

        bg1.position += Vector3.down * scrollSpeed;
        bg2.position += Vector3.down * scrollSpeed;

        if (bg1.position.y <= -bgHeight)
        {
            bg1.position = new Vector3(bg1.position.x, bg2.position.y + bgHeight, bg1.position.z);
        }

        //if bg2 moves below the screen, move it above bg1
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
