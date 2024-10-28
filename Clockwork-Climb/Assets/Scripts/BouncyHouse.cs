using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BouncyHouse : MonoBehaviour
{
    public static UnityEvent CuckooCollectedEvent = new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            Destroy(other.gameObject);
            CuckooCollectedEvent.Invoke();
        }
    }
}
