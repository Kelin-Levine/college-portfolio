using UnityEngine;

public class SortsBouncingTrigger : MonoBehaviour
{
    // Configuration
    [Tooltip("Direction to bounce incoming cuckoos in.")]
    public bool isBouncingRight = true;
    [Tooltip("Whether to destroy cuckoos that leave the trigger.")]
    public bool isFinal = false;


    // Instance Functions
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Cuckoo"))
        {
            other.GetComponent<Cuckoo>().IsFacingRight = isBouncingRight;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isFinal && other.CompareTag("Cuckoo"))
        {
            Destroy(other.gameObject);
        }
    }
}
