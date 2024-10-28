using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyGenerator : MonoBehaviour
{

    public GameObject RedGears;
    private float xpos;
    private float ypos;
    private float angle;
    public int gearAmount;
    public float radius;
    public int levelCounter;

    // Start is called before the first frame update
    void Start()
    {
        if(levelCounter == 1)
        {
            angle = 12.566f / gearAmount;
            MakeTheLevel();
        }
        else if (levelCounter == 2)
        {
            angle = 12.566f / gearAmount;
            MakeAnotherLevel();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeTheLevel()
    {
        for(int i = 0; i < gearAmount; i++)
        {
            xpos = radius * Mathf.Cos(angle * i);
            ypos = radius * Mathf.Sin(angle * i);
            Instantiate(RedGears, new Vector2(-xpos, ypos + 24), Quaternion.identity);
            radius += 0.3f;
        }
    }
    public void MakeAnotherLevel()
    {
        for(int i = 0; i < gearAmount; i++)
        {
            xpos = radius * Mathf.Cos(angle * i);
            ypos = radius * Mathf.Sin(angle * i);
            Instantiate(RedGears, new Vector2(-xpos, ypos), Quaternion.identity);
        }
    }


}
