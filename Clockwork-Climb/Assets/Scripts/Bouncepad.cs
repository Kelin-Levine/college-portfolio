using System;
using System.Collections;
using UnityEngine;

public class Bouncepad : MonoBehaviour
{
    // Configuration
    [Tooltip("Whether this bouncepad should be placed instantly. If enabled, it will automatically configure and finalize itself upon creation.")]
    public bool instantlyPlace = false;
    [Tooltip("Whether this bouncepad can start decaying when a cuckoo hits it.")]
    public bool canDecay = true;
    [Tooltip("Whether this bouncepad will change color when it appears.")]
    public bool changeColor = true;
    [Tooltip("The maximum velocity (magnitude) that a cuckoo may have after hitting this bouncepad.")]
    public float maxBounceVelocity = 20.0f;
    [Tooltip("The minimum velocity (magnitude) that a cuckoo may have after hitting this bouncepad.")]
    public float minBounceVelocity = 2.0f;
    [Tooltip("A multiplier applied to a cuckoo's velocity when it hits this bouncepad.")]
    public float bounceVelocityMarkiplier = 1.0f;
    [Tooltip("Whether to reflect the velocity of cuckoos which hit this bouncepad; otherwise, cuckoos will be redirected in the normal direction of this bouncepad.")]
    public bool reflectCuckoos = true;
    [Tooltip("The collider which is hit by a cuckoo to trigger this bouncepad.")]
    public Collider2D bounceCollider;
    [Tooltip("Bounciness of the springy bit when the cuckoo hits the bouncepad")]
    public float springBounciness;
    [Tooltip("Rigidbody of the springy visual bit that bounces when the cuckoo hits the bouncepad.")]
    public Rigidbody2D springVisualRigidbody;
    [Tooltip("GameObject that is placed at the start of the bouncepad.")]
    public GameObject startPart;
    [Tooltip("GameObject that is placed at the end of the bouncepad.")]
    public GameObject endPart;
    [Tooltip("Renderers which will have their material color changed to indicate the bouncepad's state.")]
    public Renderer[] renders;
    [Tooltip("Transparent version of the material for Renders renderers, applied when fading out.")]
    public Material rendersTransparentMaterial;
    [Tooltip("Color that is applied while the bouncepad is being placed.")]
    public Color placingColor;
    [Tooltip("Color that is applied when the bouncepad is hit.")]
    public Color hitColor;
    [Tooltip("Color that is transitioned to between hitting the bouncepad and disappearing.")]
    public Color decayColor;
    [Tooltip("Renderers other than the main one which should fade out on decay.")]
    public Renderer[] decayRenders;
    [Tooltip("Transparent version of the material for Decay Renders renderers, applied when fading out.")]
    public Material decayRendersTransparentMaterial;
    [Tooltip("Game Objects which should be activated when the bouncepads is hit.")]
    public GameObject[] activateOnDecay;
    [Tooltip("The amount of time that the bouncepad remains for after being hit (seconds).")]
    public float decayLength;

    // Read Properties
    public Vector2 Normal { get; private set; } = Vector2.up;

    // Working Variables
    private Vector2 springStartingPos;
    private Color startColor;
    private Color startEmission;
    private float slope;
    private float yInt;
    private bool isDecaying = false;


    // Instance Functions
    private void Start()
    {
        springStartingPos = springVisualRigidbody.position;

        startPart.SetActive(false);
        endPart.SetActive(false);

        if (changeColor)
        {
            startColor = renders[0].material.color;
            startEmission = renders[0].material.GetColor("_EmissionColor");
            SetMainColor(placingColor, placingColor);
        }

        if (instantlyPlace)
        {
            InitializePlacement();
            UpdatePlacement(transform.right * transform.localScale.x);
            FinalizePlacement();
        }
    }

    /// <summary>
    /// Update the placement of this bouncepad.
    /// </summary>
    /// <param name="diffVector">The vector difference between the bouncepad position and the click position.</param>
    public void UpdatePlacement(Vector2 diffVector)
    {
        // Rotate and stretch to cursor position
        float diffMag = diffVector.magnitude;
        // Edge case: diffVector has no magnitude
        if (Mathf.Approximately(diffMag, 0))
        {
            transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);
            return;
        }

        // Pose visuals
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, diffVector));
        transform.localScale = new(diffMag, transform.localScale.y, transform.localScale.z);

        // Calculate linear equation
        Normal = Vector2.Perpendicular(diffVector).normalized;
        if (Normal.y < 0) Normal *= -1;
        //y = mx+b ; b = -mx1+y1
        if (Mathf.Approximately(diffVector.x, 0)) diffVector.x = 0.0001f * diffVector.x >= 0 ? 1 : -1; //lol
        slope = diffVector.y / diffVector.x;
        yInt = -slope * transform.position.x + transform.position.y;
    }

    /// <summary>
    /// Begin placement of this bouncepad.
    /// </summary>
    public void InitializePlacement()
    {
        bounceCollider.enabled = false;
    }

    /// <summary>
    /// Finalize placement of this bouncepad.
    /// </summary>
    public void FinalizePlacement()
    {
        if (Mathf.Approximately(transform.localScale.x, 0))
        {
            Destroy(gameObject);
            return;
        }

        bounceCollider.enabled = true;
        if (changeColor) StartCoroutine(ChangeColor(renders, 0.2f, startColor, startEmission));

        float xScaleInv = 1 / transform.localScale.x;
        startPart.transform.localScale = new(xScaleInv, 1, 1);
        endPart.transform.localScale = new(xScaleInv, 1, 1);
        startPart.SetActive(true);
        endPart.SetActive(true);
    }

    public void SetMainColor(Color main, Color emission)
    {
        SetMainColor(renders, main, emission);
    }

    public void SetMainColor(Renderer[] setRenders, Color main, Color emission)
    {
        foreach (Renderer setRender in setRenders)
        {
            setRender.material.color = main;
            setRender.material.SetColor("_EmissionColor", emission);
        }
    }

    public void SetColorAlpha(Renderer[] setRenders, float alpha)
    {
        foreach (Renderer setRender in setRenders)
        {
            Color colorMain = setRender.material.color;
            Color colorEmission = setRender.material.GetColor("_EmissionColor");
            colorMain.a = alpha;
            colorEmission.a = alpha;
            setRender.material.color = colorMain;
            setRender.material.SetColor("_EmissionColor", colorEmission);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Cuckoo"))
        {
            bool isCuckoo = other.gameObject.TryGetComponent<Cuckoo>(out Cuckoo cuckoo);
            if (!isCuckoo || cuckoo.IsActiveState())
            {
                SoundManager.sound.MakeTrampolineBounceSound();

                Rigidbody2D cuckooRB = other.rigidbody;
                
                cuckooRB.velocity *= bounceVelocityMarkiplier;

                float velocityMag = cuckooRB.velocity.magnitude;
                int cuckooAboveMult = IsAboveMult(other.gameObject.transform.position);
                // Apply force to springy visual bit
                springVisualRigidbody.MovePosition(springStartingPos);
                springVisualRigidbody.velocity = -cuckooAboveMult * springBounciness * Normal;
                // Reflect/redirect cuckoo
                if (Mathf.Approximately(velocityMag, 0))
                {
                    RedirectCuckoo(minBounceVelocity, cuckoo, cuckooRB, cuckooAboveMult);
                }
                else
                {
                    if (reflectCuckoos) ReflectCuckoo(cuckoo, cuckooRB);
                    else RedirectCuckoo(velocityMag, cuckoo, cuckooRB, cuckooAboveMult);

                    EnforceCuckooVelocityLimits(cuckooRB, velocityMag);
                }

                if (canDecay)
                {
                    StartDecaying();
                }
            }
        }
    }

    private void ReflectCuckoo(Cuckoo cuckoo, Rigidbody2D cuckooRB)
    {
        // The physics engine does this automatically if nothing else is done
        /* int cuckooAboveMult = IsAboveMult(other.gameObject.transform.position);
        cuckooRB.velocity = Vector2.Reflect(cuckooRB.velocity, normal) * (Vector2.Dot(cuckooRB.velocity, normal) > 0 ? 1 : -1); */
        if (cuckoo != null) cuckoo.IsFacingRight = cuckooRB.velocity.x > 0;
    }

    private void RedirectCuckoo(float velocityMag, Cuckoo cuckoo, Rigidbody2D cuckooRB, int cuckooAboveMult)
    {
        cuckooRB.velocity = cuckooAboveMult * velocityMag * Normal;
        if (cuckoo != null) cuckoo.IsFacingRight = Normal.x * cuckooAboveMult > 0;
    }

    private void EnforceCuckooVelocityLimits(Rigidbody2D cuckooRB, float velocityMag)
    {
        
        cuckooRB.velocity *= Math.Clamp(velocityMag, minBounceVelocity, maxBounceVelocity) / velocityMag;
    }

    private void StartDecaying()
    {
        if (!isDecaying)
        {
            StartCoroutine(Decay());
            isDecaying = true;
        }
    }

    private IEnumerator Decay()
    {
        foreach (GameObject activate in activateOnDecay) activate.SetActive(true);
        yield return ChangeColor(renders, decayLength, hitColor, decayColor);
        SetMaterial(decayRenders, decayRendersTransparentMaterial);
        SetMaterial(renders, rendersTransparentMaterial);
        SetMainColor(renders, decayColor, decayColor);
        StartCoroutine(FadeOut(decayRenders, 0.5f));
        yield return FadeOut(renders, 0.5f);
        Destroy(gameObject);
    }

    private IEnumerator ChangeColor(Renderer[] renderers, float length, Color startColor, Color endColor)
    {
        float startTime = Time.time;
        float step;
        Color color;
        while (Time.time < startTime + length)
        {
            step = (Time.time - startTime) / length;
            color = Color.Lerp(startColor, endColor, step);
            SetMainColor(renderers, color, color);
            //SetColorAlpha(decayRenders, 1 - step);
            yield return null;
        }
    }

    private void SetMaterial(Renderer[] renderers, Material material)
    {
        foreach (Renderer render in renderers) render.material = material;
    }

    private IEnumerator FadeOut(Renderer[] renderers, float length)
    {
        float startTime = Time.time;
        float step;
        while (Time.time < startTime + length)
        {
            step = (Time.time - startTime) / length;
            SetColorAlpha(renderers, 1 - step);
            yield return null;
        }
    }

    private void DrawLineFromCenter(Vector3 vector, Color color)
    {
        Vector3 centerPos = transform.position + transform.right * (transform.localScale.x / 2);
        Debug.DrawLine(centerPos, centerPos + vector, color, 9999999999999);
    }

    /// <summary>
    /// Determine whether a position is above or below the line representing this bouncepad, and return a number that can be conveniently multiplied for positivity.
    /// </summary>
    /// <param name="pos">The position to compare</param>
    /// <returns>1 if pos is higher than or equal to the line of this bouncepad; -1 otherwise</returns>
    public int IsAboveMult(Vector2 pos)
    {
        return slope * pos.x + yInt <= pos.y ? 1 : -1;
    }
}
