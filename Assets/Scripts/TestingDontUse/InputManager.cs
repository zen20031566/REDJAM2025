using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    private PlayerControls playerControls;
    private Camera mainCamera;

    public delegate void StartTouch(Vector2 positiion, float time);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 positiion, float time);
    public event EndTouch OnEndTouch;

    protected override void Awake()
    {
        base.Awake();
        playerControls = new PlayerControls();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Touch.PrimaryContact.started += context => StartTouchPrimary(context);
        playerControls.Touch.PrimaryContact.canceled += context => EndTouchPrimary(context);
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.Touch.PrimaryContact.started -= context => StartTouchPrimary(context);
        playerControls.Touch.PrimaryContact.canceled -= context => EndTouchPrimary(context);
    }

    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnStartTouch != null)
        {
            OnStartTouch(Utils.ScreenToWorld(mainCamera, playerControls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.startTime);
        }
    }

    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null)
        {
            OnEndTouch(Utils.ScreenToWorld(mainCamera, playerControls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.time);
        }
    }

    public Vector2 PrimaryPosition()
    {
        return Utils.ScreenToWorld(mainCamera, playerControls.Touch.PrimaryPosition.ReadValue<Vector2>());
    }
}
