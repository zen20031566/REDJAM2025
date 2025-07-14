using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
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
    [SerializeField] private SushiNote sushiPrefab;
    [SerializeField] PerfectAndFail hitScore;
    public List<Note> activeNotesList = new();
    public float approachRate;

    //visuals
    public Transform belt1;
    public float moveAmount = 1f;
    public float beltWidth;
    private int spawnIndex = 0;
    float hittiming = 0f;
    [SerializeField] private ElephantGuy elephantGuy;

    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float delay = 1f;

    [SerializeField] RetryScript transition;
    int missCount;
    bool agh = false;

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
        beltWidth = belt1.GetComponent<SpriteRenderer>().bounds.size.x;
        

        for (int i = 0; i < 78; i++)
        {
            hittiming += conductor.crotchet;

            NoteType type;
            if (Random.value < 0.65f) //70% chance
                type = NoteType.SwipeUp;
            else if (Random.value < 0.30f)
                type = NoteType.SwipeDown;
            else
            {
                continue;
            }

                noteDataList.Add(new NoteData
                {
                    type = type,
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

    [SerializeField] Transition changingSceneThing;
    private void Update()
    {
        if (agh) return;

        if (conductor.currentSongPosition >= 63f)
        {
            changingSceneThing.TransitionTo("SushiEnd");
        }

        if (conductor.currentSongPosition > nextHalfBeat + conductor.crotchet)
        {
            nextHalfBeat += conductor.crotchet / 2f;
            elephantGuy.Bob();
        }

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
        Note prefab = sushiPrefab;

        if (noteData.type == NoteType.SwipeUp)
        {
            Note note = Instantiate(prefab);
            if (note is SushiNote sushi)
            {
                sushi.Setup(conductor, spawnPoint, hitPoint, noteData, approachRate);
            }

            activeNotesList.Add(note);
        }
        if (noteData.type == NoteType.SwipeDown)
        {
            Note note = Instantiate(prefab);
            if (note is SushiNote sushi)
            {
                sushi.Setup(conductor, spawnPoint, hitPoint, noteData, approachRate, true);
            }

            activeNotesList.Add(note);
        }
        else
        {
            return;
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
            if (timeSinceNote > missWindow && note.isInitialized)
            {
                missCount++;
                if (missCount >= 5000000)
                {
                    conductor.musicSource.Stop();
                    DOTween.KillAll();
                    transition.Transition();
                    agh = true;
                }
                Debug.Log("AUTO MISS");
                scoreText.color = Color.red;
                scoreText.text = "MISS";
                note.isInitialized = false;
                hitScore.ShowFail();
                elephantGuy.Cry();
            }

            if (note.transform.position.x >= beltWidth)
            {
                activeNotesList.RemoveAt(i);
                Destroy(note.gameObject);
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
        //if (hitTimingOffset <= -perfectWindow)
        //{
        //    Debug.Log("Tooe arly");
        //}
        if (hitTimingOffset <= perfectWindow)
        {
            hitScore.ShowPerfect();
            Debug.Log("PERFECT");
            scoreText.color = Color.green;
            scoreText.text = ("PERFECT");
            elephantGuy.Eat();
            if (closestNote is SushiNote sushi)
            {
                if (sushi.type == NoteType.SwipeUp)
                {
                    sushi.FlickUp();
                }
                else
                {
                    sushi.FlickDown();
                }
            }

            closestNote.isInitialized = false;
        }
        else
        {
            missCount++;
            if (missCount >= 50000000)
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
            closestNote.isInitialized = false;
            elephantGuy.Cry();
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
        Note hitNote = GetClosestNote(NoteType.SwipeUp);
        if (hitNote != null && Mathf.Abs(currentSongPosition - hitNote.hitTiming) <= perfectWindow)
        {
            CheckNoteHitTiming(NoteType.SwipeUp);
        }
        else
        {
            Note wrongNote = GetClosestNote(NoteType.SwipeDown);
            if (wrongNote != null && Mathf.Abs(currentSongPosition - wrongNote.hitTiming) <= perfectWindow)
            {
                if (wrongNote is SushiNote sushi)
                {
                    sushi.FlickUp();
                }
                missCount++;
                if (missCount >= 50000000)
                {
                    conductor.musicSource.Stop();
                    DOTween.KillAll();
                    transition.Transition();
                    agh = true;
                }
                hitScore.ShowFail();
                elephantGuy.Miss();
                Debug.Log("Swipe up onn swipe down");
                scoreText.color = Color.red;
                scoreText.text = "MISS";
                wrongNote.isInitialized = false;
            }
        }
    }

    private void TouchManager_OnSwipeDown(object sender, System.EventArgs e)
    {
        CheckNoteHitTiming(NoteType.SwipeDown);
    }

   
}




