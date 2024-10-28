using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindUpKey : MonoBehaviour
{
    //wind up clock hands
    public GameObject[] clockHands;
    public bool[] isWinding; //makes sure it doesn't start itself over and over again with multiple triggers
    public int[] totalTicks;
    public float[] windTickSeconds; //time you have
    public float[] windSpeed; //amount turned per second

    //pendulums
    public GameObject[] pendulums;
    public bool[] isRotating;
    public float[] pendulumRate;
    public float[] maxPendulumAngle; // times 20 (i think maybe)
    public float[] startingOrientation;


    //clockhands
    public GameObject[] spinningClockHands;
    public bool[] isTicking;
    public float[] tickRate;
    public float[] secondsPerTick;
    private int flip; //set to 1 or -1 to flip rotation
    //do not have flippable spinning hands and tickable hands in the same scene
    public bool isNewVersion; //because good physis breaks good levels :(



    // Start is called before the first frame update
    void Start()
    {
        flip = 1;
        //start pendulum swining
        for(int i = 0; i < pendulums.Length; i++)
        {
            StartCoroutine(PendulumSwing(pendulums[i], isRotating[i], pendulumRate[i], maxPendulumAngle[i], startingOrientation[i]));

        }

        //start clock spinning
        for(int i = 0; i < spinningClockHands.Length; i++)
        {
            StartCoroutine(ClockTicking(spinningClockHands[i], isTicking[i], tickRate[i], secondsPerTick[i]));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Cuckoo"))
        {
            flip = flip * -1;
            SoundManager.sound.MakeWindUpSound();

            for(int i = 0; i < clockHands.Length; i++)
            {
                if(!isWinding[i])
                {
                    Debug.Log("Activated");
                    StartCoroutine(WindUp(clockHands[i], i, totalTicks[i], windTickSeconds[i], windSpeed[i]));
                }
            }
        }
    }

    private IEnumerator WindUp(GameObject _clockHand, int handNumber, int _totalTicks, float _windTickSeconds, float _windSpeed)
    {
        isWinding[handNumber] = true;
        for(int i = _totalTicks; i > 0; i--)
        {
            yield return new WaitForSeconds(_windTickSeconds);
            if(!isNewVersion)
            {
                _clockHand.GetComponent<Rigidbody2D>().rotation += _windSpeed;
            }
            else{
                _clockHand.GetComponent<Rigidbody2D>().MoveRotation(_clockHand.GetComponent<Rigidbody2D>().rotation + _windSpeed);
            }

        }
        isWinding[handNumber] = false;
    }

    private IEnumerator PendulumSwing(GameObject pendulum, bool stillGoing, float swingSpeed, float maxAngle, float startOrientation)
    {
        while (stillGoing)
        {
            float rotationNumber = maxAngle * Mathf.Sin(Time.time * swingSpeed); //help from unity forum here
            if(!isNewVersion)
            {
                pendulum.GetComponent<Rigidbody2D>().rotation = startOrientation + rotationNumber;
            }
            else{
                pendulum.GetComponent<Rigidbody2D>().MoveRotation(startOrientation + rotationNumber);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator ClockTicking(GameObject theClockHand, bool theStillGoing, float theTickSpeed, float secondsEachTick)
    {
        while (theStillGoing)
        {
            yield return new WaitForSeconds(secondsEachTick);
            if(!isNewVersion)
            {
                theClockHand.GetComponent<Rigidbody2D>().rotation += theTickSpeed * flip;
            }
            else{
                theClockHand.GetComponent<Rigidbody2D>().MoveRotation(theClockHand.GetComponent<Rigidbody2D>().rotation + (theTickSpeed));
            }

        }
    }


}
