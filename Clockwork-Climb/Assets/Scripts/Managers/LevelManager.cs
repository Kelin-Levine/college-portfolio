using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections;



public class LevelManager : MonoBehaviour
{
    public static LevelManager S;
    public static UnityEvent<int> GiveStarsEvent = new();
    private static readonly Dictionary<string, LevelProperties> levelProperties = new();
    private static Dictionary<string, LevelStats> levelStats = new();
    public int starCounter;
    public int[] specificStarCounters;
    public TextMeshProUGUI levelText;
    public Image[] pauseCollectibles;
    public Color OnColor;
    public Color OffColor;


    //Image arrays
    public Image[] winStars, timeStars, goldenGearStars, bouncepadParStars;

    /// <summary>
    /// If no level properties exist for the given name, returns an empty LevelProperties.
    /// </summary>
    public static LevelProperties GetLevelProperties(string name)
    {
        try { return levelProperties[name]; }
        catch (KeyNotFoundException) { return new LevelProperties();}
    }
    /// <summary>
    /// If no level properties exist for the given name, returns null.
    /// </summary>
    public static LevelStats GetLevelStats(string name)
    {
        try { return levelStats[name]; }
        catch (KeyNotFoundException) { return null;}
    }

    /// <summary>
    /// If no level properties exist for the given name, returns an empty LevelProperties.
    /// </summary>
    public static LevelProperties GetCurrentLevelProperties()
    {
        return GetLevelProperties(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// If no level properties exist for the given name, returns null.
    /// </summary>
    public static LevelStats GetCurrentLevelStats()
    {
        return GetLevelStats(SceneManager.GetActiveScene().name);
    }


    private void Awake()
    {
        if (S == null) S = this;
        else if (S != this) Destroy(this);
    }

    // Start is called before the first frame update
    private void Start()
    {
        ReloadSaveData();

        CollectibleScript.SomethingWasCollected.AddListener(OnGoldenGearCollected);
        GameManagerScript.LevelCompleted.AddListener(LevelWon);
        GameManagerScript.BonusLevelCompleted.AddListener(SaveAllData);
        Cuckoo.CuckooStartMovingEvent.AddListener(StartLevelTimer);
        UIManager.LevelSelectMenuOn.AddListener(ShowStars);
    }

    // Update is called once per frame
    private void Update()
    {
        /*
        tic tac toe:
        ha ha i drew a penis
        X |   |   | O
        __|___|___/__
        X | O-|-O-|
        __|___|___\__
          |   |   | O
          |   | X |
        */
    }

    public static void SaveAllData()
    {
        // Tracks: levelsUnlocked from UIManager, high scores from bonus games, levelStats

        FileManager.SaveData saveData = new(levelStats, UIManager.S.levelsUnlocked, new int[]{GameManagerScript.current.airCuckooHighScore, GameManagerScript.current.bouncyHouseHighScore, GameManagerScript.current.cuckooCarHighScore, GameManagerScript.current.sortsHighScore, GameManagerScript.current.bucketsHighScore, GameManagerScript.current.flappyCuckooHighScore});
        if (FileManager.WriteToFile(FileManager.savePath, saveData))
        {
            Debug.Log("Successfully saved data.");
        }
    }

    public static void ReloadSaveData()
    {
        LoadAllData();
        S.PopulateLevelProperties();
    }

    private static void LoadAllData()
    {
        // Tracks: levelsUnlocked from UIManager, high scores from bonus games, levelStats

        // Load defaults
        levelStats = new();
        UIManager.S.levelsUnlocked = 1;
        GameManagerScript.current.airCuckooHighScore = 0;
        GameManagerScript.current.bouncyHouseHighScore = 0;
        GameManagerScript.current.cuckooCarHighScore = 0;
        GameManagerScript.current.sortsHighScore = 0;
        GameManagerScript.current.bucketsHighScore = 0;
        GameManagerScript.current.flappyCuckooHighScore = 0;

        // Load from file
        if (FileManager.LoadFromFile(FileManager.savePath, out FileManager.SaveData saveData))
        {
            levelStats = saveData.levelStats;
            UIManager.S.levelsUnlocked = saveData.levelsUnlocked;
            try
            {
                GameManagerScript.current.airCuckooHighScore = saveData.bonusHighScores[0];
                GameManagerScript.current.bouncyHouseHighScore = saveData.bonusHighScores[1];
                GameManagerScript.current.cuckooCarHighScore = saveData.bonusHighScores[2];
                GameManagerScript.current.sortsHighScore = saveData.bonusHighScores[3];
                GameManagerScript.current.bucketsHighScore = saveData.bonusHighScores[4];
                GameManagerScript.current.flappyCuckooHighScore = saveData.bonusHighScores[5];

            } catch (IndexOutOfRangeException) {}
            
            Debug.Log("Successfully loaded save data.");
        }
    }

    private void PopulateLevelProperties()
    {
        // Creating a new LevelProperties with configuration automatically adds it to the dictionary.
        new LevelProperties("1.1", 1, 15.0f, winStars[0], timeStars[0], goldenGearStars[0], bouncepadParStars[0]);
        new LevelProperties("1.2", 2, 20.0f, winStars[1], timeStars[1], goldenGearStars[1], bouncepadParStars[1]);
        new LevelProperties("1.3", 2, 25.0f, winStars[2], timeStars[2], goldenGearStars[2], bouncepadParStars[2]); //time limit note: technically possible, cutting a little closer than the others
        new LevelProperties("1.4", 1, 15.0f, winStars[3], timeStars[3], goldenGearStars[3], bouncepadParStars[3]);
        new LevelProperties("1.5", 1, 17.0f, winStars[4], timeStars[4], goldenGearStars[4], bouncepadParStars[4]);
        new LevelProperties("1.6", 1, 10.0f, winStars[5], timeStars[5], goldenGearStars[5], bouncepadParStars[5]);
        //hour two (highly subject to change, could be a lot lower if I was good which I'm not, just personal bests for now to be safe)
        new LevelProperties("2.1", 1, 11.0f, winStars[6], timeStars[6], goldenGearStars[6], bouncepadParStars[6]);
        new LevelProperties("2.2", 6, 55.0f, winStars[7], timeStars[7], goldenGearStars[7], bouncepadParStars[7]);
        new LevelProperties("2.3", 0, 22.0f, winStars[8], timeStars[8], goldenGearStars[8], bouncepadParStars[8]);
        new LevelProperties("2.4", 25, 75.0f, winStars[9], timeStars[9], goldenGearStars[9], bouncepadParStars[9]);
        new LevelProperties("2.5", 50, 60.0f, winStars[10], timeStars[10], goldenGearStars[10], bouncepadParStars[10]);
        new LevelProperties("2.6", 10, 60.0f, winStars[11], timeStars[11], goldenGearStars[11], bouncepadParStars[11]); //idk about this one
        //hour three
        new LevelProperties("3.1", 2, 11.0f, winStars[12], timeStars[12], goldenGearStars[12], bouncepadParStars[12]);
        new LevelProperties("3.2", 1, 17.0f, winStars[13], timeStars[13], goldenGearStars[13], bouncepadParStars[13]);
        new LevelProperties("3.3", 2, 30.0f, winStars[14], timeStars[14], goldenGearStars[14], bouncepadParStars[14]);
        new LevelProperties("3.4", 3, 19.0f, winStars[15], timeStars[15], goldenGearStars[15], bouncepadParStars[15]);
        new LevelProperties("3.5", 2, 25.0f, winStars[16], timeStars[16], goldenGearStars[16], bouncepadParStars[16]);
        new LevelProperties("3.6", 6, 50.0f, winStars[17], timeStars[17], goldenGearStars[17], bouncepadParStars[17]); //idk about this one
        //hour four
        new LevelProperties("4.1", 4, 19.0f, winStars[18], timeStars[18], goldenGearStars[18], bouncepadParStars[18]);
        new LevelProperties("4.2", 1, 19.0f, winStars[19], timeStars[19], goldenGearStars[19], bouncepadParStars[19]); 
        new LevelProperties("4.3", 3, 20.0f, winStars[20], timeStars[20], goldenGearStars[20], bouncepadParStars[20]);
        new LevelProperties("4.4", 10, 60.0f, winStars[21], timeStars[21], goldenGearStars[21], bouncepadParStars[21]);
        new LevelProperties("4.5", 2, 15.0f, winStars[22], timeStars[22], goldenGearStars[22], bouncepadParStars[22]);
        new LevelProperties("4.6", 1, 25.0f, winStars[23], timeStars[23], goldenGearStars[23], bouncepadParStars[23]);
        //hour five (parameters not set yet)
        new LevelProperties("5.1", 4, 30.0f, winStars[24], timeStars[24], goldenGearStars[24], bouncepadParStars[24]);
        new LevelProperties("5.2", 12, 60.0f, winStars[25], timeStars[25], goldenGearStars[25], bouncepadParStars[25]);
        new LevelProperties("5.3", 0, 30.0f, winStars[26], timeStars[26], goldenGearStars[26], bouncepadParStars[26]);
        new LevelProperties("5.4", 3, 100.0f, winStars[27], timeStars[27], goldenGearStars[27], bouncepadParStars[27]);
        new LevelProperties("5.5", 15, 100.0f, winStars[28], timeStars[28], goldenGearStars[28], bouncepadParStars[28]);
        new LevelProperties("5.6", 6, 50.0f, winStars[29], timeStars[29], goldenGearStars[29], bouncepadParStars[29]);
        //hour six (parameters not set yet)
        new LevelProperties("6.1", 1, 30.0f, winStars[30], timeStars[30], goldenGearStars[30], bouncepadParStars[30]);
        new LevelProperties("6.2", 3, 40.0f, winStars[31], timeStars[31], goldenGearStars[31], bouncepadParStars[31]);
        new LevelProperties("6.3", 40, 70.0f, winStars[32], timeStars[32], goldenGearStars[32], bouncepadParStars[32]);
        new LevelProperties("6.4", 10, 50.0f, winStars[33], timeStars[33], goldenGearStars[33], bouncepadParStars[33]);
        new LevelProperties("6.5", 4, 32.0f, winStars[34], timeStars[34], goldenGearStars[34], bouncepadParStars[34]);
        new LevelProperties("6.6", 50, 200.0f, winStars[35], timeStars[35], goldenGearStars[35], bouncepadParStars[35]);

    }

    //level select stuff
    public void ShowStars()
    {
        starCounter = 0;
        specificStarCounters[0] = 0;
        specificStarCounters[1] = 0;
        specificStarCounters[2] = 0;
        foreach (KeyValuePair<string, LevelProperties> pair in levelProperties)
        {
            LevelProperties properties = pair.Value;
            properties.winStarsImage.enabled = properties.Stats.levelWon;
            properties.goldenGearStarsImage.enabled = properties.Stats.goldenGearCompleted;
            properties.timeStarsImage.enabled = properties.onTime;
            properties.bouncepadParImage.enabled = properties.underBouncePar;
            if(properties.Stats.levelWon)
            {
                starCounter += 1;
            }
            if(properties.Stats.goldenGearCompleted)
            {
                starCounter += 1;
                specificStarCounters[0] += 1;
            }
            if(properties.onTime)
            {
                starCounter += 1;
                specificStarCounters[1] += 1;
            }
            if(properties.underBouncePar)
            {
                starCounter += 1;
                specificStarCounters[2] += 1;
            }
        }
        DisplayLevelStars();
        Debug.Log("You have " + starCounter + " stars.");
        GiveStarsEvent.Invoke(starCounter);
    }




    private void OnGoldenGearCollected()
    {
        GetCurrentLevelProperties().goldenGearTaken = true;
    }

    public void TellMeAboutTheLevel()
    {
        LevelProperties currentLevel = GetCurrentLevelProperties();
        Debug.Log("Golden Gear is " + currentLevel.Stats.goldenGearCompleted);
        Debug.Log("Level Won is " + currentLevel.Stats.levelWon);
        Debug.Log("Under bounce par is " + currentLevel.underBouncePar);
        Debug.Log("Win on time is " + currentLevel.onTime);
        Debug.Log(currentLevel.TimeElapsed());
    }

    public void LevelWon()
    {
        GetCurrentLevelProperties().FinishLevel();
        TellMeAboutTheLevel();
        StartCoroutine(DeferSaveAllData());
    }

    private IEnumerator DeferSaveAllData()
    {
        yield return null;
        SaveAllData();
    }

    public void StartLevelTimer()
    {
        GetCurrentLevelProperties().StartTimer();
    }

    public void DisplayLevelStats() //during pause show level par
    {
        levelText.text = "\nTime par: " + GetCurrentLevelProperties().levelParTime + "s\nBouncepad par: " + GetCurrentLevelProperties().bouncepadPar;
        if(GetCurrentLevelProperties().onTime)
        {
            pauseCollectibles[0].color = OnColor;
        }
        else{
            pauseCollectibles[0].color = OffColor;
        }
        if(GetCurrentLevelProperties().underBouncePar)
        {
            pauseCollectibles[1].color = OnColor;
        }
        else{
            pauseCollectibles[1].color = OffColor;
        }
    }

    public void DisplayLevelStars()
    {
        levelText.text = specificStarCounters[0] + "\n" + specificStarCounters[1] + "\n" + specificStarCounters[2];
    }



    /// <summary>
    /// May be saved and reloaded between game launches.
    /// </summary>
    [Serializable]
    public class LevelStats
    {
        public string levelName;
        public bool levelWon, goldenGearCompleted;
        public int bouncepadsUsedBest;
        public float levelWinTimeBest;

        public LevelStats(string levelName)
        {
            this.levelName = levelName;
            levelWon = false;
            goldenGearCompleted = false;
            bouncepadsUsedBest = int.MaxValue;
            levelWinTimeBest = float.MaxValue;
        }
    }



    /// <summary>
    /// Reset on game shutdown.
    /// </summary>
    public class LevelProperties
    {
        //three stars... three cogs?
        public string Name { get; private set; }
        public LevelStats Stats { get; private set; }
        public bool onTime, underBouncePar, goldenGearTaken, timerGoing;
        public int bouncepadPar, bouncepadsUsed;
        public float levelParTime, startTime;
        public Image winStarsImage, timeStarsImage, goldenGearStarsImage, bouncepadParImage;

        public LevelProperties() {}

        public LevelProperties(string setName, int setBouncepadPar, float setLevelParTime, Image setWinStarsImage, Image setTimeStarsImage, Image setGoldenGearStarsImage, Image setBouncepadParImage)
        {
            // Set/get name and stats
            Name = setName;
            Stats = GetLevelStats(Name);
            if (Stats == null)
            {
                Debug.Log("Creating new level stats data for '" + Name + "'.");
                Stats = new LevelStats(Name);
                levelStats.Add(Name, Stats);
            }

            // Set defaults
            goldenGearTaken = false;
            timerGoing = false;
            bouncepadsUsed = 0;
            startTime = 0;

            // Set provided values
            bouncepadPar = setBouncepadPar;
            levelParTime = setLevelParTime;
            winStarsImage = setWinStarsImage;
            timeStarsImage = setTimeStarsImage;
            goldenGearStarsImage = setGoldenGearStarsImage;
            bouncepadParImage = setBouncepadParImage;

            // Read stars from stats
            onTime = Stats.levelWon && Stats.levelWinTimeBest <= levelParTime;
            underBouncePar = Stats.levelWon && Stats.bouncepadsUsedBest <= bouncepadPar;

            // Add to dictionary
            levelProperties.Remove(Name); // If no entry exists, does nothing
            levelProperties.Add(Name, this);
        }

        public void StartTimer()
        {
            if (!timerGoing)
            {
                startTime = Time.time;
                timerGoing = true;
                Debug.Log("Timer has been started");
            }
        }

        /// <summary>
        /// End the timer and update stars.
        /// Make sure to start the timer first!
        /// </summary>
        public void FinishLevel()
        {
            Stats.levelWon = true;
            UpdateWinOnTime();
            UpdateUsedParBouncepads();
            UpdateGoldenGearCompleted();
            timerGoing = false;
        }

        public float TimeElapsed()
        {
            return Time.time - startTime;
        }

        public bool UpdateUsedParBouncepads()
        {
            Stats.bouncepadsUsedBest = Math.Min(Stats.bouncepadsUsedBest, bouncepadsUsed);
            underBouncePar = (Stats.levelWon && Stats.bouncepadsUsedBest <= bouncepadPar) || underBouncePar;
            return underBouncePar;
        }

        public bool UpdateWinOnTime()
        {
            Stats.levelWinTimeBest = Math.Min(Stats.levelWinTimeBest, TimeElapsed());
            onTime = (Stats.levelWon && Stats.levelWinTimeBest <= levelParTime) || onTime;
            return onTime;
        }
        
        public bool UpdateGoldenGearCompleted()
        {
            Stats.goldenGearCompleted = (Stats.levelWon && goldenGearTaken) || Stats.goldenGearCompleted;
            return Stats.goldenGearCompleted;
        }
    }
}
