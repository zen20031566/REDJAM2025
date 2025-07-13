using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Audio;
using System.Collections;

public class SumoScript : MonoBehaviour
{
    //song related
    public Conductor conductor;
    private float currentSongPosition;
    [SerializeField] int bpm;

    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] private AudioClip song;
    [SerializeField] private AudioClip tapPerfectSound;
    [SerializeField] private AudioClip tapMissSound;
    [SerializeField] private AudioClip swipeUpPerfectSound;
    [SerializeField] private AudioClip swipeDownPerfectSound;
    [SerializeField] private AudioClip swipeMissSound;
    [SerializeField] private AudioClip shoutSound;
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
    [SerializeField] Ugo Ugo;
    [SerializeField] SumoMan SumoMan;
    float lastBeat;
    private int spawnIndex = 0;
    private int visualIndex = 0;

    private void Start()

    {

        noteDataList = noteDataList.FindAll(note => note.hitTiming >= 30f);
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 3.6920f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 4.6150f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 5.5380f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 6.4610f });//4

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 7.3840f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 7.6147f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 8.3070f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 8.5377f });//double

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 9.2300f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 10.1530f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 11.0760f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 11.3067f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 11.9990f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 12.2297f });//double

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 12.9220f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 13.8450f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 14.7680f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 14.9987f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 15.6910f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 15.9217f });//double

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 16.6140f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 17.5370f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 18.4600f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 18.6907f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 19.3830f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 19.6137f });//double

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 20.3060f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 21.2290f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 22.1520f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 22.3827f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 23.0750f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 23.3057f });//double

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 23.9980f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 24.9210f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 25.8440f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 26.0747f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 26.7670f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 26.9977f });//double

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 27.6900f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 28.6130f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 29.5360f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 29.7667f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 30.4590f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 30.6897f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 31.3820f });


        noteDataList.Add(new NoteData { type = NoteType.Shout, hitTiming = 32.3050f });//Shout
        //noteDataList.Add(new NoteData { type = NoteType.SwipeDown, hitTiming = 33.2280f });//Down

        noteDataList.Add(new NoteData { type = NoteType.Shout, hitTiming = 33.2280f }); // shout
        noteDataList.Add(new NoteData { type = NoteType.SwipeUp, hitTiming = 34.1510f });   // up
        noteDataList.Add(new NoteData { type = NoteType.SwipeDown, hitTiming = 35.0740f });   // down

        noteDataList.Add(new NoteData { type = NoteType.Shout, hitTiming = 35.9970f }); // shout
        noteDataList.Add(new NoteData { type = NoteType.SwipeUp, hitTiming = 36.9200f });   // up
        noteDataList.Add(new NoteData { type = NoteType.SwipeDown, hitTiming = 37.8430f });   // down

        noteDataList.Add(new NoteData { type = NoteType.Shout, hitTiming = 38.7660f }); // shout
        noteDataList.Add(new NoteData { type = NoteType.SwipeUp, hitTiming = 39.6890f });   // up
        noteDataList.Add(new NoteData { type = NoteType.SwipeDown, hitTiming = 40.6120f });   // down

        noteDataList.Add(new NoteData { type = NoteType.Shout, hitTiming = 41.5350f }); // shout
        noteDataList.Add(new NoteData { type = NoteType.SwipeUp, hitTiming = 42.4580f });   // up
        noteDataList.Add(new NoteData { type = NoteType.SwipeDown, hitTiming = 43.3810f });   // down

        noteDataList.Add(new NoteData { type = NoteType.Shout, hitTiming = 44.3040f });//shout
        noteDataList.Add(new NoteData { type = NoteType.SwipeUp, hitTiming = 45.2270f });//up   
        noteDataList.Add(new NoteData { type = NoteType.SwipeDown, hitTiming = 46.1500f });//down*/

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 47.0730f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 47.9960f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 48.2267f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 48.9190f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 49.1497f });//double

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 49.8420f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 50.7650f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 51.6880f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 51.9187f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 52.6110f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 52.8417f });//double

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 53.5340f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 54.4570f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 55.3800f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 55.6107f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 56.3030f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 56.5337f });//double

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 57.2260f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 58.1490f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 59.0720f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 59.3027f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 59.9950f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 60.2257f });//double


        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 60.9180f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 61.8410f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 62.7640f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 62.9947f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 63.6870f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 63.9177f });//double*/

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 64.6100f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 65.5330f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 66.4560f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 66.6867f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 67.3790f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 67.6097f });//double*/

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 68.3020f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 69.2250f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 70.1480f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 70.3787f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 71.0710f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 71.3017f });//double

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 71.9940f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 72.9170f });

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 73.8400f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 74.0707f });//double
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 74.7630f });
        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 74.9937f });//double

        noteDataList.Add(new NoteData { type = NoteType.Tap, hitTiming = 75.6860f });

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
        if (Input.GetKeyDown(KeyCode.D))
        {
            conductor.Setup(song, bpm, 0f);
        }
        ClearInactiveNotes();

        currentSongPosition = conductor.currentSongPosition;
        Debug.Log(currentSongPosition);

        //Every half beat
        if (conductor.currentSongPosition > lastBeat + conductor.crotchet)
        {
            lastBeat += conductor.crotchet / 2;

            Ugo.Bob();
            SumoMan.Bob();
        }

        while (visualIndex < noteDataList.Count && noteDataList[visualIndex].hitTiming <= currentSongPosition)
        {
            NoteData note = noteDataList[visualIndex];

            if (note.type == NoteType.Shout)
            {
                SumoMan.Shout();
            }
            else if (note.type == NoteType.SwipeUp)
            {
                SumoMan.LegUp();
            }
            else if (note.type == NoteType.SwipeDown)
            {
                SumoMan.Stomp();
            }
            else
            {
                SumoMan.Push();
            }
            visualIndex++;
        }

        while (spawnIndex < noteDataList.Count && noteDataList[spawnIndex].hitTiming <= currentSongPosition + approachRate)
        {
            var note = noteDataList[spawnIndex];

            if (note.type != NoteType.Shout)
            {
                SpawnNote(note);
            }
            spawnIndex++;
        }
    }

    public void SpawnNote(NoteData noteData)
    {
        Note prefab = null;
        if (noteData.type == NoteType.Tap ||
            noteData.type == NoteType.SwipeUp ||
            noteData.type == NoteType.SwipeDown)
        {
            prefab = tapNotePrefab; // Assuming same prefab for all
        }
            Note note = Instantiate(prefab);
            note.Setup(conductor, spawnPoint, hitPoint, noteData, approachRate);
            activeNotesList.Add(note);
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
                Ugo.Miss();
                Debug.Log("AUTO MISS");
                scoreText.color = Color.red;
                scoreText.text = "MISS";
                if (note.type == NoteType.Tap)
                {
                    sfxAudioSource.PlayOneShot(tapMissSound);
                }
                else if (note.type == NoteType.SwipeUp || note.type == NoteType.SwipeDown)
                {
                    sfxAudioSource.PlayOneShot(swipeMissSound);
                }
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

            if (type == NoteType.Tap)
            {
                Ugo.Push();
                sfxAudioSource.PlayOneShot(tapPerfectSound);
            }
            else if (type == NoteType.SwipeUp)
            {
                Ugo.LegUp();
                sfxAudioSource.PlayOneShot(swipeUpPerfectSound);
            }
            else if (type == NoteType.SwipeDown)
            {
                Ugo.Stomp();
                sfxAudioSource.PlayOneShot(swipeDownPerfectSound);
            }
        }
        else
        {
            Ugo.Miss();
            Debug.Log("MISS");
            scoreText.color = Color.red;
            scoreText.text = ("MISS");
            if (type == NoteType.Tap)
            {
                sfxAudioSource.PlayOneShot(tapMissSound);
            }
            else if (type == NoteType.SwipeUp || type == NoteType.SwipeDown)
            {
                sfxAudioSource.PlayOneShot(swipeMissSound);
            }
        }
        Destroy(closestNote.gameObject);
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
