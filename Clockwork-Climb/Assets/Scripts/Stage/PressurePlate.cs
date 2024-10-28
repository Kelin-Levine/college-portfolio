using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    // Configuration
    [Tooltip("Number of cuckoos required to sink this pressure plate.")]
    public int requiredCount;
    [Tooltip("Text that displays the number of cuckoos needed to sink the pressure plate.")]
    public TMP_Text displayText;
    [Tooltip("Collision layer of cuckoos.")]
    public LayerMask cuckooLayer;
    [Tooltip("Range, relative to the attached object, to start the search for cuckoos once enough weight is on the pressure plate.")]
    public Bounds relativeStandingRange;

    // Working Variables
    private Joint2D someJoint; //could be any joint attached to this object
    private HashSet<Collider2D> adjacentCuckoos;


    // Instance Functions
    private void Start()
    {
        someJoint = GetComponent<Joint2D>();

        displayText.text = "" + requiredCount;
    }

    private void FixedUpdate()
    {
        CountAdjacentCuckoos();
        displayText.text = "" + (requiredCount - adjacentCuckoos.Count);
    }

    private void OnJointBreak2D(Joint2D brokenJoint)
    {
        //CountAdjacentCuckoos();
        if (adjacentCuckoos.Count >= requiredCount)
        {
            // Destroy the joint
            displayText.text = "X";
            brokenJoint.connectedBody.constraints = RigidbodyConstraints2D.None;
            Joint2D[] joints = GetComponents<Joint2D>();
            foreach (Joint2D joint in joints) Destroy(joint);
            Destroy(this);
        }
    }

    private void CountAdjacentCuckoos()
    {
        // world's first practical use of recursion
        adjacentCuckoos = new();
        AddAllAndSurrounding(Physics2D.OverlapBoxAll(someJoint.connectedBody.transform.position + relativeStandingRange.center, relativeStandingRange.extents * 2.0f, 0, cuckooLayer));
    }

    private void AddAllAndSurrounding(Collider2D[] hitCuckoos)
    {
        foreach (Collider2D hit in hitCuckoos)
        {
            if (adjacentCuckoos.Add(hit))
            {
                Bounds bounds = hit.GetComponent<Cuckoo>().colliderBounds;
                AddAllAndSurrounding(Physics2D.OverlapBoxAll(hit.transform.position + bounds.center, bounds.extents * 2.0f + Vector3.zero * 0.2f, 0, cuckooLayer));
            }
        }
    }
}
