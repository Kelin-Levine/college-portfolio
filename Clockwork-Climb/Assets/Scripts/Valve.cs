using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : MonoBehaviour
{

    public GameObject[] steamVents;
    private bool isTurned;


    // Start is called before the first frame update
    void Start()
    {
        SoundManager.sound.TurnSteamOn();
        SwitchVents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Cuckoo"))
        {
            SwitchVents();
            SoundManager.sound.MakeValveSound();
        }
    }

    public void SwitchVents()
    {
        for(int i = 0; i < steamVents.Length; i++)
        {
            if(i % 2 == 0 && isTurned)
            {
                steamVents[i].gameObject.SetActive(false);
            }
            else if(i % 2 == 1 && !isTurned)
            {
                steamVents[i].gameObject.SetActive(false);
            }
            else{
                steamVents[i].gameObject.SetActive(true);
            }
        }
        isTurned = !isTurned;
    }

}
