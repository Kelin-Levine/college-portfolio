using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameObjectTiler : MonoBehaviour
{
    // Configuration
    [Tooltip("Visual object to tile in this object. Should be an existing object parented to this one.")]
    public GameObject visualObject;
    [Tooltip("How many seconds after shattering the tiles will despawn.")]
    public float shatterDespawnTime = 5.0f;
    [Tooltip("Maximum random linear velocity for shattering tiles.")]
    public Vector3 shatterVelocityMax;
    [Tooltip("Minimum random linear velocity for shattering tiles.")]
    public Vector3 shatterVelocityMin;
    [Tooltip("Maximum random angular velocity for shattering tiles.")]
    public Vector3 shatterAngularVelocityMax;
    [Tooltip("Minimum random angular velocity for shattering tiles.")]
    public Vector3 shatterAngularVelocityMin;

    // Working Variables
    private GameObject[] tiles;


    // Instance functions
    private void Start()
    {
        int tilesX = (int) Math.Round(transform.localScale.x, MidpointRounding.AwayFromZero);
        float scaleX = transform.localScale.x / tilesX / transform.localScale.x;
        float farX = 0.5f - (scaleX / 2.0f);

        int tilesY = (int) Math.Round(transform.localScale.y, MidpointRounding.AwayFromZero);
        float scaleY = transform.localScale.y / tilesY / transform.localScale.y;
        float farY = 0.5f - (scaleY / 2.0f);

        int tilesZ = (int) Math.Round(transform.localScale.z, MidpointRounding.AwayFromZero);
        float scaleZ = transform.localScale.z / tilesZ / transform.localScale.z;
        float farZ = 0.5f - (scaleZ / 2.0f);

        tiles = new GameObject[tilesX * tilesY * tilesZ];

        int l = 0;
        for (int i = 0; i < tilesX; i++)
        {
            for (int j = 0; j < tilesY; j++)
            {
                for (int k = 0; k < tilesZ; k++)
                {
                    tiles[l] = Instantiate(visualObject, gameObject.transform);
                    tiles[l].transform.localPosition = new(farX - (scaleX * i), farY - (scaleY * j), farZ - (scaleZ * k));
                    tiles[l].transform.localScale = new(scaleX, scaleY, scaleZ);
                    l++;
                }
            }
        }
        Destroy(visualObject);
    }

    public void Shatter()
    {
        GetComponent<Collider2D>().enabled = false;
        Rigidbody tileRB;
        foreach (GameObject tile in tiles)
        {
            tile.transform.SetParent(null);
            tileRB = tile.AddComponent<Rigidbody>();
            tileRB.velocity = new(
                UnityEngine.Random.Range(shatterVelocityMin.x, shatterVelocityMax.x),
                UnityEngine.Random.Range(shatterVelocityMin.y, shatterVelocityMax.y),
                UnityEngine.Random.Range(shatterVelocityMin.z, shatterVelocityMax.z));
            tileRB.angularVelocity = new(
                UnityEngine.Random.Range(shatterAngularVelocityMin.x, shatterAngularVelocityMax.x),
                UnityEngine.Random.Range(shatterAngularVelocityMin.y, shatterAngularVelocityMax.y),
                UnityEngine.Random.Range(shatterAngularVelocityMin.z, shatterAngularVelocityMax.z));
        }
        StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(shatterDespawnTime);
        foreach (GameObject tile in tiles)
        {
            Destroy(tile);
        }
        Destroy(gameObject);
    }
}
