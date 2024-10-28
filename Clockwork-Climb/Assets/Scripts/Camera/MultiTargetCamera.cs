using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTargetCamera : MonoBehaviour
{
    public enum UpdateEvent
    {
        Update,
        LateUpdate,
        FixedUpdate,
    }


    // Configuration
    [Tooltip("Whether automatic camera movement is active. This has similar function to enabling the script.")]
    public bool isActive = true;
    [Tooltip("At what event should the camera update?")]
    public UpdateEvent updateEvent = UpdateEvent.FixedUpdate;
    [Tooltip("Something, probably a canvas, to deactivate when the game is lost.")]
    public GameObject deactivateOnLoss;
    [Tooltip("Targets for this object to keep in frame. Only acknowledged at start.")]
    public Transform[] startingTargets;
    [Tooltip("Whether to find and add any goals as targets at start.")]
    public bool targetTheGoals = false;
    [Tooltip("Whether to begin focused on only the goals, then switch to all targets after a few seconds.")]
    public bool startAtGoals = true;
    [Tooltip("Seconds to spend looking at goals before switching to all targets (if Start At Goals is enabled).")]
    public float startAtGoalsWait = 3.0f;
    /// <summary>Notice: unit changes after Start.</summary>
    [Tooltip("Rate to chase at.")]
    public float chaseRate;
    /// <summary>Notice: unit changes after Start.</summary>
    [Tooltip("Rate to zoom at.")]
    public float zoomRate;
    [Tooltip("Maximum size that the camera can expand to.")]
    public float maxSize;
    [Tooltip("Minimum size that the camera can expand to.")]
    public float minSize;
    [Tooltip("Distance for farthest targets to be from the edge of the camera view.")]
    public float padding;
    [Tooltip("Multiplier applied when chasing ahead of a target based on its velocity. Increase to chase farther ahead of a moving target.")]
    public Vector2 aheadMultiplier;
    [Tooltip("Pan offset from the center of where it pans to.")]
    public Vector2 offset;
    [Tooltip("Direction vector specifying the direction of the linear path for the camera to be locked to. Set to 0 to disable and let the camera move freely.")]
    public Vector2 pathDirection;
    [Tooltip("Y-axis intercept of the linear path for the camera to be locked to. If slope is undefined, this becomes the X-axis intercept.")]
    public float pathIntercept;
    [Tooltip("Global boundary in which the camera can view. Set extents in either axis to 0 to disable.")]
    public Bounds cameraBounds;
    [Tooltip("Z axis of the plane to focus on in perspective view.")]
    public float zPlane = 0.0f;

    // Public Variables
    /// <summary>The transforms that this camera will track.</summary>
    public HashSet<Transform> targets = new(), priorityTargets = new(), exclusiveTargets = new();

    // Working Variables
    private Camera cam;
    private Vector3 targetPosition;
    private float targetSize;
    private HashSet<Transform> targetsRef;
    private float notFixedChaseRate;
    private float notFixedZoomRate;

    // Reused Variables
    Vector3 targetSum, newPos;
    Vector2 maxXY, localPos, idvTarPos;
    float aspectRatioInv, newSize, chaseStep, zoomStep;
    bool pathXZero, pathYZero;


    // Instance Functions
    private void Awake()
    {
        targets.UnionWith(startingTargets);
    }

    private void Start()
    {
        cam = GetComponent<Camera>();

        notFixedChaseRate = chaseRate;
        notFixedZoomRate = zoomRate;
        chaseRate = CalculateContinuousLerpStep(chaseRate, Time.fixedDeltaTime);
        zoomRate = CalculateContinuousLerpStep(zoomRate, Time.fixedDeltaTime);
        
        targetPosition = transform.position;
        targetSize = cam.orthographicSize;

        if (targetTheGoals || startAtGoals)
        {
            HashSet<Transform> goalTargets = new();

            GameObject[] goals = GameObject.FindGameObjectsWithTag("Finish");
            foreach (GameObject goal in goals) goalTargets.Add(goal.transform);

            if (targetTheGoals) targets.UnionWith(goalTargets);
            if (startAtGoals) StartCoroutine(WaitThenRestoreTargetsBuffer(goalTargets));
        }

        Cuckoo.CuckooDiedEvent.AddListener(OnCuckooDeath);

        CameraUpdate(false);
    }

    private IEnumerator WaitThenRestoreTargetsBuffer(HashSet<Transform> goalTargets)
    {
        foreach (Transform target in goalTargets) AddExclusiveTarget(target);
        GameManagerScript.ChangeGameState(GameState.GetReady);
        yield return new WaitForSeconds(startAtGoalsWait);
        foreach (Transform target in goalTargets) RemoveExclusiveTarget(target);
        GameManagerScript.ChangeGameState(GameState.Playing);
    }

    public void AddTarget(Transform target)
    {
        targets.Add(target);
    }
    public void AddPriorityTarget(Transform target)
    {
        targets.Add(target);
        priorityTargets.Add(target);
    }
    public void AddExclusiveTarget(Transform target)
    {
        exclusiveTargets.Add(target);
    }
    public void RemoveTarget(Transform target)
    {
        priorityTargets.Remove(target);
        targets.Remove(target);
    }
    public void RemovePriorityTarget(Transform target)
    {
        priorityTargets.Remove(target);
    }
    public void RemoveExclusiveTarget(Transform target)
    {
        exclusiveTargets.Remove(target);
    }

    public void OnCuckooDeath(GameObject cuckoo)
    {
        deactivateOnLoss.SetActive(false);
        GetComponent<ManualCamera>().enabled = false;
        notFixedChaseRate = 2.0f;
        notFixedZoomRate = 2.0f;
        updateEvent = UpdateEvent.LateUpdate; // change to LateUpdate because FixedUpdate isn't run while the game is paused
        Transform shotTarget = new GameObject("Game Over Camera Target").transform;
        shotTarget.position = cuckoo.transform.position + Vector3.up * (minSize / 2.0f + 1.0f);
        AddExclusiveTarget(shotTarget);
        /* Vector3 shotPos = cuckoo.transform.position;
        shotPos.z = -15.0f;
        StartCoroutine(TranslateTo(shotPos, 2.0f)); */
    }

    /// <summary>
    /// Make sure that time is frozen/CameraUpdate() doesn't run while this is active, or else they will probably fight for control of the camera.
    /// </summary>
    private IEnumerator TranslateTo(Vector3 pos, float time)
    {
        Vector3 startingPos = transform.position;
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime < startTime + time)
        {
            yield return null;
            transform.position = Vector3.Lerp(startingPos, pos, (Time.unscaledTime - startTime) / time);
        }
    }

    private void Update()
    {
        CameraUpdateIfEvent(UpdateEvent.Update);
    }

    private void LateUpdate()
    {
        CameraUpdateIfEvent(UpdateEvent.LateUpdate);
    }

    private void FixedUpdate()
    {
        CameraUpdateIfEvent(UpdateEvent.FixedUpdate);
    }

    private void CameraUpdateIfEvent(UpdateEvent thisEvent)
    {
        if (updateEvent == thisEvent) CameraUpdate(true);
    }

    public void CameraUpdate(bool lerp)
    {
        if (!isActive) return;

        if (exclusiveTargets.Count > 0) targetsRef = exclusiveTargets;
        else if (priorityTargets.Count > 0) targetsRef = priorityTargets;
        else targetsRef = targets;

        aspectRatioInv = (float) Screen.height / Screen.width;

        // Filter null
        UpdateTrackedTargets();

        // Calculate targets
        if (targetsRef.Count > 0)
        {
            // Position (target = average position of targets)
            CalculateTargetPosition();
            // Lock positional target to linear path
            LockPositionToPath(ref targetPosition);
            // Size (target = size which keeps all targets in view)
            CalculateTargetSize();
            // Apply offset
            targetPosition += (Vector3) offset;
        }

        // Lerp to calculated targets
        if (lerp) LerpToTargets();
        else TeleportToTargets();

        // Clamp view to bounds
        ClampViewToBounds(ref newPos, ref newSize);

        // Apply
        ApplyView(newPos, newSize);
    }

    private void UpdateTrackedTargets()
    {
        targetsRef.RemoveWhere(trans => trans == null);
    }

    private void CalculateTargetPosition()
    {
        targetSum = Vector3.zero;
        foreach (Transform target in targetsRef)
        {
            idvTarPos = target.position;
            if (target.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)) idvTarPos += rb.velocity * aheadMultiplier;
            targetSum += (Vector3) idvTarPos;
        }
        targetPosition = targetSum / targetsRef.Count;
    }

    /// <summary>
    /// Change a position to be on the closest point on the camera's path.
    /// </summary>
    /// <param name="position">The position variable to change</param>
    public void LockPositionToPath(ref Vector3 position)
    {
        pathXZero = Mathf.Approximately(pathDirection.x, 0);
        pathYZero = Mathf.Approximately(pathDirection.y, 0);
        if (pathXZero && pathYZero) return;
        if (pathXZero)
        {
            position.x = pathIntercept;
        }
        else if (pathYZero)
        {
            position.y = pathIntercept;
        }
        else
        {
            // Solved with Mathway, a Chegg(R) service
            position.x = -(pathDirection.x * (pathDirection.y * pathIntercept - pathDirection.y * position.y - position.x * pathDirection.x))
                            / (pathDirection.y * pathDirection.y + pathDirection.x * pathDirection.x);
            position.y = pathDirection.y / pathDirection.x * position.x + pathIntercept;
        }
    }

    private void CalculateTargetSize()
    {
        maxXY = Vector2.zero;
        foreach (Transform target in targetsRef)
        {
            localPos = target.position - targetPosition;
            maxXY.x = Math.Max(maxXY.x, Math.Abs(localPos.x));
            maxXY.y = Math.Max(maxXY.y, Math.Abs(localPos.y));
        }
        targetSize = Math.Clamp(Math.Max(maxXY.x * aspectRatioInv, maxXY.y) + padding, minSize, maxSize);
    }

    private void LerpToTargets()
    {
        if (updateEvent == UpdateEvent.FixedUpdate)
        {
            chaseStep = chaseRate;
            zoomStep = zoomRate;
        }
        else
        {
            chaseStep = CalculateContinuousLerpStep(notFixedChaseRate, Time.unscaledDeltaTime);
            zoomStep = CalculateContinuousLerpStep(notFixedZoomRate, Time.unscaledDeltaTime);
        }
        // Position
        newPos = Vector2.Lerp(transform.position, targetPosition, chaseStep);
        newPos.z = transform.position.z;
        // Size
        newSize = Mathf.Lerp(cam.orthographic ? cam.orthographicSize : PerspToOrtho(transform.position.z), targetSize, zoomStep);
    }

    private void TeleportToTargets()
    {
        // Position
        newPos = targetPosition;
        newPos.z = transform.position.z;
        // Size
        newSize = targetSize;
    }

    /// <summary>
    /// Change a position and camera size to be within the camera's bounds.
    /// </summary>
    /// <param name="position">The position variable to change</param>
    /// <param name="size">The camera size variable to change</param>
    public void ClampViewToBounds(ref Vector3 position, ref float size)
    {
        if (Mathf.Approximately(cameraBounds.extents.x, 0) || Mathf.Approximately(cameraBounds.extents.y, 0)) return;

        float camOrthoSizeX = size / aspectRatioInv;
        
        float xClamped = cameraBounds.center.x;
        float xClampedSize = size;
        try { xClamped = Math.Clamp(position.x, cameraBounds.min.x + camOrthoSizeX, cameraBounds.max.x - camOrthoSizeX); }
        catch (ArgumentException) { xClampedSize = cameraBounds.extents.x * aspectRatioInv; }
        float yClamped = cameraBounds.center.y;
        float yClampedSize = size;
        try { yClamped = Math.Clamp(position.y, cameraBounds.min.y + size, cameraBounds.max.y - size); }
        catch (ArgumentException) { yClampedSize = cameraBounds.extents.y; }

        position = new Vector3(xClamped, yClamped, position.z);
        size = Math.Min(xClampedSize, yClampedSize);
    }

    public void ApplyView(Vector3 position, float size)
    {
        if (cam.orthographic)
        {
            transform.position = position;
            cam.orthographicSize = size;
        }
        else
        {
            position.z = OrthoToPersp(size);
            transform.position = position;
        }
    }

    /// <summary>
    /// Converts a perspective zoom (position on Z axis) to orthogonal zoom (size or Y extent of view).
    /// </summary>
    /// <param name="perspectiveZ">Position on Z axis that describes perspective zoom</param>
    /// <returns>Y extent of view describing orthogonal size</returns>
    public float PerspToOrtho(float perspectiveZ)
    {
        // Math.Tan(cam.fieldOfView / 2.0f) * (zPlane - perspectiveZ) == viewExtentY
        // zPlane - viewExtentY / Math.Tan(cam.fieldOfView / 2.0f) = perspectiveZ
        return (float) Math.Tan(cam.fieldOfView / 2.0f * Mathf.Deg2Rad) * (zPlane - perspectiveZ);
    }

    /// <summary>
    /// Converts an orthogonal zoom (size or Y extent of view) to perspective zoom (position on Z axis).
    /// </summary>
    /// <param name="viewExtentY">Y extent of view describing orthogonal size</param>
    /// <returns>Position on Z axis that describes perspective zoom</returns>
    public float OrthoToPersp(float viewExtentY)
    {
        return zPlane - viewExtentY / (float) Math.Tan(cam.fieldOfView / 2.0f * Mathf.Deg2Rad);
    }


    // Static Functions
    public static float CalculateContinuousLerpStep(float rate)
    {
        return CalculateContinuousLerpStep(rate, Time.deltaTime);
    }

    public static float CalculateContinuousLerpStep(float rate, float deltaTime)
    {
        return (float) -Math.Pow(Math.E, -rate * deltaTime) + 1;
    }
}
