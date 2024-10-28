using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Cuckoo : MonoBehaviour
{
    public enum CuckooState
    {
        /// <summary>Cuckoo has not had its state set yet.</summary>
        NONE,
        /// <summary>Cuckoo is inactive; standing still, waiting to be activated.</summary>
        STATIONARY,
        /// <summary>Cuckoo is inactive; paused for an action, probably not moving.</summary>
        PAUSE,
        /// <summary>Cuckoo is active; on the ground, marching forward.</summary>
        MARCHING,
        /// <summary>Cuckoo is active; in the air.</summary>
        AIRBORNE,
    }


    // Events
    /// <summary>Event for when cuckoo reaches goal to tell GameManager to do stuff.</summary>
    public static UnityEvent CuckooReachedGoalEvent = new();
    /// <summary>Event for when cuckoo spawns in to give GameManager a counter.</summary>
    public static UnityEvent CuckooExistsEvent = new();
    /// <summary>Event to say when a cuckoo dies ONLY IN BONUS ROUND(S).</summary>
    public static UnityEvent CuckooDiedBonusEvent = new();
    /// <summary>Event to say when a cuckoo dies.</summary>
    public static UnityEvent<GameObject> CuckooDiedEvent = new();
    /// <summary>Event for when any cuckoo starts moving.</summary>
    public static UnityEvent CuckooStartMovingEvent = new();
    public static UnityEvent CuckooBucketted = new(); //for bonus game


    // Configuration
    [Tooltip("Whether the cuckoo starts facing to the right, rather than facing to the left.")]
    public bool startFacingRight = true;
    [Tooltip("Alive or not so it doesn't double trigger.")]
    public bool stillAlive = true;

    [Tooltip("Speed the cuckoo walks at.")]
    public float walkSpeed = 2.0f;
    [Tooltip("Rate that the cuckoo accelerates at while marching.")]
    public float walkAcceleration = 9.0f;
    [Tooltip("A small amount of speed that the cuckoo experiences while airborne.")]
    public float airSpeed = 1.0f;
    [Tooltip("Rate that the cuckoo accelerates at while airborne.")]
    public float airAcceleration = 6.0f;
    [Tooltip("Maximum velocity that the cuckoo can have.")]
    public float maxVelocity = 100.0f;
    [Tooltip("Bounds of the cuckoo's box collider, offset from the center of the script's gameobject.")]
    public Bounds colliderBounds;
    [Tooltip("Minimum depth that a drop must have to be considered a ledge.")]
    public float ledgeDepth = 0.9f;
    [Tooltip("The amount of velocity that this cuckoo jumps with.")]
    public Vector2 jumpVelocity;
    [Tooltip("Maximum vertical velocity to have when hitting ground before immediately going from airborne to marching.")]
    public float groundingVelocity;
    [Tooltip("Gravity scale to be manipulated by steam vents.")]
    public float cuckooGravity;
    [Tooltip("Layers which are excluded from the cuckoo's ground/turning check.")]
    public LayerMask excludedGroundLayers;
    [Tooltip("GameObject of alive visual part, to be deactivated when the cuckoo dies.")]
    public GameObject aliveVisual;
    [Tooltip("GameObject of death visual part, to be activated when the cuckoo dies.")]
    public GameObject deathVisual;
    [Tooltip("Container GameObject for visual components, to be flipped when the cuckoo turns around.")]
    public GameObject visualContainer;
    [Tooltip("Animator of the cuckoo model.")]
    public Animator cuckooAnimator;
    [Tooltip("ID number to identify this cuckoo in the Sorts bonus level. Only used in the Sorts bonus level.")]
    public int sortsID = 0;

    // Current Condition
    public CuckooState State { get; private set; } = CuckooState.NONE;
    public bool IsFacingRight
    {
        set
        {
            if (IsActiveState()) SetIsFacingRight(value);
        }
        get => _directionMult > 0;
    }
    /// <summary>Skips active check; please prefer setting IsFacingRight</summary>
    private void SetIsFacingRight(bool val)
    {
        _directionMult = val ? 1 : -1;
        visualContainer.transform.localScale = new Vector3(_directionMult, visualContainer.transform.localScale.y, visualContainer.transform.localScale.z);
    }

    // Working Variables
    private Rigidbody2D rb;
    private MultiTargetCamera camMT;
    /// <summary>Do not set directly; use IsFacingRight</summary>
    private int _directionMult;

    // Reused Variables
    CastHitArrayHelper floorHit, frontCast;
    Vector2 castOrigin;
    float velocityMag;

    private bool isBucketted; //random thing for bonus, sry for disorganization
    
    // Instance Functions
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        camMT = Camera.main.GetComponent<MultiTargetCamera>();
    }

    private void Start()
    {
        aliveVisual.SetActive(true);
        deathVisual.SetActive(false);

        SetIsFacingRight(startFacingRight);

        ChangeState(CuckooState.STATIONARY);
        CuckooExistsEvent.Invoke();
        GameManagerScript.ActivateCuckoos.AddListener(ActivateIfStationaryNoReturn);
        if(UIManager.S.currentDifficulty == Difficulty.crazy)
        {
            StartCoroutine(CrazyActivation());
        }
    }
    private IEnumerator CrazyActivation()
    {
        yield return new WaitForSeconds(2f);
        ActivateIfStationaryNoReturn();
    }

    private void OnEnable()
    {
        StartCoroutine(LateFixedUpdateInvoker());

        camMT.AddTarget(transform);
    }

    private void OnDisable()
    {
        StopCoroutine(LateFixedUpdateInvoker());
        if (camMT != null) camMT.RemoveTarget(transform);
    }

    private void FixedUpdate()
    {
        EnforceVelocityLimit();

        switch (State)
        {
            case CuckooState.STATIONARY:
            case CuckooState.PAUSE:
                break;

            case CuckooState.MARCHING:
                MarchingUpdate();
                break;

            case CuckooState.AIRBORNE:
                AirborneUpdate();
                break;
        }
    }

    private void LateFixedUpdate()
    {
        EnforceVelocityLimit();

        switch (State)
        {
            case CuckooState.STATIONARY:
            case CuckooState.PAUSE:
                break;

            case CuckooState.MARCHING:
                MarchingLateUpdate();
                break;

            case CuckooState.AIRBORNE:
                AirborneLateUpdate();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            StartCoroutine(ReachedGoal(other.transform.position));
        }
        else if (other.CompareTag("Steam"))
        {
            rb.gravityScale = cuckooGravity;
        }
        else if (other.CompareTag("BucketTrigger"))
        {
            if(!isBucketted)
            {
                isBucketted = true;
                CuckooBucketted.Invoke();

            }
        }
        else if(other.CompareTag("OutOfBounds") &&  stillAlive)
        {
            stillAlive = false;
            AppearDead();
            if(UIManager.S.currentDifficulty != Difficulty.cozy)
            {
                CuckooDiedEvent.Invoke(gameObject);
            }
            else
            {
                GameManagerScript.current.cuckooCounter -= 1;
                if(GameManagerScript.current.cuckooCounter == 0)
                {
                    CuckooDiedEvent.Invoke(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                    Debug.Log("ANOTHER ONE BITES THE DUST");
                }
            }
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Steam"))
        {
            rb.gravityScale = 1;
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("OutOfBounds"))
        {
            if (SceneManager.GetActiveScene().name == "FlappyCuckoo")
            {
                AppearDead();
                FindObjectOfType<BonusLevelManager>().GetComponent<BonusLevelManager>().flaps = 0;
                UIManager.S.gameText.text = "You DIED!";
                CuckooDiedEvent.Invoke(gameObject);
                CuckooDiedBonusEvent.Invoke();
                
            }
            else if (SceneManager.GetActiveScene().name == "Sorts")
            {
                AppearDead();
                BonusLevelManager.CuckooMissedSortEvent.Invoke(gameObject);
                StartCoroutine(DestroyInAMomentPlease());
            }
            else
            {
                AppearDead();
                CuckooDiedBonusEvent.Invoke();
                Destroy(gameObject);
            }
        }

        if (IsActiveState())
        {
            FrontHitCheck();
        }
    }

    private IEnumerator DestroyInAMomentPlease()
    {
        yield return new WaitForSeconds(0.01f);
        Destroy(gameObject);
    }


    private void AppearDead()
    {
        SoundManager.sound.MakeCuckooDeathSound();
        // avatar troubles brought us here
        aliveVisual.SetActive(false);
        deathVisual.SetActive(true);
        //cuckooAnimator.SetTrigger("dead");
        //cuckooAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }




    private void MarchingUpdate()
    {
        if (FloorCast())
        {
            // I am on ground; walk forward
            AccelerateInFacingUpTo(walkAcceleration, -walkAcceleration, walkSpeed);
        }
        else
        {
            // I am not on ground; become airborne
            ChangeState(CuckooState.AIRBORNE);
        }
    }

    private void MarchingLateUpdate()
    {
        floorHit = FloorCast();
        if (floorHit)
        {
            // I am still on ground; check for jump ledge
            RaycastHit2D jumpLedgeHit = floorHit.CompareTag("JumpLedge");
            if (jumpLedgeHit && !EdgeCast())
            {
                // I am past a jump ledge; backstep and jump
                MoveInFacing(-jumpLedgeHit.distance);
                Jump();
            }
        }
        else
        {
            // I am no longer on ground; become airborne
            ChangeState(CuckooState.AIRBORNE);
        }
    }

    private void AirborneUpdate()
    {
        AccelerateInFacingUpTo(airAcceleration, 0, airSpeed);
    }

    private void AirborneLateUpdate()
    {
        if (FloorCast() && Math.Abs(rb.velocity.y) < groundingVelocity)
        {
            ChangeState(CuckooState.MARCHING);
        }
    }

    private void EnforceVelocityLimit()
    {
        velocityMag = rb.velocity.magnitude;
        if (velocityMag > maxVelocity) rb.velocity *= maxVelocity / velocityMag;
    }

    private void AccelerateInFacingUpTo(float rate, float minRate, float max)
    {
        rb.velocity += new Vector2(_directionMult * Math.Clamp(max - rb.velocity.x * _directionMult, minRate, rate), 0);
    }

    private void MoveInFacing(float distance)
    {
        transform.position += _directionMult * distance * Vector3.right;
    }

    private void Jump()
    {
        rb.velocity += new Vector2(jumpVelocity.x * _directionMult, jumpVelocity.y);
        ChangeState(CuckooState.AIRBORNE);
    }

    private IEnumerator ReachedGoal(Vector3 goalPos)
    {
        goalPos += Vector3.up;
        Vector3 startPos = goalPos;
        goalPos += Vector3.forward * 4.0f;

        ChangeState(CuckooState.PAUSE);
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;
        transform.position = startPos;
        cuckooAnimator.SetTrigger("win");
        cuckooAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        Debug.Log("Start");
        yield return new WaitForSecondsRealtime(1.0f);
        Debug.Log("Go");

        float startTime = Time.unscaledTime;
        while (Time.unscaledTime < startTime + 1.0f)
        {
            transform.position = Vector3.Lerp(startPos, goalPos, Time.unscaledTime - startTime);
            yield return null;
        }

        CuckooReachedGoalEvent.Invoke();
        Destroy(gameObject);
    }

    private void FrontHitCheck()
    {
        // Turn check
        frontCast = FrontCast();
        if (frontCast && InsideOverlap().Length == 0)
        {
            // Hit a wall; turn around
            IsFacingRight = !IsFacingRight;

            // If hit another cuckoo, activate it
            RaycastHit2D cuckooHit = frontCast.CompareTag("Cuckoo");
            if (cuckooHit)
            {
                cuckooHit.transform.GetComponent<Cuckoo>().ActivateIfStationary();
            }
        }
    }

    private Collider2D[] InsideOverlap()
    {
        return Physics2D.OverlapBoxAll(transform.position + colliderBounds.center, colliderBounds.extents * 2.0f - Vector3.one * 0.1f, 0, ~excludedGroundLayers).Where(hit => hit.transform != transform).ToArray();
    }

    private CastHitArrayHelper FrontCast()
    {
        castOrigin = transform.position + colliderBounds.center;
        castOrigin += new Vector2(GetBoundsXEdgeInFacing() + (0.06f * _directionMult), colliderBounds.max.y - 0.05f);

        return new CastHitArrayHelper(transform, Physics2D.BoxCastAll(castOrigin, new(0.1f, 0.01f), 0, Vector2.down, colliderBounds.extents.y * 2.0f - 0.10f, ~excludedGroundLayers));
    }

    private CastHitArrayHelper FloorCast()
    {
        castOrigin = transform.position + colliderBounds.center;
        castOrigin += new Vector2(GetBoundsXEdgeInFacing() - (0.05f * _directionMult), colliderBounds.min.y - 0.06f);

        return new CastHitArrayHelper(transform, Physics2D.BoxCastAll(castOrigin, new(0.01f, 0.1f), 0, Vector2.left * _directionMult, colliderBounds.extents.x * 2.0f - 0.1f, ~excludedGroundLayers));
    }

    private CastHitArrayHelper EdgeCast()
    {
        castOrigin = transform.position + colliderBounds.center;
        castOrigin += new Vector2(GetBoundsXEdgeInFacing(), colliderBounds.min.y);

        return new CastHitArrayHelper(transform, Physics2D.RaycastAll(castOrigin, Vector2.down, ledgeDepth, ~excludedGroundLayers));
    }

    private float GetBoundsXEdgeInFacing()
    {
        return IsFacingRight ? colliderBounds.max.x : colliderBounds.min.x;
    }

    private IEnumerator LateFixedUpdateInvoker()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            LateFixedUpdate();
        }
    }

    /// <summary>
    /// Is this cuckoo in a state where it is considered active?
    /// </summary>
    /// <returns>Whether the cuckoo is active</returns>
    public bool IsActiveState()
    {
        return State == CuckooState.MARCHING || State == CuckooState.AIRBORNE;
    }

    private void ActivateIfStationaryNoReturn()
    {
        ActivateIfStationary();
    }

    /// <summary>
    /// Activate this cuckoo (start marching) if currently stationary.
    /// </summary>
    /// <returns>Whether the cuckoo was activated</returns>
    public bool ActivateIfStationary()
    {
        if (State == CuckooState.STATIONARY)
        {
            ChangeState(CuckooState.MARCHING);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Change the state of this cuckoo.
    /// </summary>
    /// <param name="newState">The state to change to</param>
    public void ChangeState(CuckooState newState)
    {
        // Pre-change check
        switch (State)
        {
            case CuckooState.STATIONARY:
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                CuckooStartMovingEvent.Invoke();
                break;

            case CuckooState.PAUSE:
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                break;

            case CuckooState.MARCHING:
                break;

            case CuckooState.AIRBORNE:
                cuckooAnimator.SetBool("isGrounded", true);
                break;
        }

        // Change
        State = newState;

        // Post-change check
        switch (State)
        {
            case CuckooState.STATIONARY:
                camMT.RemovePriorityTarget(transform);
                cuckooAnimator.SetBool("isActive", false);
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                break;
                
            case CuckooState.PAUSE:
                cuckooAnimator.SetBool("isActive", false);
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                break;

            case CuckooState.MARCHING:
                camMT.AddPriorityTarget(transform);
                cuckooAnimator.SetBool("isActive", true);
                rb.velocity = new(rb.velocity.x, 0);
                FrontHitCheck();
                break;

            case CuckooState.AIRBORNE:
                camMT.AddPriorityTarget(transform);
                cuckooAnimator.SetBool("isGrounded", false);
                cuckooAnimator.SetBool("isActive", true);
                FrontHitCheck();
                break;
        }
    }
}


class CastHitArrayHelper
{
    public RaycastHit2D[] array;

    public CastHitArrayHelper(Transform transform, RaycastHit2D[] castArray)
    {
        array = castArray.Where(hit => hit.transform != transform).ToArray();
    }

    public static implicit operator bool(CastHitArrayHelper value)
    {
        return value != null && value.array != null && value.array.Length > 0;
    }

    public RaycastHit2D CompareTag(string tag)
    {
        foreach (RaycastHit2D hit in array)
        {
            if (hit.transform.CompareTag(tag)) return hit;
        }
        return new RaycastHit2D();
    }
}
