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
        input.Touch.NoteTouch.started += OnNoteTouch;
        input.Touch.NoteTouch.canceled += OnNoteRelease;
    }

    private void OnDisable()
    {
        input.Touch.NoteTouch.started -= OnNoteTouch;
        input.Touch.NoteTouch.canceled -= OnNoteRelease;
        input.Disable();
    }

    private void OnNoteTouch(InputAction.CallbackContext context)
    {
        startTouchTime = Time.time;
        startTouchPos = GetPointerPosition();
    }

    private void OnNoteRelease(InputAction.CallbackContext context)
    {
        swipeJustDetected = false;

        endTouchTime = Time.time;
        endTouchPos = GetPointerPosition();

        Vector2 swipe = endTouchPos - startTouchPos;

        if (swipe.magnitude > minimumDistance &&
            (endTouchTime - startTouchTime) <= maximumTime)
        {
            if (Mathf.Abs(swipe.y) > Mathf.Abs(swipe.x))
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

                swipeJustDetected = true;
            }
        }

        if (!swipeJustDetected)
        {
            Debug.Log("Tap Detected");
            OnTap?.Invoke(this, EventArgs.Empty);
        }

        OnTapReleased?.Invoke(this, EventArgs.Empty);
        Debug.Log("Tap Released");
    }

    private Vector2 GetPointerPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }

        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue(); // No need to check isPressed
        }

        return Vector2.zero;
    }
}
