using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BouncepadIndicator : MonoBehaviour
{
    // Configuration
    [Tooltip("Textbox to display bouncepad count.")]
    public TMP_Text textbox;
    [Tooltip("Color while bouncepad count is under par.")]
    public Color underParColor;
    [Tooltip("Color while bouncepad count is over par.")]
    public Color overParColor;

    // Working Variables
    private Image image;
    private LevelManager.LevelProperties currentLevel;
    private Action updateAction;


    // Instance Functions
    private void Start()
    {
        image = GetComponent<Image>();

        image.color = underParColor;
        updateAction = GetLevelProperties;
    }

    private void Update()
    {
        updateAction.Invoke();
    }
    
    private void GetLevelProperties()
    {
        currentLevel = LevelManager.GetCurrentLevelProperties();
        if (currentLevel.Name == SceneManager.GetActiveScene().name)
        {
            updateAction = delegate{UpdateCounter(); CheckPar();};
        }
        else
        {
            // There are no properties for this level, therefore no need to check for par
            updateAction = UpdateCounter;
        }
    }

    private void UpdateCounter()
    {
        textbox.text = "" + currentLevel.bouncepadsUsed;
    }

    private void CheckPar()
    {
        if (currentLevel.bouncepadsUsed > currentLevel.bouncepadPar)
        {
            image.color = overParColor;
            updateAction = UpdateCounter; // Players can't "unuse" a bouncepad, so no need to keep checking
        }
    }
}
