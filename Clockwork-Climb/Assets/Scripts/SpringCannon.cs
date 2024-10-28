using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringCannon : MonoBehaviour
{

    public bool isRotating;
    public float maxRotationAngle;
    public float rotationSpeed;
    public float startRotation;
    public GameObject point, aimPoint;
    public Animator cannonAnimator;
    public GameObject particleEffect;

    // Working
    private Rigidbody2D rb;
    private readonly List<Cuckoo> cuckooBirds = new();
    private Cuckoo lastFired;
    private float lastFireTime;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isRotating)
        {
            float rotationAngle = maxRotationAngle * Mathf.Sin(Time.time * rotationSpeed); //this should probably be changed, maybe lerp or linear idk
            rb.MoveRotation(startRotation + rotationAngle);
        }

        foreach(Cuckoo thisCuckoo in cuckooBirds)
        {
            thisCuckoo.transform.position = point.transform.position;
            //thisCuckoo.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            //thisCuckoo.transform.rotation = point.transform.rotation;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Cuckoo"))
        {
            Cuckoo cuckoo = other.gameObject.GetComponent<Cuckoo>();
            if (cuckoo == lastFired && Time.time < lastFireTime + 0.1f) return;
            cuckooBirds.Add(cuckoo);
            other.enabled = false;
            cuckoo.ChangeState(Cuckoo.CuckooState.PAUSE);
        }
    }

    public void SpringCuckoo()
    {
        if (cuckooBirds.Count == 0) return;
        StartCoroutine(DelaySpringCuckoo());
    }

    private IEnumerator DelaySpringCuckoo()
    {
        yield return new WaitForEndOfFrame();

        //Vector2 aiming = ((Vector2) (aimPoint.transform.position - point.transform.position)).normalized;
        //Vector2 aiming = new(-Mathf.Sin(rb.rotation * Mathf.Deg2Rad), Mathf.Cos(rb.rotation * Mathf.Deg2Rad));

        cuckooBirds[0].GetComponent<Collider2D>().enabled = true;
        cuckooBirds[0].ChangeState(Cuckoo.CuckooState.AIRBORNE);
        cuckooBirds[0].IsFacingRight = transform.up.x > 0;
        //cuckoo.IsFacingRight = aiming.x > 0;
        cuckooBirds[0].GetComponent<Rigidbody2D>().velocity += (Vector2) transform.up * 200; // new Vector2(100, 100);
        //cuckooBirds[0].GetComponent<Rigidbody2D>().velocity += aiming * 200;
        //cuckooBirds[0].GetComponent<Rigidbody2D>().velocity += new Vector2(1000 * Mathf.Sin(rb.rotation * Mathf.Deg2Rad), -1000 * Mathf.Cos(rb.rotation * Mathf.Deg2Rad));
        //cuckooBirds[0].transform.rotation = Quaternion.identity;

        lastFired = cuckooBirds[0];
        lastFireTime = Time.time;
        cuckooBirds.RemoveAt(0);
        cannonAnimator.SetTrigger("CannonFire");
        StartCoroutine(SpawnParticles());
        SoundManager.sound.MakeCannonSound();
    }

    private IEnumerator SpawnParticles()
    {
        GameObject particles = Instantiate(particleEffect, point.transform.position, point.transform.rotation);
        yield return new WaitForSeconds(5);
        if (particles != null) Destroy(particles);
    }
}

