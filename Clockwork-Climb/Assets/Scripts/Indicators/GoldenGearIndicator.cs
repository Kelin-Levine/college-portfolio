using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoldenGearIndicator : MonoBehaviour
{
    // Configuration
    [Tooltip("Color while golden gear is not obtained.")]
    public Color unobtainedColor;
    [Tooltip("Color while golden gear is obtained.")]
    public Color obtainedColor;

    // Working Variables
    private Image image;
    private LevelManager.LevelProperties currentLevel;
    private Action updateAction;


    // Instance Functions
    private void Start()
    {
        image = GetComponent<Image>();

        image.color = unobtainedColor;
        updateAction = GetLevelProperties;
    }

    private void Update()
    {
        updateAction.Invoke();
    }

    private void GetLevelProperties()
    {
        currentLevel = LevelManager.GetCurrentLevelProperties();
        if (currentLevel.Name != SceneManager.GetActiveScene().name)
        {
            // There are no properties for this level, therefore no golden gear
            Destroy(gameObject);
            return;
        }
        updateAction = CheckGear;
    }

    private void CheckGear()
    {
        if (currentLevel.goldenGearTaken)
        {
            image.color = obtainedColor;
            Destroy(this); // Players can't "uncollect" a golden gear, so no need to keep checking
        }
    }
}
