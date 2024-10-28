using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ManualCamera : MonoBehaviour
{
    // Configuration
    [Tooltip("Whether manual camera movement is active. This has similar function to enabling the script.")]
    public bool isActive = false;
    [Tooltip("Rate to pan at.")]
    public float panRate;
    [Tooltip("Amount of pan required to start panning (squared).")]
    public float panThresholdSquared;
    [Tooltip("Rate to zoom at.")]
    public float zoomRate;
    [Tooltip("Amount of zoom required to start zooming.")]
    public float zoomThreshold;
    [Tooltip("Rate to decelerate at if no inputs are recieved.")]
    public float decelerateRate;
    [Tooltip("Maximum size that the camera can expand to. Set to <= 0 to use the values set for MultiTargetCamera.")]
    public float maxSize;
    [Tooltip("Minimum size that the camera can expand to. Set to <= 0 to use the values set for MultiTargetCamera.")]
    public float minSize;
    [Tooltip("Image to recolor to indicate the manual camera toggle.")]
    public Image toggleImage;
    [Tooltip("Color of Toggle Image when manual control is disabled.")]
    public Color disabledColor;
    [Tooltip("Color of Toggle Image when manual control is preferred.")]
    public Color preferredColor;
    [Tooltip("Color of Toggle Image when manual control is locked.")]
    public Color lockedColor;

    // Working Variables
    private Camera cam;
    private MultiTargetCamera camMT;
    private bool bindsSet = false;
    private Vector2 panVector;
    private float zoomScale;

    // Reused Variables
    Vector3 newPos;
    float actualMinSize, actualMaxSize, newSize, lerpStep;


    private void Start()
    {
        cam = GetComponent<Camera>();
        camMT = GetComponent<MultiTargetCamera>();

        //GameManagerScript.StateChanged.AddListener(GameStateUpdate);
    }

    private void OnEnable()
    {
        //GameStateUpdate();
        BindAll();
    }

    private void OnDisable()
    {
        UnbindAll();
    }

    private void GameStateUpdate(GameState callbackState = 0)
    {
        switch (GameManagerScript.currentState)
        {
            case GameState.Playing:
                BindAll();
                break;

            default:
                UnbindAll();
                break;
        }
    }

    private void BindAll()
    {
        if (bindsSet) return;
        InputManager.BindCameraToggleDown(ToggleCamMode);
        InputManager.BindCameraPanChanged(ChangePanDirection);
        InputManager.BindPanTouchGestureChanged(PanDirectionChanged);
        InputManager.BindCameraZoomChanged(ChangeZoomScale);
        InputManager.BindPinchTouchGestureChanged(ZoomScaleChanged);
        bindsSet = true;
    }

    private void UnbindAll()
    {
        if (!bindsSet) return;
        InputManager.UnbindCameraToggleDown(ToggleCamMode);
        InputManager.UnbindCameraPanChanged(ChangePanDirection);
        InputManager.UnbindCameraZoomChanged(ChangeZoomScale);
        bindsSet = false;
    }

    private void LateUpdate()
    {
        if (camMT.enabled && camMT.isActive) SetActive(false);
        toggleImage.color = isActive ? !camMT.enabled ? lockedColor : preferredColor : disabledColor;

        if (!isActive) return;
        
        // Translate position
        newPos = transform.position + (Vector3) (panRate * Time.unscaledDeltaTime * panVector);

        // Lock position to path
        camMT.LockPositionToPath(ref newPos);

        // Change camera size (zoom)
        actualMinSize = minSize > 0 ? minSize : camMT.minSize;
        actualMaxSize = maxSize > 0 ? maxSize : camMT.maxSize;
        newSize = Math.Clamp((cam.orthographic ? cam.orthographicSize : camMT.PerspToOrtho(transform.position.z)) - zoomRate * Time.unscaledDeltaTime * zoomScale, actualMinSize, actualMaxSize);

        // Clamp to bounds
        camMT.ClampViewToBounds(ref newPos, ref newSize);

        // Apply
        camMT.ApplyView(newPos, newSize);

        // Decelerate for next time
        lerpStep = MultiTargetCamera.CalculateContinuousLerpStep(decelerateRate);
        panVector *= 1.0f - lerpStep;
        zoomScale *= 1.0f - lerpStep;
        //Debug.Log("panVector: " + panVector + "; zoomScale: " + zoomScale);
    }

    private void ToggleCamMode(InputAction.CallbackContext context)
    {
        ClickManager.current.FinishSpawn(true);
        SwitchCam(camMT.enabled, true);
    }

    private void ChangePanDirection(InputAction.CallbackContext context)
    {
        PanDirectionChanged(InputManager.CameraPanPosition);
    }

    private void ChangeZoomScale(InputAction.CallbackContext context)
    {
        ZoomScaleChanged(InputManager.CameraZoomAxis);
    }

    /// <summary>
    /// Set whether this manual camera control is active.
    /// </summary>
    /// <param name="active">Whether is active</param>
    public void SetActive(bool active)
    {
        if (!active)
        {
            panVector = Vector2.zero;
            zoomScale = 0;
        }
        isActive = active;
    }

    /// <summary>
    /// Switch camera modes by enabling/disabling the automatic camera and enabling/disabling this manual camera.
    /// </summary>
    /// <param name="toManual">Whether to switch to the manual camera or automatic camera</param>
    /// <param name="persist">Whether the setting should persist through changes of the automatic camera's isActive (disable the automatic camera)</param>
    public void SwitchCam(bool toManual, bool persist)
    {
        camMT.isActive = !toManual;
        if (persist) camMT.enabled = !toManual;
        SetActive(toManual);
    }

    /// <summary>
    /// Change the direction of manual panning. This will disable the automatic camera and enable this manual camera.
    /// </summary>
    /// <param name="newPanVector">The new direction</param>
    public void PanDirectionChanged(Vector2 newPanVector)
    {
        if (newPanVector.sqrMagnitude >= panThresholdSquared)
        {
            ClickManager.current.FinishSpawn(true);
            SwitchCam(true, false);
            panVector = newPanVector;
        }
        else panVector = Vector2.zero;
        //Debug.Log("Pan vector: " + newPanVector + "; Camera position: " + transform.position);
    }

    /// <summary>
    /// Change the zoom amount (not overall zoom) of manual panning. This will disable the automatic camera and enable this manual camera.
    /// </summary>
    /// <param name="newZoomScale">The new zoom amount</param>
    public void ZoomScaleChanged(float newZoomScale)
    {
        if (Math.Abs(newZoomScale) >= zoomThreshold)
        {
            ClickManager.current.FinishSpawn(true);
            SwitchCam(true, false);
            zoomScale = newZoomScale;
        }
        else zoomScale = 0;
    }
}
