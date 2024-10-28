using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;


public enum GameState { Menu, GetReady, Playing, Oof, Win}

public class GameManagerScript : MonoBehaviour
{
    /// <summary>Singleton instance</summary>
    public static GameManagerScript current;

    public static GameState currentState { get; private set; } 
    public int cuckooCounter;
    private int airCuckooScore; //normal score for individual levels
    public int airCuckooHighScore; //high score across many tries
    public int bouncyHouseScore, bouncyHouseHighScore, sortsScore, sortsHighScore, bucketsScore, bucketsHighScore, flappyCuckooScore, flappyCuckooHighScore;
    public int cuckooCarScore;
    public int cuckooCarHighScore;
    public static UnityEvent<GameState> StateChanged = new();
    public static UnityEvent LevelCompleted = new UnityEvent();
    public static UnityEvent BonusLevelCompleted = new UnityEvent();
    public static UnityEvent ActivateCuckoos = new UnityEvent();
    private bool isWinnable;
    public TextMeshProUGUI gameText;
    public TextMeshProUGUI mainGameText;



    // Awake is called before start, after scene change (if active and persisted from previous scene), and when going from inactive to active
    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(this);
    }

    void Start()
    {
        Cuckoo.CuckooReachedGoalEvent.AddListener(GoalReached);
        Cuckoo.CuckooExistsEvent.AddListener(CuckooExists);
        UIManager.ResetLevelEvent.AddListener(ResetTheLevel);
        BonusLevelManager.AirCuckoosEvent.AddListener(AirCuckoos);
        Cuckoo.CuckooDiedBonusEvent.AddListener(DeadCuckoo);
        CarScript.CarCrash.AddListener(DeadCuckoo);
        BonusLevelManager.BonusDone.AddListener(DeadCuckoo); //all these three do the same bc I'm super disorganized, my bad sry
        //Cuckoo.CuckooDiedCozyEvent.AddListener(CozyDeadCuckoo);

        gameText.enabled = false;
        mainGameText.enabled = false;
        isWinnable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }




//Level Managing
    public void ResetTheLevel() //probably should not be static, but referenced somewhere else...?
    {
        cuckooCounter = 0;
        isWinnable = true;
        gameText.enabled = false;
        mainGameText.enabled = false;
        StartCoroutine(ResetStatsLater());
    }

    private IEnumerator ResetStatsLater()
    {
        yield return null;
        LevelManager.LevelProperties currentLevel = LevelManager.GetCurrentLevelProperties();
        currentLevel.bouncepadsUsed = 0;
        currentLevel.goldenGearTaken = false;
        currentLevel.timerGoing = false;
        Debug.Log(currentLevel.bouncepadsUsed);
    }




//Cuckoo Tracking
    private void CuckooExists() //something to be called when cuckoos spawn to set a counter, probably a better way idk
    {
        cuckooCounter += 1;
    }

    private void DeadCuckoo()
    {
        if(SceneManager.GetActiveScene().name == "AirCuckoos")
        {
            cuckooCounter -= 1;
            gameText.text = cuckooCounter + " Cuckoos remaining";
            isWinnable = false;
        }
        else if (SceneManager.GetActiveScene().name == "BouncyHouse")
        {
            //SetBonusHighScore(FindObjectOfType<BonusLevelManager>().GetComponent<BonusLevelManager>().bounces, ref bouncyHouseScore, ref bouncyHouseHighScore);
            bouncyHouseScore = FindObjectOfType<BonusLevelManager>().bounces;
            if(bouncyHouseScore < bouncyHouseHighScore) //bc its a lowScore, not a high score
            {
                bouncyHouseHighScore = bouncyHouseScore;
            }
            mainGameText.enabled = true;
            mainGameText.text = "Score: " + bouncyHouseScore + "\nBest Score: " + bouncyHouseHighScore;
            BonusLevelCompleted.Invoke();
        }
        else if(SceneManager.GetActiveScene().name == "CuckooCar")
        {
            SetBonusHighScore(FindObjectOfType<BonusLevelManager>().yards, ref cuckooCarScore, ref cuckooCarHighScore);  
            mainGameText.enabled = true;
            mainGameText.text = "Score: " + cuckooCarScore + "\nHigh Score: " + cuckooCarHighScore;
            BonusLevelCompleted.Invoke();
        }
        else if (SceneManager.GetActiveScene().name == "Sorts")
        {
            SetBonusHighScore(FindObjectOfType<BonusLevelManager>().sorted, ref sortsScore, ref sortsHighScore);
            mainGameText.enabled = true;
            mainGameText.text = "Score: " + sortsScore + "\nHigh Score: " + sortsHighScore;
            BonusLevelCompleted.Invoke();
        }
        else if (SceneManager.GetActiveScene().name == "Buckets")
        {
            SetBonusHighScore(FindObjectOfType<BonusLevelManager>().buckets, ref bucketsScore, ref bucketsHighScore);
            mainGameText.enabled = true;
            mainGameText.text = "Score: " + bucketsScore + "\nHigh Score: " + bucketsHighScore;
            BonusLevelCompleted.Invoke();
        }
        else if (SceneManager.GetActiveScene().name == "FlappyCuckoo")
        {
            SetBonusHighScore(FindObjectOfType<BonusLevelManager>().flaps, ref flappyCuckooScore, ref flappyCuckooHighScore);
            mainGameText.enabled = true;
            mainGameText.text = "Score: " + flappyCuckooScore + "\nHigh Score: " + flappyCuckooHighScore;
            BonusLevelCompleted.Invoke();
            
        }

    }


    private void GoalReached()
    {
        cuckooCounter -= 1;
        if(cuckooCounter == 0 && isWinnable == true && UIManager.S.currentDifficulty != Difficulty.cozy && SceneManager.GetActiveScene().name != "FlappyCuckoo")
        {
            LevelCompleted.Invoke();
            ChangeGameState(GameState.Win);
        }
        else if(UIManager.S.currentDifficulty == Difficulty.cozy && SceneManager.GetActiveScene().name != "FlappyCuckoo")
        {
            LevelCompleted.Invoke();
            ChangeGameState(GameState.Win);
        }
        else if(SceneManager.GetActiveScene().name == "FlappyCuckoo" && cuckooCounter == 0)
        {
            DeadCuckoo();
        }
    }





//Bonus Level Stuff
    private void AirCuckoos()
    {
        gameText.enabled = true;
        gameText.text = cuckooCounter + " Cuckoos remaining";
        mainGameText.enabled = true;
        SoundManager.sound.PlayAnthemMusic();
        StartCoroutine(AirCuckoosTimer());
    }

    private IEnumerator AirCuckoosTimer()
    {
        mainGameText.text = "Bonus Game: Air Cuckoos!";
        yield return new WaitForSeconds(1f);
        mainGameText.text = "Keep as many cuckoos alive as you can!";
        yield return new WaitForSeconds(2f);
        mainGameText.text = "Ready?";
        yield return new WaitForSeconds(0.5f);
        mainGameText.text = "3";
        yield return new WaitForSeconds(1f);
        mainGameText.text = "2";
        yield return new WaitForSeconds(1f);
        mainGameText.text = "1";
        yield return new WaitForSeconds(1f);
        mainGameText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        ActivateCuckoos.Invoke();

        for(int i = 30; i > 0; i--)
        {
            if(cuckooCounter > 0)
            {
                mainGameText.text = i + "s";
                yield return new WaitForSeconds(1f);
            }
            else{
                mainGameText.text = "You lose! Score: 0";
                //loser art
                //loser sound
                yield return new WaitForSeconds(2f);
                i = 0;
            }
        }

        airCuckooScore = cuckooCounter;
        if(airCuckooScore > airCuckooHighScore)
        {
            airCuckooHighScore = airCuckooScore;
        }
        mainGameText.text = "Your score: " + airCuckooScore + "\nYour high score: " + airCuckooHighScore;
        if(airCuckooScore > 72)
        {
            Debug.Log("Cheater/Tryhard");
        }
        BonusLevelCompleted.Invoke();
        //Invoke should give retry or menu buttons and do that:    mainGameText.enabled = false;

        //eventually put a countdown animation here (meaning up above in the countdown section) maybe?
    }

    public void StopAirCuckoosTimer()
    {
        StopCoroutine(AirCuckoosTimer());
    }

    public void SetBonusHighScore(int points, ref int score, ref int highScore)
    {
        score = points;
        if(score > highScore)
        {
            highScore = score;
        }
    }



//Gamestate

    public static void ChangeGameState(GameState nextState)
    {
        if (nextState == currentState) return; // Don't do anything if the state isn't actually changing

        switch(currentState)
        {

            //pre change check... ig...
            case GameState.Menu:
                break;

            case GameState.GetReady:
                break;

            case GameState.Playing:
                break;

            case GameState.Oof:
                break;

            case GameState.Win:
                break;
        }

        //change
        currentState = nextState;
        StateChanged.Invoke(currentState);

        //post check?
        switch(currentState)
        {
            case GameState.Menu:
                break;

            case GameState.GetReady:
                break;

            case GameState.Playing:
                break;

            case GameState.Oof:
                break;

            case GameState.Win:
                break;
        }

    }





}


//what does this script actually need to do?
//first: Keep track of cuckoos and have an event trigger thingy to tell UIManager to go to the next level
//second: keep track of stars/collectibles for every level individually... or maybe UIManager should do that, at least communicate with UIManager maybe to tell that
//third: Notice when cuckoos die and the level has to restart (first and third go together)