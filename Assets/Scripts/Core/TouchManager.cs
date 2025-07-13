using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class TouchManager : MonoBehaviour
{
    private TouchAction input;

    [SerializeField] private float minimumDistance = 50f;
    [SerializeField] private float maximumTime = 1f;
    public bool swipeJustDetected = false;

    public event EventHandler OnTap;
    public event EventHandler OnTapReleased;
    public event EventHandler OnSwipeUp;
    public event EventHandler OnSwipeDown;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    private float startTouchTime;
    private float endTouchTime;

    private void Awake()
    {
        input = new TouchAction();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Touch.NoteTouch.performed += OnNoteTouch;
        input.Touch.NoteTouch.canceled += OnNoteRelease;
    }
    private void OnDisable()
    {
        input.Touch.NoteTouch.performed -= OnNoteTouch;
        input.Touch.NoteTouch.canceled -= OnNoteRelease;
        input.Disable();
    }
    public void OnNoteTouch(InputAction.CallbackContext context)
    {
        startTouchPos = Touchscreen.current.position.ReadValue(); //Save up touch position for later swipe //this in screen space not world so based on pixels
        startTouchTime = (float)context.startTime;
    }

    private void OnNoteRelease(InputAction.CallbackContext context)
    {
        swipeJustDetected = false;

        endTouchPos = Touchscreen.current.position.ReadValue(); //Save up untouch position for later swipe //this in screen space not world so based on pixels
        endTouchTime = (float)context.time;
        Vector2 swipe = endTouchPos - startTouchPos;

        //if swipe magnitde over certain value then count as swipe?
        if (swipe.magnitude > minimumDistance &&
            (endTouchTime - startTouchTime) <= maximumTime) //how long swipe last is <= max time else its a hold not swipe
        {
            swipeJustDetected = true;
            if (Mathf.Abs(swipe.y) > Math.Abs(swipe.x))
            {
                if (swipe.y > 0)
                {
                    Debug.Log("Swipe Up Detected");
                    OnSwipeUp?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Debug.Log("Swipe Down Detected");
                    OnSwipeDown?.Invoke(this, EventArgs.Empty);
                }
            }
            
        }

        //if not swipe then trigger onTap event, just to not let touch and swipe conflict
        if (!swipeJustDetected)
        {
            Debug.Log("Tap Detected");
            OnTap?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            swipeJustDetected = false;
        }

            OnTapReleased?.Invoke(this, EventArgs.Empty);
        Debug.Log("Tap Releasedd");

    }
}