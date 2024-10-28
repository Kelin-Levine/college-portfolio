using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarScript : MonoBehaviour
{
    private bool active;

    public static UnityEvent CarCrash = new();

    // Start is called before the first frame update
    void Start()
    {
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Car") && active)
        {
            active = false;
            CarCrash.Invoke();
        }
    }


}
