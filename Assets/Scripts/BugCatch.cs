using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonController : MonoBehaviour
{
    [SerializeField] SpriteRenderer SpriteRenderer;
    [SerializeField] private TouchManager touchManager;
    [SerializeField] private Conductor conductor;
    [SerializeField] AudioClip song;
    [SerializeField] float bpm;

    public Sprite defaultImage;
    public Sprite pressedImage;

    private float startTime;
    private float perfectWindow = 0.5f;

    List<Note> notes = new List<Note>();

    private Vector2 startTouchPos;

    private void OnEnable()
    {
        touchManager.OnTap += TouchManager_OnScreenTouched;
        touchManager.OnTapReleased += TouchManager_OnScreenReleased;
        touchManager.OnSwipeUp += TouchManager_OnSwipeUp;
    }

    private void OnDisable()
    {
        touchManager.OnTap -= TouchManager_OnScreenTouched;
        touchManager.OnTapReleased -= TouchManager_OnScreenReleased;
        touchManager.OnSwipeUp -= TouchManager_OnSwipeUp;
    }

    void Setup()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();

        startTime = conductor.dspSongStartTime;
        conductor.Setup(song, bpm);

        notes.Add(new Note { time = 0.8f, hit = false, type = NoteType.SwipeUp });
        notes.Add(new Note { time = 1.6f, hit = false, type = NoteType.SwipeUp });
        notes.Add(new Note { time = 2.4f, hit = false, type = NoteType.SwipeUp });
        notes.Add(new Note { time = 3.2f, hit = false, type = NoteType.SwipeUp });
        notes.Add(new Note { time = 4.0f, hit = false, type = NoteType.SwipeUp });
        notes.Add(new Note { time = 4.8f, hit = false, type = NoteType.SwipeUp });
    }

    void Update()
    {
        float currentTime = conductor.currentSongPosition;

        foreach (Note note in notes)
        {
            if (note.hit) continue;

            //If player missed the note (time already passed and they didnnt touch it)
            if (currentTime > note.time + perfectWindow)
            {
                note.hit = true;      //note lock
                ShowResult(0);        //fail
            }
        }

    }
    private void NoteHitChecker(NoteType inputType)
    {
        float currentTime = Time.time - startTime;
        Note closestNote = null;
        float closestDifference = Mathf.Infinity;

        //Go through each note and check whether player perfect or fail
        foreach (Note note in notes)
        {
            if (note.hit || note.type != inputType) continue;

            float difference = Mathf.Abs(currentTime - note.time);
            if (difference < closestDifference)
            {
                closestDifference = difference;
                closestNote = note;
            }
        }

        if (closestNote != null && closestDifference <= perfectWindow)
        {
            closestNote.hit = true;
            ShowResult(1); //Perfect
        }
        else
        {
            ShowResult(0); //fail
        }
    }

    //Just to show perfect or fail, i just seperate the logics
    void ShowResult(int result)
    {
        if (result == 1)
        {
            Debug.Log("Perfect");
            SpriteRenderer.sprite = pressedImage;
        }
        else
        {
            Debug.Log("fail");
            SpriteRenderer.sprite = defaultImage;
        }
    }

    private void TouchManager_OnScreenTouched(object sender, System.EventArgs e)
    {
        Debug.Log("Touch detected in ButtonController!");

        if (!touchManager.swipeJustDetected)
        {
            NoteHitChecker(NoteType.Tap);
        }
    }

    private void TouchManager_OnScreenReleased(object sender, System.EventArgs e)
    {
        SpriteRenderer.sprite = defaultImage;
    }

    private void TouchManager_OnSwipeUp(object sender, System.EventArgs e)
    {
        NoteHitChecker(NoteType.SwipeUp);
    }



}
