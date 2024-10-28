using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectibleScript : MonoBehaviour
{
    public static UnityEvent SomethingWasCollected = new UnityEvent();
    public static UnityEvent GearFearCollected = new UnityEvent();

    public bool special; //for GearFear


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Cuckoo") && !special)
        {
            SomethingWasCollected.Invoke();
            SoundManager.sound.MakeGoldenGearSound();
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Cuckoo") && special)
        {
            GearFearCollected.Invoke();
            SoundManager.sound.MakeGoldenGearSound();
            Destroy(gameObject);
        }
    }

}
