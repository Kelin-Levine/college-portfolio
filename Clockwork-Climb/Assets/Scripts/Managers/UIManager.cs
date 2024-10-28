using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public enum Difficulty {cozy, normal, crazy}

public class UIManager : MonoBehaviour
{
    /// <summary>Singleton named S so it doesn't destroy on load.</summary>
    public static UIManager S; //singleton for UIManager
    public static UnityEvent ResetLevelEvent = new();
    public static UnityEvent LevelSelectMenuOn = new();


    //temporary stuff: DELETE LATER!! (But nrly)
    public Button DebuggingButton;
    public Button SecretButton;
    public bool DebugOn;
    private bool SecretOn;


    //pause menu stuff
    public Button UnpauseButton;
    public Button RestartButton;
    public Button SettingsButton;
    private bool settingsOn;
    private bool creditsOn;
    public Button MenuButton;    
    public Button PauseButton;
    
    //Settings stuff
    public Slider MusicSlider;
    public Slider SFXSlider;
    public Button CreditsButton;
    public Button DeleteDataButton1;
    public Button DeleteDataButton2;
    public Button DifficultyButton;
    public bool DifficultyDisplayed;
    public Button CozyButton;
    public Button NormalButton;
    public Button CrazyButton;
    public TextMeshProUGUI DifficultyText;
    public Difficulty currentDifficulty;
    public Image CreditsImage;
    public TextMeshProUGUI mainCreditText;
    public TextMeshProUGUI creditText;

    //winner menu stuff
    public Button NextLevelButton;
    public Button MenuButtonForWinnerScreen;
    public Button RestartLevelButtonForWinnerScreen;
    //loser menu stuff
    public Image blackout;
    public RectTransform blackoutMask;

    //level selector stuff
    public Button[] LevelChoices, BonusButtons;
    public int starCounterForUI;
    public Sprite[] bonusButtonIcons;
    public GameObject levelSelect; //used to turn off the fonts, could make another array for text but this is faster
    public Button ScrollRight;
    public Button ScrollLeft;
    public int levelsUnlocked;
    private bool isANewLevel; //to restrain levelsUnlocked
    public Sprite[] numberedButtons;
    public Sprite lockButton;
    public Button settingsForMenu;

    //mainscreen stuff
    public Button StartButton;
    public Image titleScreenImage;
    public Image titleScreenBackgroundImage;

    //gameplay stuff
    public TextMeshProUGUI gameText;
    public TextMeshProUGUI mainGameText;
    public TextMeshProUGUI levelText;
    public Image[] pauseCollectibles;
    public Image pauseGear; //bc I'm lazy and don't want to add it back into the array
    

    //collectibles stuff


    private void Awake()
    {
        if(S)
        {
            Destroy(gameObject);
        }
        else{
            S = this;
        }

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        StartMenuUI();
        GameManagerScript.LevelCompleted.AddListener(WinnerWinnerChickenDinner); //sry for naming, its a cuckoo, couldn't resist
        GameManagerScript.BonusLevelCompleted.AddListener(BonusFinishedUI); //once bonus level is finished gives buttons
        Cuckoo.CuckooDiedEvent.AddListener(LoseSequence);
        LevelManager.GiveStarsEvent.AddListener(TakeStars);
    }

    // Update is called once per frame
    void Update()
    {

    }

//Level selecting stuff

    public void LevelCompleted() //call this when all the cuckoos get to the end
    {
        Debug.Log("levelsUnlocked depends on BuildIndex, if it gets changes change LevelCompleted() in UIManager");
        if(SceneManager.GetActiveScene().buildIndex > levelsUnlocked) //buildIndex is one greater than the level number
        {
            levelsUnlocked += 1;
        }
    }
    public void StartNextLevel(string sceneName) //should take you to the given level on click with start menu button
    {
        Time.timeScale = 1;
        SoundManager.sound.TurnSteamOff();
        SceneManager.LoadScene(sceneName);
        GameManagerScript.ChangeGameState(GameState.Playing);
        SoundManager.sound.MakeStartJingleSound();
        GameplayUI();
        ResetLevelEvent.Invoke();
        //anything else for starting a level...?
    }
    public void StartCutscene(string sceneName)
    {
        Time.timeScale = 1;
        SoundManager.sound.TurnSteamOff();
        SoundManager.sound.PlayGameplayMusic();
        SceneManager.LoadScene(sceneName);
        GameManagerScript.ChangeGameState(GameState.Playing);
        TurnEverythingOff();
        PauseButton.gameObject.SetActive(true);
    }
    public void StartStartingVideo()
    {
        SceneManager.LoadScene("Cinematic scene");
        StartButton.gameObject.SetActive(false);
        titleScreenBackgroundImage.enabled = false;
        titleScreenImage.enabled = false;
        MenuButton.gameObject.SetActive(true);
    }
    public void DebuggingUnlock() //DELETE THIS LATER
    {
        if(SecretOn)
        {
            levelsUnlocked = 100;
            DebugOn = true;
            LevelSelectUI();
        }
    }
    public void SecretFunction()
    {
        StartCoroutine(Secret());
    }
    private IEnumerator Secret()
    {
        SecretOn = true;
        yield return new WaitForSeconds(5f);
        SecretOn = false;
    }




//winner menu stuff

    public void GoToNextLevel() //to be called for going up the scene index instead of loading specific level
    {
        if(SceneManager.GetActiveScene().name == "1.6")
        {
            StartCutscene("Cutscene 2");
        }
        else if(SceneManager.GetActiveScene().name == "2.6")
        {
            StartCutscene("cutscene 3");
        }
        else if(SceneManager.GetActiveScene().name == "3.4")
        {
            StartCutscene("cutscene 4");
        }
        else if(SceneManager.GetActiveScene().name == "3.6")
        {
            StartCutscene("Cutscene 5");
        }
        else if(SceneManager.GetActiveScene().name == "4.6")
        {
            StartCutscene("cutscene 6");
        }
        else if(SceneManager.GetActiveScene().name == "5.6")
        {
            StartCutscene("Cutscene 7");
        }


        else{
            SoundManager.sound.TurnSteamOff();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
            GameManagerScript.ChangeGameState(GameState.Playing);
            GameplayUI();
            //also an intro or whatever maybe idk

        }
    }
    private void WinnerWinnerChickenDinner()
    {
        SoundManager.sound.MakeWinJingle();
        LevelCompleted(); //needs to be fixed so you can't farm on one single level
        WinnerUI();
    }


//UI stuff

    private void TurnEverythingOff()
    {
        levelSelect.SetActive(false);
        CreditsImage.enabled = false;
        mainCreditText.enabled = false;
        creditText.enabled = false;
        UnpauseButton.gameObject.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        SettingsButton.gameObject.SetActive(false);
        MenuButton.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);
        SFXSlider.gameObject.SetActive(false);
        MusicSlider.gameObject.SetActive(false);
        CreditsButton.gameObject.SetActive(false);
        DeleteDataButton1.gameObject.SetActive(false);
        DeleteDataButton2.gameObject.SetActive(false);
        DifficultyButton.gameObject.SetActive(false);
        DifficultyText.enabled = false;
        CozyButton.gameObject.SetActive(false);
        NormalButton.gameObject.SetActive(false);
        CrazyButton.gameObject.SetActive(false);
        NextLevelButton.gameObject.SetActive(false);
        MenuButtonForWinnerScreen.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(false);
        titleScreenImage.enabled = false;
        titleScreenBackgroundImage.enabled = false;
        RestartLevelButtonForWinnerScreen.gameObject.SetActive(false);
        ScrollLeft.gameObject.SetActive(false);
        ScrollRight.gameObject.SetActive(false);        
        LevelManager.GetCurrentLevelProperties().timerGoing = false;
        blackout.enabled = false;
        blackoutMask.gameObject.SetActive(false);
        levelText.enabled = false;
        settingsForMenu.gameObject.SetActive(false);
        pauseGear.enabled = false;    
        SecretButton.gameObject.SetActive(false);
        DebuggingButton.gameObject.SetActive(false);
        foreach(Button thisButton in LevelChoices)
        {
            thisButton.gameObject.SetActive(false);
        }
        foreach(Button bonusButton in BonusButtons)
        {
            bonusButton.gameObject.SetActive(false);
        }
        foreach(Image thisImage in pauseCollectibles)
        {
            thisImage.enabled = false;
        }
    }
    private void GameplayUI()
    {
        TurnEverythingOff();
        StopAllCoroutines();
        SoundManager.sound.PlayGameplayMusic();
        PauseButton.gameObject.SetActive(true);
        GameManagerScript.ChangeGameState(GameState.Playing);
    }
    private void PauseMenuUI()
    {
        TurnEverythingOff();
        UnpauseButton.gameObject.SetActive(true);
        RestartButton.gameObject.SetActive(true);
        SettingsButton.gameObject.SetActive(true);
        settingsOn = false;
        MenuButton.gameObject.SetActive(true);
        GameManagerScript.ChangeGameState(GameState.Menu);
        if(LevelManager.GetCurrentLevelProperties().Name == SceneManager.GetActiveScene().name)
        {
            levelText.enabled = true;
            foreach(Image thisImage in pauseCollectibles)
            {
                thisImage.enabled = true;
            }
        }
    }
    private void SettingsUI()
    {
        DifficultyDisplayed = false;
        creditsOn = false;
        SFXSlider.gameObject.SetActive(true);
        MusicSlider.gameObject.SetActive(true);
        CreditsButton.gameObject.SetActive(true);
        DeleteDataButton1.gameObject.SetActive(true);
        DeleteDataButton2.gameObject.SetActive(false);
        DifficultyButton.gameObject.SetActive(true);
    }
    private void StartMenuUI()
    {
        TurnEverythingOff();
        StartButton.gameObject.SetActive(true);
        titleScreenImage.enabled = true;
        titleScreenBackgroundImage.enabled = true;
        GameManagerScript.ChangeGameState(GameState.Menu);
        SoundManager.sound.PlayMenuMusic();
    }
    private void WinnerUI()
    {
        TurnEverythingOff();
        MenuButtonForWinnerScreen.gameObject.SetActive(true);
        RestartLevelButtonForWinnerScreen.gameObject.SetActive(true);
        NextLevelButton.gameObject.SetActive(true);
        NextLevelButton.interactable = true;
    }
    private void LoserUI()
    {
        TurnEverythingOff();
        blackout.enabled = true;
        blackoutMask.gameObject.SetActive(true);
        MenuButtonForWinnerScreen.gameObject.SetActive(true);
        RestartLevelButtonForWinnerScreen.gameObject.SetActive(true);
        NextLevelButton.gameObject.SetActive(true);
        NextLevelButton.interactable = false;
    }
    private void LoseSequence(GameObject cuckoo)
    {
        GameManagerScript.ChangeGameState(GameState.Oof);
        ClickManager.current.FinishSpawn(true);
        Time.timeScale = 0;
        BlackoutCircleOn(cuckoo);
        StartCoroutine(DeferForSecondsRealtime(3.0f, LoserUI));
        Debug.Log("THIS JUST HAPPENED");
        //frowny face?
    }
    public void BlackoutCircleOn(GameObject target)
    {
        blackout.enabled = true;
        blackoutMask.gameObject.SetActive(true);
        StartCoroutine(BlackingOut(2, 0.85f));
        StartCoroutine(BlackoutMaskToPosition((Vector2) target.transform.position + Vector2.up * 0.5f, 2.0f, 20.0f));
    }
    private IEnumerator BlackingOut(float duration, float amount)
    {
        float startTime = Time.unscaledTime;
        Color color;
        while (Time.unscaledTime < startTime + duration)
        {
            color = blackout.color;
            color.a = Mathf.Lerp(0, amount, (Time.unscaledTime - startTime) / duration);
            blackout.color = color;
            yield return null;
        }
    }
    private IEnumerator BlackoutMaskToPosition(Vector2 position, float scaleDuration, float positionDuration)
    {
        float startTime = Time.unscaledTime;
        RectTransform canvasRect = blackoutMask.parent.GetComponent<RectTransform>();
        Vector2 viewPos;
        while (Time.unscaledTime < startTime + positionDuration)
        {
            // Set position
            viewPos = Camera.main.WorldToViewportPoint(position);
            //blackoutMask.anchorMin = viewPos;
            //blackoutMask.anchorMax = viewPos;
            //blackoutMask.anchoredPosition = viewPos;
            blackoutMask.anchoredPosition = new((viewPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f), (viewPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
            // Transition scale
            blackoutMask.transform.localScale = Vector3.one * Mathf.Lerp(1.0f, 0.1f, (Time.unscaledTime - startTime) / scaleDuration);
            yield return null;
        }
    }
    private IEnumerator DeferForSecondsRealtime(float seconds, Action action)
    {
        yield return new WaitForSecondsRealtime(seconds);
        action.Invoke();
    }
    private void LevelSelectUI()
    {
        StopAllCoroutines();
        Time.timeScale = 1;
        SoundManager.sound.PlayMenuMusic();
        gameText.enabled = false; //should probably be moved into everything, but for now leave them exclusively bonus, if it changes we need to set up a bonusinprogress UI, which is doable but nrn
        mainGameText.enabled = false;
        TurnEverythingOff();
        settingsOn = false;
        LevelSelectMenuOn.Invoke();
        levelSelect.SetActive(true);
        gameObject.SetActive(true);
        ScrollLeft.gameObject.SetActive(true);
        ScrollRight.gameObject.SetActive(true);    
        settingsForMenu.gameObject.SetActive(true);   
        DebuggingButton.gameObject.SetActive(true); 
        for(int i = 0; i < LevelChoices.Length; i++)
        {
            if(i < levelsUnlocked)
            {
                LevelChoices[i].gameObject.SetActive(true);
                LevelChoices[i].interactable = true;
                LevelChoices[i].enabled = true;
                LevelChoices[i].GetComponent<Image>().sprite = numberedButtons[i % 6]; //change if add more
                //changes from locked to unlocked
            }
            else{
                LevelChoices[i].gameObject.SetActive(true);
                LevelChoices[i].interactable = false;
                LevelChoices[i].GetComponent<Image>().sprite = lockButton;
            }
        }
        for(int i = 1; i <= BonusButtons.Length; i++) 
        {
            if((i * 24) <= starCounterForUI)
            {
                BonusButtons[i - 1].gameObject.SetActive(true);
                BonusButtons[i - 1].interactable = true;
                BonusButtons[i - 1].enabled = true;
                BonusButtons[i - 1].GetComponent<Image>().sprite = bonusButtonIcons[i - 1];
            }
            else{
                BonusButtons[i - 1].gameObject.SetActive(true);
                BonusButtons[i - 1].interactable = false;
                BonusButtons[i - 1].GetComponent<Image>().sprite = lockButton;
            }
        }
        foreach(Image thisImage in pauseCollectibles)
        {
            thisImage.enabled = true;
            thisImage.color = new Color(255, 255, 255, 255);
        }
        pauseGear.enabled = true;
        levelText.enabled = true;
        Debug.Log(starCounterForUI);
    }

    private void TakeStars(int lvlStarCounter)
    {
        starCounterForUI = lvlStarCounter;
        if(DebugOn)
        {
            starCounterForUI = 300;
        }
    }

    private void BonusFinishedUI()
    {
        StartCoroutine(BonusFinishedUIDelayed());
    }
    private IEnumerator BonusFinishedUIDelayed()
    {
        MenuButtonForWinnerScreen.gameObject.SetActive(false);
        RestartLevelButtonForWinnerScreen.gameObject.SetActive(false);
        NextLevelButton.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(3f);
        TurnEverythingOff();
        if(SceneManager.GetActiveScene().name == "FlappyCuckoo" && GameManagerScript.current.cuckooCounter != 0)
        {
            blackout.enabled = true;
            blackoutMask.gameObject.SetActive(true);
        }
        gameText.enabled = false; //should probably be moved into everything, but for now leave them exclusively bonus, if it changes we need to set up a bonusinprogress UI, which is doable but nrn
        mainGameText.enabled = false;
        MenuButtonForWinnerScreen.gameObject.SetActive(true);
        RestartLevelButtonForWinnerScreen.gameObject.SetActive(true);
    }


//pause menu stuff
    public void PauseTheGame() //pauses the game
    {
        PauseMenuUI();
        Time.timeScale = 0;
    }
    
    public void UnpauseTheGame()
    {
        Time.timeScale = 1;
        TurnEverythingOff();
        GameManagerScript.ChangeGameState(GameState.Playing);
        GameplayUI();
    }
    public void ReloadLevel()
    {
        ResetLevelEvent.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //if we add a little intro thing, like looking around or a countdown or whatever, that would go here... and also somewhere else probably
        UnpauseTheGame();
    }
    public void Settings() //sry this is public but the button accesses it...
    {
        if(!settingsOn)
        {
            SettingsUI();
            DifficultyButton.interactable = false;
            DeleteDataButton1.gameObject.SetActive(false);
            DeleteDataButton2.gameObject.SetActive(false);

            settingsOn = true;
        }
        else
        {
            PauseMenuUI();
        }
    }

//Settings stuff

    public void SettingsForMenu()
    {
        if(!settingsOn)
        {
            TurnEverythingOff();
            SettingsUI();
            DifficultyButton.interactable = true;
            DeleteDataButton1.interactable = true;
            DeleteDataButton2.interactable = true;
            settingsForMenu.gameObject.SetActive(true);
            settingsOn = true;
        }
        else
        {
            LevelSelectUI();
        }
    }
    public void DisplayCredits()
    {
        creditsOn = !creditsOn;
        SFXSlider.gameObject.SetActive(!creditsOn);
        MusicSlider.gameObject.SetActive(!creditsOn);
        DeleteDataButton1.gameObject.SetActive(!creditsOn);
        DeleteDataButton2.gameObject.SetActive(false);
        DifficultyButton.gameObject.SetActive(!creditsOn);
        CreditsImage.enabled = creditsOn;
        mainCreditText.enabled = creditsOn;
        creditText.enabled = creditsOn;
    }
    public void ShowDeleteDataButtons(bool show1)
    {
        DeleteDataButton1.gameObject.SetActive(show1);
        DeleteDataButton2.gameObject.SetActive(!show1);
    }
    /// <summary>Best not get this mixed up with TryDeleteSaveData()!</summary>
    public void DeleteSaveData()
    {
        FileManager.DeleteFile(FileManager.savePath);
        LevelManager.ReloadSaveData();
    }
    public void SwitchDifficulty()
    {
        DifficultyDisplayed = !DifficultyDisplayed;
        CozyButton.gameObject.SetActive(DifficultyDisplayed);
        NormalButton.gameObject.SetActive(DifficultyDisplayed);
        CrazyButton.gameObject.SetActive(DifficultyDisplayed);
        SFXSlider.gameObject.SetActive(!DifficultyDisplayed);
        MusicSlider.gameObject.SetActive(!DifficultyDisplayed);
        CreditsButton.gameObject.SetActive(!DifficultyDisplayed);
        DeleteDataButton1.gameObject.SetActive(!DifficultyDisplayed);
        DeleteDataButton2.gameObject.SetActive(false);
        DifficultyText.enabled = DifficultyDisplayed;
        SecretButton.gameObject.SetActive(DifficultyDisplayed);
        AdjustDifficultyButtons();
    }
    private void AdjustDifficultyButtons()
    {
        if(currentDifficulty == Difficulty.cozy)
        {
            CozyButton.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            NormalButton.GetComponent<Image>().color = new Color(0, 0, 0, 255);
            CrazyButton.GetComponent<Image>().color = new Color(0, 0, 0, 255);
            DifficultyText.text = "In Cozy mode, only once cuckoo must reach the goal. You heartlessly abandon the rest, but levels are easier!"; //working description, change later pls
        }
        else if(currentDifficulty == Difficulty.normal)
        {
            NormalButton.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            CozyButton.GetComponent<Image>().color = new Color(0, 0, 0, 255);
            CrazyButton.GetComponent<Image>().color = new Color(0, 0, 0, 255);
            DifficultyText.text = "In Normal mode, all three cuckoos must reach the goal without dying."; //working description, change later pls

        }
        else if (currentDifficulty == Difficulty.crazy)
        {
            CrazyButton.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            NormalButton.GetComponent<Image>().color = new Color(0, 0, 0, 255);
            CozyButton.GetComponent<Image>().color = new Color(0, 0, 0, 255);
            DifficultyText.text = "In Crazy mode, all cuckoos will activate at once with no time to preplan the levels. Crazy? I was crazy once..."; //working description, change later pls
        }
    }

    public void CozyMode()
    {
        currentDifficulty = Difficulty.cozy;
        AdjustDifficultyButtons();
    }
    public void NormalMode()
    {
        currentDifficulty = Difficulty.normal;
        AdjustDifficultyButtons();
    }
    public void CrazyMode()
    {
        currentDifficulty = Difficulty.crazy;
        AdjustDifficultyButtons();
        Debug.Log("Crazy? I was crazy once... they locked me in a room... a ruber room... a rubber room with rats... the rats made me crazy...Crazy? I was crazy once... they locked me in a room... a ruber room... a rubber room with rats... the rats made me crazy...Crazy? I was crazy once... they locked me in a room... a ruber room... a rubber room with rats... the rats made me crazy...Crazy? I was crazy once... they locked me in a room... a ruber room... a rubber room with rats... the rats made me crazy...Crazy? I was crazy once... they locked me in a room... a ruber room... a rubber room with rats... the rats made me crazy...");
    }


    public void GoToLevelMenu()
    {
        SoundManager.sound.TurnSteamOff();
        StopAllCoroutines();
        SceneManager.LoadScene("LevelSelectMenu");
        LevelSelectUI();
    }





//Collectible Stuff





}
