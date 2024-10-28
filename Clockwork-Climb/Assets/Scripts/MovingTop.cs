using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovingTop : MonoBehaviour
{
    public float speed;
    public float maxRange;
    private float flip;
    // Start is called before the first frame update
    void Start()
    {
        speed = 0.05f;
        Cuckoo.CuckooBucketted.AddListener(IncreaseSpeed);
        flip = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += new Vector3(speed * flip, 0f, 0f);
        if(Mathf.Abs(transform.position.x) > maxRange)
        {
            flip = flip * -1f;
        }
    }

    private void IncreaseSpeed()
    {
        speed += 0.01f;
    }

}
