using System;
using System.Collections;
using UnityEngine;

public class RandomCuckooSpawner : MonoBehaviour
{
    // Configuration
    [Tooltip("Time to wait before first spawn.")]
    public float initialWait;
    [Tooltip("Starting value of the spawn period (seconds between cuckoo spawn).")]
    public float spawnPeriodStart;
    [Tooltip("Minimum value that the spawn period can reach.")]
    public float spawnPeriodMin;
    [Tooltip("Rate at which spawn period changes (per second).")]
    public float spawnPeriodRate;
    [Tooltip("All types of cuckoos to spawn.")]
    public GameObject[] cuckoos;
    [Tooltip("Locations to randomly pick between when spawning a cuckoo.")]
    public Transform[] spawnLocations;

    // Working Variables
    [HideInInspector] public float spawnPeriod;
    private float nextSpawnTime;
    private Action rateAction;
    private bool begunSpawning = false;

    // Reused VariablesMultiTargetCamera
    private GameObject currentCuckoo;


    // Instance Functions
    private void Start()
    {
        rateAction = delegate{if (begunSpawning) rateAction = delegate{spawnPeriod = Math.Max(spawnPeriod + (spawnPeriodRate * Time.fixedDeltaTime), spawnPeriodMin);};};

        spawnPeriod = spawnPeriodStart;
        nextSpawnTime = Time.fixedTime + initialWait;
    }

    private void FixedUpdate()
    {
        while (nextSpawnTime < Time.fixedTime)
        {
            begunSpawning = true;
            currentCuckoo = cuckoos[UnityEngine.Random.Range(0, cuckoos.Length)];
            StartCoroutine(DelayedActivation(Instantiate(currentCuckoo, spawnLocations[UnityEngine.Random.Range(0, spawnLocations.Length)].position, currentCuckoo.transform.rotation)));
            nextSpawnTime += spawnPeriod;
        }
        rateAction.Invoke();
    }

    private IEnumerator DelayedActivation(GameObject cuckoo)
    {
        yield return null;
        cuckoo.GetComponent<Cuckoo>().ActivateIfStationary();
    }
}
