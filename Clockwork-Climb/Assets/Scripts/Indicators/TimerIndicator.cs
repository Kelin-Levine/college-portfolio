using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimerIndicator : MonoBehaviour
{
    // Configuration
    [Tooltip("Textbox to display time.")]
    public TMP_Text textbox;
    [Tooltip("Color while time is under par.")]
    public Color underParColor;
    [Tooltip("Color while time is over par.")]
    public Color overParColor;

    // Working Variables
    private Image image;
    private LevelManager.LevelProperties currentLevel;
    private Action updateAction; //don't do this

    // Reused Variables
    float roundedTime;


    // Instance Functions
    private void Start()
    {
        image = GetComponent<Image>();

        image.color = underParColor;
        updateAction = GetLevelProperties;

        // Stop changing indicator when level ends
        GameManagerScript.LevelCompleted.AddListener(Stop);
        GameManagerScript.BonusLevelCompleted.AddListener(Stop);
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
            updateAction = delegate{WaitForStart(delegate{UpdateTimer(); CheckPar();});}; // !?!?
        }
        else
        {
            // There are no properties for this level, therefore no need to check for par
            updateAction = delegate{WaitForStart(UpdateTimer);};
        }
    }

    private void WaitForStart(Action nextAction)
    {
        if (currentLevel.timerGoing) updateAction = nextAction;
    }

    private void UpdateTimer()
    {
        roundedTime = (float) Math.Round(currentLevel.TimeElapsed(), 1);
        textbox.text = roundedTime + (Mathf.Approximately(roundedTime, (int) roundedTime) ? ".0" : "");
    }

    private void CheckPar()
    {
        if (currentLevel.TimeElapsed() > currentLevel.levelParTime)
        {
            image.color = overParColor;
            updateAction = UpdateTimer; // Reverse time travel hasn't been invented yet, so no need to keep checking
        }
    }

    private void Stop()
    {
        Destroy(this);
    }
}
