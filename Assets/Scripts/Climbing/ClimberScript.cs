using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ClimberScript : MonoBehaviour
{
    bool agh;
    //song related
    public Conductor conductor;
    private float currentSongPosition;
    [SerializeField] int bpm;

    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip song;
    [SerializeField] private AudioClip tapPerfectSound;
    [SerializeField] private AudioClip tapMissSound;
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
    [SerializeField] PerfectAndFail hitScore;
    [SerializeField] RetryScript transition;
    public List<Note> activeNotesList = new();
    public float approachRate;

    //visuals
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
    [SerializeField] TapirGuy tapir;

    [SerializeField] private Image beatProgressBar;
    float lastFullBeat = 0f;


    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float delay = 1f;

    int missCount;

    [SerializeField] private GameObject countdown;
    [SerializeField] private Image countdownImage;
    [SerializeField] private Sprite three;
    [SerializeField] private Sprite two;
    [SerializeField] private Sprite one;
    [SerializeField] private Sprite go;

    private void Start()
    {
        StartCountdown();
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        countdown.SetActive(true);

        countdownImage.sprite = three;
        yield return new WaitForSeconds(delay);

        countdownImage.sprite = two;
        yield return new WaitForSeconds(delay);

        countdownImage.sprite = one;
        yield return new WaitForSeconds(delay);

        countdownImage.sprite = go;
        yield return new WaitForSeconds(0.5f);

        countdown.SetActive(false);

        Setup();
    }

    private void Setup()
    {
        conductor.Setup(song, bpm);
        lastFullBeat = conductor.currentSongPosition;
        spriteRenderer = tapir.spriteRenderer;
        playerSprite1 = tapir.idleSprite;
        playerSprite2 = tapir.climbSprite;
        bgHeight = bg1.GetComponent<SpriteRenderer>().bounds.size.y;
        

        for (int i = 0; i < 100; i++)
        {
            hittiming += conductor.crotchet;

            noteDataList.Add(new NoteData
            {
                type = NoteType.Tap,
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


    [SerializeField] Transition changingSceneThing;
    private void Update()
    {
        if (agh) return;

        if (conductor.currentSongPosition >= 55f)
        {
            changingSceneThing.TransitionTo("ClimbEnd");
        }
        ClearInactiveNotes();

        currentSongPosition = conductor.currentSongPosition;

        float timeSinceLastBeat = currentSongPosition - lastFullBeat;

        if (timeSinceLastBeat >= conductor.crotchet)
        {
            lastFullBeat += conductor.crotchet;
            timeSinceLastBeat = currentSongPosition - lastFullBeat; // recalculate
        }

        //half beat
        if (conductor.currentSongPosition > lastBeat + conductor.crotchet)
        {
            lastBeat += conductor.crotchet / 2;

            tapir.Bob();

        }

        beatProgressBar.fillAmount = Mathf.Clamp01(timeSinceLastBeat / conductor.crotchet);

        

        
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
                missCount++;
                if (missCount >= 500000)
                {
                    conductor.musicSource.Stop();
                    DOTween.KillAll();
                    transition.Transition();
                    agh = true;
                }
                hitScore.ShowFail();
                Debug.Log("AUTO MISS");
                scoreText.color = Color.red;
                scoreText.text = "MISS";
                tapir.Miss();
                sfxAudioSource.PlayOneShot(tapMissSound);
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
            hitScore.ShowPerfect();
            Debug.Log("PERFECT");
            scoreText.color = Color.green;
            scoreText.text = ("PERFECT");
            sfxAudioSource.PlayOneShot(tapPerfectSound);
            Destroy(closestNote.gameObject);
            Climb();  
        }
        else
        {
            missCount++;
            if (missCount >= 5000000)
            {
                conductor.musicSource.Stop();
                DOTween.KillAll();
                transition.Transition();
                agh = true;
            }
            hitScore.ShowFail();
            Debug.Log("MISS");
            scoreText.color = Color.red;
            scoreText.text = ("MISS");
            tapir.Miss();
            sfxAudioSource.PlayOneShot(tapMissSound);
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
