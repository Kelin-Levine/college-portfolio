using UnityEngine;

public class StretchBetweenTransforms : MonoBehaviour
{
    // Configuration
    [Tooltip("The first transform to stretch between.")]
    public Transform transform1;
    [Tooltip("The second transform to stretch between.")]
    public Transform transform2;

    // Reused Variables
    Vector3 diff;
    Vector3 scale;


    // Instance Functions
    private void Update()
    {
        diff = transform2.position - transform1.position;
        transform.SetPositionAndRotation(
            (transform1.position + transform2.position) / 2,
            Quaternion.LookRotation(diff));
        scale = transform.localScale;
        transform.localScale = new(scale.x, scale.y, diff.magnitude);
    }
}
