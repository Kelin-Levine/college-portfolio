using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Instance
    private static bool InstanceCreated = false;
    private static InputManager _Instance;
    private static InputManager Instance
    {
        get
        {
            // There is a reason InstanceCreated is used instead of checking if _Instance is null.
            // When the game shuts down, the instance is destroyed, and a new one must not be created, even if calls to it are made.
            // There should only ever be one instance created, so if one was created but is no longer there, then we can assume that it was destroyed by the game shutting down.
            // This is also the reason for the null checks on uses of Instance. If an instance can't be retrieved, then actions on it should be avoided.
            if (!InstanceCreated) InitializeInstance();
            return _Instance;
        }
        set { _Instance = value; }
    }


    // Input Controller
    private static PlayerInput _Input;
    public static PlayerInput Input
    {
        get
        {
            if (_Input == null) InitializePlayerInput();
            return _Input;
        }
        private set { _Input = value; }
    }


    // Action Group Toggles
    // Gameplay
    public static bool GameplayEnabled
    {
        get => Input.Gameplay.enabled;
        set => SetActionsEnabled(Input.Gameplay, value);
    }
    

    // Input Actions
    // Click
    public static bool IsClickDown { get => Pointer.current.press.isPressed; }
    public static void BindClickDown(UnityAction call) { if (Instance != null) Instance.OnClickDownEvent.AddListener(call); }
    public static void UnbindClickDown(UnityAction call) { if (Instance != null) Instance.OnClickDownEvent.RemoveListener(call); }
    public static void BindClickUp(UnityAction call) { if (Instance != null) Instance.OnClickUpEvent.AddListener(call); }
    public static void UnbindClickUp(UnityAction call) { if (Instance != null) Instance.OnClickUpEvent.RemoveListener(call); }
    public static void BindWhileClickDown(UnityAction call) { if (Instance != null) Instance.WhileClickDownEvent.AddListener(call); }
    public static void UnbindWhileClickDown(UnityAction call) { if (Instance != null) Instance.WhileClickDownEvent.RemoveListener(call); }
    public static void BindWhileClickUp(UnityAction call) { if (Instance != null) Instance.WhileClickUpEvent.AddListener(call); }
    public static void UnbindWhileClickUp(UnityAction call) { if (Instance != null) Instance.WhileClickUpEvent.RemoveListener(call); }
    
    // Menu (exclusive to PC (esc) and Android (back))
    public static InputAction MenuAction { get => Input.Gameplay.Menu; }
    public static bool IsMenuDown { get => MenuAction.IsPressed(); }
    public static void BindMenuDown(Action<InputAction.CallbackContext> call) { MenuAction.started += call; }
    public static void UnbindMenuDown(Action<InputAction.CallbackContext> call) { MenuAction.started -= call; }
    public static void BindMenuUp(Action<InputAction.CallbackContext> call) { MenuAction.canceled += call; }
    public static void UnbindMenuUp(Action<InputAction.CallbackContext> call) { MenuAction.canceled -= call; }

    // CursorPosition (would PointerPosition be a better name? oh well, too late now)
    public static Vector2 CursorPosition { get => Pointer.current.position.ReadValue(); }
    
    // CameraToggle
    public static InputAction CameraToggleAction { get => Input.Gameplay.CameraToggle; }
    public static bool IsCameraToggleDown { get => CameraToggleAction.IsPressed(); }
    public static void BindCameraToggleDown(Action<InputAction.CallbackContext> call) { CameraToggleAction.started += call; }
    public static void UnbindCameraToggleDown(Action<InputAction.CallbackContext> call) { CameraToggleAction.started -= call; }
    public static void BindCameraToggleUp(Action<InputAction.CallbackContext> call) { CameraToggleAction.canceled += call; }
    public static void UnbindCameraToggleUp(Action<InputAction.CallbackContext> call) { CameraToggleAction.canceled -= call; }

    // CameraPan
    public static InputAction CameraPanAction { get => Input.Gameplay.CameraPan; }
    public static Vector2 CameraPanPosition { get => CameraPanAction.ReadValue<Vector2>(); }
    public static void BindCameraPanChanged(Action<InputAction.CallbackContext> call) { CameraPanAction.started += call; CameraPanAction.performed += call; CameraPanAction.canceled += call; }
    public static void UnbindCameraPanChanged(Action<InputAction.CallbackContext> call) { CameraPanAction.started -= call; CameraPanAction.performed -= call; CameraPanAction.canceled -= call; }

    // CameraZoom
    public static InputAction CameraZoomAction { get => Input.Gameplay.CameraZoom; }
    public static float CameraZoomAxis { get => CameraZoomAction.ReadValue<float>(); }
    public static void BindCameraZoomChanged(Action<InputAction.CallbackContext> call) { CameraZoomAction.started += call; CameraZoomAction.performed += call; CameraZoomAction.canceled += call; }
    public static void UnbindCameraZoomChanged(Action<InputAction.CallbackContext> call) { CameraZoomAction.started -= call; CameraZoomAction.performed -= call; CameraZoomAction.canceled -= call; }


    // Exclusive to touchscreen devices
    // PrimaryTouchContact
    public static InputAction PrimaryTouchContactAction { get => Input.Gameplay.PrimaryTouchContact; }
    public static bool IsPrimaryTouchContactDown { get => PrimaryTouchContactAction.IsPressed(); }
    public static void BindPrimaryTouchContactDown(Action<InputAction.CallbackContext> call) { PrimaryTouchContactAction.started += call; }
    public static void UnbindPrimaryTouchContactDown(Action<InputAction.CallbackContext> call) { PrimaryTouchContactAction.started -= call; }
    public static void BindPrimaryTouchContactUp(Action<InputAction.CallbackContext> call) { PrimaryTouchContactAction.canceled += call; }
    public static void UnbindPrimaryTouchContactUp(Action<InputAction.CallbackContext> call) { PrimaryTouchContactAction.canceled -= call; }

    // SecondaryTouchContact
    public static InputAction SecondaryTouchContactAction { get => Input.Gameplay.SecondaryTouchContact; }
    public static bool IsSecondaryTouchContactDown { get => SecondaryTouchContactAction.IsPressed(); }
    public static void BindSecondaryTouchContactDown(Action<InputAction.CallbackContext> call) { SecondaryTouchContactAction.started += call; }
    public static void UnbindSecondaryTouchContactDown(Action<InputAction.CallbackContext> call) { SecondaryTouchContactAction.started -= call; }
    public static void BindSecondaryTouchContactUp(Action<InputAction.CallbackContext> call) { SecondaryTouchContactAction.canceled += call; }
    public static void UnbindSecondaryTouchContactUp(Action<InputAction.CallbackContext> call) { SecondaryTouchContactAction.canceled -= call; }

    // TertiaryTouchContact
    public static InputAction TertiaryTouchContactAction { get => Input.Gameplay.TertiaryTouchContact; }
    public static bool IsTertiaryTouchContactDown { get => TertiaryTouchContactAction.IsPressed(); }
    public static void BindTertiaryTouchContactDown(Action<InputAction.CallbackContext> call) { TertiaryTouchContactAction.started += call; }
    public static void UnbindTertiaryTouchContactDown(Action<InputAction.CallbackContext> call) { TertiaryTouchContactAction.started -= call; }
    public static void BindTertiaryTouchContactUp(Action<InputAction.CallbackContext> call) { TertiaryTouchContactAction.canceled += call; }
    public static void UnbindTertiaryTouchContactUp(Action<InputAction.CallbackContext> call) { TertiaryTouchContactAction.canceled -= call; }

    // PrimaryTouchPosition
    public static InputAction PrimaryTouchPositionAction { get => Input.Gameplay.PrimaryTouchPosition; }
    public static Vector2 PrimaryTouchPosition { get => PrimaryTouchPositionAction.ReadValue<Vector2>(); }
    public static void BindPrimaryTouchPositionChanged(Action<InputAction.CallbackContext> call) { PrimaryTouchPositionAction.performed += call; }
    public static void UnbindPrimaryTouchPositionChanged(Action<InputAction.CallbackContext> call) { PrimaryTouchPositionAction.performed -= call; }

    // SecondaryTouchPosition
    public static InputAction SecondaryTouchPositionAction { get => Input.Gameplay.SecondaryTouchPosition; }
    public static Vector2 SecondaryTouchPosition { get => SecondaryTouchPositionAction.ReadValue<Vector2>(); }
    public static void BindSecondaryTouchPositionChanged(Action<InputAction.CallbackContext> call) { SecondaryTouchPositionAction.performed += call; }
    public static void UnbindSecondaryTouchPositionChanged(Action<InputAction.CallbackContext> call) { SecondaryTouchPositionAction.performed -= call; }

    // TertiaryTouchPosition
    public static InputAction TertiaryTouchPositionAction { get => Input.Gameplay.TertiaryTouchPosition; }
    public static Vector2 TertiaryTouchPosition { get => TertiaryTouchPositionAction.ReadValue<Vector2>(); }
    public static void BindTertiaryTouchPositionChanged(Action<InputAction.CallbackContext> call) { TertiaryTouchPositionAction.performed += call; }
    public static void UnbindTertiaryTouchPositionChanged(Action<InputAction.CallbackContext> call) { TertiaryTouchPositionAction.performed -= call; }


    // Touchscreen multi-touch gestures (exclusive to touchscreen devices)
    public static void BindPanTouchGestureChanged(UnityAction<Vector2> call) { if (Instance != null) Instance.PanTouchGestureChangedEvent.AddListener(call); }
    public static void UnbindPanTouchGestureChanged(UnityAction<Vector2> call) { if (Instance != null) Instance.PanTouchGestureChangedEvent.RemoveListener(call); }
    public static void BindPinchTouchGestureChanged(UnityAction<float> call) { if (Instance != null) Instance.PinchTouchGestureChangedEvent.AddListener(call); }
    public static void UnbindPinchTouchGestureChanged(UnityAction<float> call) { if (Instance != null) Instance.PinchTouchGestureChangedEvent.RemoveListener(call); }


    // Helper Functions
    private static void InitializeInstance()
    {
        GameObject instanceObject = new("InputManager");
        DontDestroyOnLoad(instanceObject);
        Instance = instanceObject.AddComponent<InputManager>();
        InstanceCreated = true;
    }

    private static void InitializePlayerInput()
    {
        Input = new();
        GameplayEnabled = true; //hardcoded?
    }

    private static void SetActionsEnabled(PlayerInput.GameplayActions actions, bool enabled)
    {
        if (enabled) actions.Enable();
        else actions.Disable();
    }


    // Instance Events
    private readonly UnityEvent OnClickDownEvent = new();
    private readonly UnityEvent OnClickUpEvent = new();
    private readonly UnityEvent WhileClickDownEvent = new();
    private readonly UnityEvent WhileClickUpEvent = new();
    private readonly UnityEvent<Vector2> PanTouchGestureChangedEvent = new();
    private readonly UnityEvent<float> PinchTouchGestureChangedEvent = new();

    // Instance Variables
    private bool WasLastClickDown = false;
    private Vector2 scaledPrimaryTouch = Vector2.zero, scaledSecondaryTouch = Vector2.zero, scaledTertiaryTouch = Vector2.zero;
    private Vector2 lastPrimaryTouch = Vector2.zero, lastSecondaryTouch = Vector2.zero, lastTertiaryTouch = Vector2.zero;
    private Vector2 deltaPrimaryTouch = Vector2.zero, deltaSecondaryTouch = Vector2.zero, deltaTertiaryTouch = Vector2.zero;
    private bool lastPrimaryTouchDown = false, lastSecondaryTouchDown = false, lastTertiaryTouchDown = false;
    //private bool panEventThis = false, pinchEventThis = false, panEventLast = false, pinchEventLast = false;

    // Reused
    float viewTouchDot;


    // Instance Functions
    private void Update()
    {
        // ClickDown
        if (IsClickDown)
        {
            if (!WasLastClickDown) OnClickDownEvent.Invoke();
            WhileClickDownEvent.Invoke();
        }
        else
        {
            if (WasLastClickDown) OnClickUpEvent.Invoke();
            WhileClickUpEvent.Invoke();
        }

        // Update last click down
        WasLastClickDown = IsClickDown;


        // Fix for PC
        if (CameraPanPosition != Vector2.zero) PanTouchGestureChangedEvent.Invoke(CameraPanPosition);
        if (!Mathf.Approximately(CameraZoomAxis, 0)) PinchTouchGestureChangedEvent.Invoke(CameraZoomAxis);


        // Touch Gestures
        // Update scaled touch positions
        scaledPrimaryTouch = PrimaryTouchPosition / Screen.dpi;
        scaledSecondaryTouch = SecondaryTouchPosition / Screen.dpi;
        scaledTertiaryTouch = TertiaryTouchPosition / Screen.dpi;
        // Update delta touch positions
        deltaPrimaryTouch = scaledPrimaryTouch - lastPrimaryTouch;
        deltaSecondaryTouch = scaledSecondaryTouch - lastSecondaryTouch;
        deltaTertiaryTouch = scaledTertiaryTouch - lastTertiaryTouch;

        // PanTouchGesture
        // PinchTouchGesture
        if (IsSecondaryTouchContactDown && lastSecondaryTouchDown)
        {
            if (IsTertiaryTouchContactDown && lastTertiaryTouchDown)
            {
                viewTouchDot = Vector2.Dot(deltaSecondaryTouch.normalized, deltaTertiaryTouch.normalized);
                CheckPanGesture(viewTouchDot, deltaSecondaryTouch, deltaTertiaryTouch);
                CheckPinchGesture(viewTouchDot, scaledSecondaryTouch, lastSecondaryTouch, deltaSecondaryTouch, scaledTertiaryTouch, lastTertiaryTouch, deltaTertiaryTouch);
            }
            else
            {
                viewTouchDot = Vector2.Dot(deltaPrimaryTouch, deltaSecondaryTouch);
                CheckPanGesture(viewTouchDot, deltaPrimaryTouch, deltaSecondaryTouch);
                CheckPinchGesture(viewTouchDot, scaledPrimaryTouch, lastPrimaryTouch, deltaPrimaryTouch, scaledSecondaryTouch, lastSecondaryTouch, deltaSecondaryTouch);
            }
        }
        // Reset Events
        //if (panEventLast && !panEventThis) PanTouchGestureChangedEvent.Invoke(Vector2.zero);
        //if (pinchEventLast && !pinchEventThis) PinchTouchGestureChangedEvent.Invoke(0.0f);

        // Update last touches
        lastPrimaryTouch = scaledPrimaryTouch;
        lastSecondaryTouch = scaledSecondaryTouch;
        lastTertiaryTouch = scaledTertiaryTouch;
        lastPrimaryTouchDown = IsPrimaryTouchContactDown;
        lastSecondaryTouchDown = IsSecondaryTouchContactDown;
        lastTertiaryTouchDown = IsTertiaryTouchContactDown;
        //panEventLast = panEventThis;
        //pinchEventLast = pinchEventThis;
        //pinchEventThis = false;
        //pinchEventThis = false;
    }

    // Camera Gesture Calculations
    public const float panDotThreshold = -0.5f;
    public const float panMultiplier = 7.5f;
    //public const float panLimit = 50.0f;
    public const float pinchDotThreshold = 0.75f;
    public const float pinchMultiplier = 15.0f;
    //public const float pinchLimit = 10.0f;

    private void CheckPanGesture(float dot, Vector2 delta1, Vector2 delta2)
    {
        if (dot > panDotThreshold)
        {
            Vector2 target = -(delta1 + delta2) * panMultiplier;
            //if (!Mathf.Approximately(target.sqrMagnitude, 0)) target *= Math.Min(target.magnitude, panLimit) / target.magnitude;
            PanTouchGestureChangedEvent.Invoke(target);
            //panEventThis = true;
        }
    }

    private void CheckPinchGesture(float dot, Vector2 pos1, Vector2 pos1Last, Vector2 pos1Delta, Vector2 pos2, Vector2 pos2Last, Vector2 pos2Delta)
    {
        if (dot < pinchDotThreshold)
        {
            float target = (pos1Delta - pos2Delta).magnitude * pinchMultiplier;
            //target = Math.Min(target, pinchLimit);
            int mult = (pos2 - pos1).sqrMagnitude - (pos2Last - pos1Last).sqrMagnitude > 0 ? 1 : -1;
            PinchTouchGestureChangedEvent.Invoke(target * mult);
            //pinchEventThis = true;
        }
    }
}
