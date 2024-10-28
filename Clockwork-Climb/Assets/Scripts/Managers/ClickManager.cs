using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/*
This script handles:
Spawning, positioning, and finalizing bouncepads;
and activating cuckoos on click.
*/

public class ClickManager : MonoBehaviour
{
    // Singleton Instance
    public static ClickManager current;


    // Configuration
    [Tooltip("Prefab to instantiate for bouncepads.")]
    public GameObject bouncepadPrefab;
    [Tooltip("Minimum length that a bouncepad may be to be placed.")]
    public float minBouncepadLength;
    [Tooltip("Collision layer of cuckoos.")]
    public LayerMask cuckooLayer;
    [Tooltip("Maximum amount of (resource) the player has.")]
    public float maxResource;
    [Tooltip("Rate at which (resource) regenerates (per second).")]
    public float resourceRegenRate;
    [Tooltip("Transform of the (resource) meter object.")]
    public Transform meterTransform;

    // Working Variables
    [HideInInspector] public float resourceAmt;
    private MultiTargetCamera camScript;
    private Bouncepad currentBouncepad;

    // Reused Variables
    Vector2 diffVector;
    float cost;


    // Static Functions
    /// <summary>
    /// Calculate the in-world coordinate position of the cursor's current position, automatically adapting for whether the camera is perspective (3D) or orthogonal (2D).
    /// Z is always 0.
    /// </summary>
    /// <returns>The 3D coordinates of the in-world cursor posiion ('z' is always 0)</returns>
    public static Vector3 ClickToWorldPos()
    {
        return Camera.main.orthographic ? ClickTo2dWorldPos() : ClickTo3dPlanePos(0);
    }

    /// <summary>
    /// Calculate the in-world coordinate position of a cursor position, automatically adapting for whether the camera is perspective (3D) or orthogonal (2D).
    /// Z is always 0.
    /// </summary>
    /// <param name="cursorPos">The cursor position in screen coordinates</param>
    /// <returns>The 3D coordinates of the in-world cursor posiion ('z' is always 0)</returns>
    public static Vector3 ClickToWorldPos(Vector2 cursorPos)
    {
        return Camera.main.orthographic ? ClickTo2dWorldPos(cursorPos) : ClickTo3dPlanePos(0, cursorPos);
    }

    /// <summary>
    /// Calculate the in-world coordinate position of the cursor's current position in 2D space.
    /// </summary>
    /// <returns>The 2D coordinates of the in-world cursor position ('z' is always 0)</returns>
    public static Vector3 ClickTo2dWorldPos()
    {
        return ClickTo2dWorldPos(InputManager.CursorPosition);
    }

    /// <summary>
    /// Calculate the in-world coordinate position of a cursor position in 2D space.
    /// </summary>
    /// <param name="cursorPos">The cursor position in screen coordinates</param>
    /// <returns>The 2D coordinates of the in-world cursor position ('z' is always 0)</returns>
    public static Vector3 ClickTo2dWorldPos(Vector2 cursorPos)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(cursorPos);
        worldPosition.z = 0;
        return worldPosition;
    }

    /// <summary>
    /// Calculate the in-world coordinate position of the cursor's current position in 3D space along a flat plane on the z axis.
    /// </summary>
    /// <param name="zPlane">The z axis position of the plane to cast to</param>
    /// <returns>The 3D coordinates of the in-world cursor position</returns>
    public static Vector3 ClickTo3dPlanePos(float zPlane)
    {
        return ClickTo3dPlanePos(zPlane, InputManager.CursorPosition);
    }

    /// <summary>
    /// Calculate the in-world coordinate position of a cursor position in 3D space along a flat plane on the z axis.
    /// </summary>
    /// <param name="zPlane">The z axis position of the plane to cast to</param>
    /// <param name="cursorPos">The cursor position in screen coordinates</param>
    /// <returns>The 3D coordinates of the in-world cursor position</returns>
    public static Vector3 ClickTo3dPlanePos(float zPlane, Vector2 cursorPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(cursorPos);
        float mag = (zPlane - ray.origin.z) / ray.direction.z;
        return ray.origin + ray.direction * mag;
    }


    // Instance Functions
    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
        else if (current != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        camScript = Camera.main.GetComponent<MultiTargetCamera>();

        resourceAmt = maxResource;

        InputManager.BindClickDown(OnClickDown);
        InputManager.BindWhileClickDown(WhileClickDown);
        InputManager.BindWhileClickUp(RegenResource);
        InputManager.BindClickUp(OnClickUp);
    }

    private void OnDisable()
    {
        InputManager.UnbindClickDown(OnClickDown);
        InputManager.UnbindWhileClickDown(WhileClickDown);
        InputManager.UnbindWhileClickUp(RegenResource);
        InputManager.UnbindClickUp(OnClickUp);
        FinishSpawn();
    }

    private void UpdateMeter()
    {
        UpdateMeter(resourceAmt);
    }

    private void UpdateMeter(float amount)
    {
        meterTransform.localScale = new Vector3(amount / maxResource, meterTransform.localScale.y, meterTransform.localScale.z);
    }

    private void RegenResource()
    {
        if (GameManagerScript.currentState != GameState.Playing) return;

        resourceAmt = Math.Min(resourceAmt + resourceRegenRate * Time.deltaTime, maxResource);
        UpdateMeter();
    }

    private void WhileClickDown()
    {
        if (GameManagerScript.currentState == GameState.Playing)
        {
            if (!UpdateSpawn())
            {
                RegenResource();
            }
        }
    }

    private bool UpdateSpawn()
    {
        return UpdateSpawn(InputManager.CursorPosition);
    }

    private bool UpdateSpawn(Vector2 cursorPos)
    {
        if (currentBouncepad == null) return false;
        
        // Compute diffVector and limit to remaining resource amount
        diffVector = ClickToWorldPos(cursorPos) - currentBouncepad.transform.position;
        cost = Math.Min(diffVector.magnitude, resourceAmt);
        diffVector = diffVector.normalized * cost;

        // Update placement of bouncepad
        if (cost >= minBouncepadLength)
        {
            if (!currentBouncepad.gameObject.activeSelf) currentBouncepad.gameObject.SetActive(true);
            currentBouncepad.UpdatePlacement(diffVector);
            UpdateMeter(resourceAmt - cost);
            return true;
        }
        else
        {
            if (currentBouncepad.gameObject.activeSelf) currentBouncepad.gameObject.SetActive(false);
            return false;
        }
    }

    private void OnClickDown()
    {
        // When the game is refocused, the input system sometimes needs a moment before correctly registering inputs. This is the simplest way to detect that.
        if (InputManager.CursorPosition == Vector2.zero) return;

        if (GameManagerScript.currentState == GameState.Playing)
        {
            Vector3 clickPos = ClickToWorldPos();
            // Check activation of cuckoos
            if (ActivateThingsAt(clickPos))
            {
                camScript.isActive = true;
            }
            else
            {
                // Start bouncepad spawn
                //StartCoroutine(DelaySpawnBouncepad(clickPos));
                SpawnBouncepad(clickPos);
            }
        }
    }

    /// <summary>
    /// Activate all "things" (cuckoos, cannons, etc.) at this position.
    /// </summary>
    /// <param name="pos">The position to look for things to activate at</param>
    /// <returns>Whether anything was activated</returns>
    private bool ActivateThingsAt(Vector3 pos)
    {
        Collider2D[] intersecting = Physics2D.OverlapCircleAll(pos, 0.75f);
        bool wasSomethingHit = false;
        foreach (Collider2D hit in intersecting)
        {
            if (hit.CompareTag("Cuckoo"))
            {
                if (hit.GetComponent<Cuckoo>().ActivateIfStationary()) wasSomethingHit = true;
            }
            else if (hit.CompareTag("Cannon"))
            {
                hit.GetComponent<SpringCannon>().SpringCuckoo();
                wasSomethingHit = true;
            }
        }
        return wasSomethingHit;
    }

    private void SpawnBouncepad(Vector3 pos)
    {
        currentBouncepad = Instantiate(bouncepadPrefab, pos, Quaternion.identity).GetComponent<Bouncepad>();
        currentBouncepad.InitializePlacement();
        camScript.isActive = false;
    }

    private IEnumerator DelaySpawnBouncepad(Vector3 pos)
    {
        yield return null;
        SpawnBouncepad(pos);
    }

    private void OnClickUp()
    {
        // Finalize bouncepad spawn
        if (GameManagerScript.currentState == GameState.Playing)
        {
            UpdateSpawn();
            FinishSpawn();
        }
        else
        {
            FinishSpawn(true);
        }
        // I spent an hour coming up with a way to defer this call until the game started playing again before realizing that a better solution could be done in one line.
        /* else if (currentBouncepad != null && !deferringFinalization) {
            Vector2 deferredCursorPos = InputManager.CursorPosition;
            StartCoroutine(DeferUntilPlaying(delegate {
                if (!InputManager.IsClickDown)
                {
                    UpdateSpawn(deferredCursorPos);
                    FinishSpawn(deferredCursorPos);
                }
                deferringFinalization = false;
            }));
            deferringFinalization = true;
        } */
    }

    private void CancelSpawn(InputAction.CallbackContext context)
    {
        FinishSpawn(true);
    }

    /// <summary>
    /// Force the currently spawning bouncepad to finish spawning, if there is one.
    /// </summary>
    /// <param name="cancel">Whether to force the bouncepad spawn to be cancelled</param>
    public void FinishSpawn(bool cancel = false)
    {
        FinishSpawn(InputManager.CursorPosition, cancel);
    }
    
    /// <summary>
    /// Force the currently spawning bouncepad to finish spawning, if there is one.
    /// </summary>
    /// <param name="cursorPos">Position of the cursor in screen coordinates</param>
    /// <param name="cancel">Whether to force the bouncepad spawn to be cancelled</param>
    public void FinishSpawn(Vector2 cursorPos, bool cancel = false)
    {
        if (currentBouncepad != null)
        {
            Vector2 diffVector = ClickToWorldPos(cursorPos) - currentBouncepad.transform.position;
            if (!cancel && diffVector.magnitude >= minBouncepadLength && !IsCurrentBouncepadOverlappingCuckoosNearCollision())
            {
                resourceAmt -= Math.Min(diffVector.magnitude, resourceAmt);
                currentBouncepad.FinalizePlacement();
                SoundManager.sound.MakeTrampolinePlacementSound();
                LevelManager.GetCurrentLevelProperties().bouncepadsUsed += 1; //adds to the levelManager's count of bouncepads used
            }
            else
            {
                Destroy(currentBouncepad.gameObject);
            }
            UpdateMeter();
            if (camScript != null) camScript.isActive = true;
        }
        currentBouncepad = null;
    }

    private bool IsCurrentBouncepadOverlappingCuckoosNearCollision()
    {
        Collider2D[] overlappedCuckoos = Physics2D.OverlapBoxAll(currentBouncepad.bounceCollider.bounds.center, currentBouncepad.transform.localScale, currentBouncepad.transform.rotation.eulerAngles.z, cuckooLayer);
        foreach (Collider2D cuckoo in overlappedCuckoos)
        {
            if (Physics2D.OverlapBox(cuckoo.bounds.center, cuckoo.transform.localScale * 3.0f, 0, ~(currentBouncepad.gameObject.layer | cuckooLayer))) return true;
        }
        return false;
    }

    private IEnumerator DeferUntilPlaying(Action callback)
    {
        while (GameManagerScript.currentState != GameState.Playing) yield return null;
        callback.Invoke();
    }
}
