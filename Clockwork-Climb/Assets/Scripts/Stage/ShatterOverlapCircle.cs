using UnityEngine;

public class ShatterOverlapCircle : MonoBehaviour
{
    // Configuration
    [Tooltip("Radius of the circle.")]
    public float radius;
    [Tooltip("Center offset of the circle.")]
    public Vector2 offset;
    [Tooltip("Objects on these layers will be shattered if overlapping.")]
    public LayerMask destroyLayers;


    // Instance Functions
    private void FixedUpdate()
    {
        Collider2D[] overlapping = Physics2D.OverlapCircleAll((Vector2) transform.position + offset, radius, destroyLayers);
        foreach (Collider2D overlapped in overlapping) if (overlapped.TryGetComponent<GameObjectTiler>(out GameObjectTiler tiler)) tiler.Shatter();
    }
}
