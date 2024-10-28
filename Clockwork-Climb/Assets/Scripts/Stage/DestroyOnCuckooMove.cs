using UnityEngine;

public class DestroyOnCuckooMove : MonoBehaviour
{
    private void OnEnable()
    {
        Cuckoo.CuckooStartMovingEvent.AddListener(DestroyMe);
    }

    private void OnDisable()
    {
        Cuckoo.CuckooStartMovingEvent.RemoveListener(DestroyMe);
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
