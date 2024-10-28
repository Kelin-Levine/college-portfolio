using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public class BonusLevelManager : MonoBehaviour
{
    public static UnityEvent AirCuckoosEvent = new();
    public static UnityEvent<GameObject> CuckooMissedSortEvent = new();
    public static UnityEvent BonusDone = new();
    public TextMeshProUGUI gameTextBonus;
    public TextMeshProUGUI mainGameTextBonus;
    public GameObject explosionParticles;
    public GameObject Car;
    public int yards;
    public int bounces; //bounces in the bouncy house level... but ACTUALLY just a timer, ignore bounce and how confusing it is...
    public int buckets; //birds in buckets 
    public int sorted; //birds correctly sorted 
    public int sortingChances; //birds mistakenly missed in sorting
    public int flaps; //flaps in flappy Cuckoo, just placeholder/wrong name, idk how it will work hopefully we think of something better
    public bool carStillGoing;
    public int cuckoosToCollect;
    public bool stillBouncing;




    // Start is called before the first frame update
    void Start()
    {
        BouncyHouse.CuckooCollectedEvent.AddListener(CollectedCuckoo);
        Cuckoo.CuckooBucketted.AddListener(BuckettedCuckoo);
        CollectibleScript.GearFearCollected.AddListener(FearGearCollected);
        CuckooMissedSortEvent.AddListener(SortedCuckooMissed);
        carStillGoing = true;
        gameTextBonus = UIManager.S.gameText;
        mainGameTextBonus = UIManager.S.mainGameText;

        if(SceneManager.GetActiveScene().name == "AirCuckoos")
        {
            AirCuckoosEvent.Invoke();
        }
        else if(SceneManager.GetActiveScene().name == "CuckooCar")
        {
            CuckooCar();
        }
        else if(SceneManager.GetActiveScene().name == "BouncyHouse")
        {
            cuckoosToCollect = 3;
            BouncyHouseStuff();
        }
        else if(SceneManager.GetActiveScene().name == "Buckets")
        {
            buckets = 0;
            BucketsStuff();
        }
        else if(SceneManager.GetActiveScene().name == "Sorts")
        {
            sorted = 0;
            sortingChances = 3;
            SortsStuff();
        }
        else if(SceneManager.GetActiveScene().name == "FlappyCuckoo")
        {
            flaps = 0;
            JonesStuff();
        }


    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "CuckooCar")
        {
            yards = (int)Mathf.Floor(Car.transform.position.x / 10);
            gameTextBonus.text = yards + " yards.";
        }

    }

    public void CuckooCar()
    {
        StartCoroutine(CuckooCarCoroutine());
    }
    private IEnumerator CuckooCarCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        gameTextBonus.enabled = true;
        mainGameTextBonus.enabled = true;
        mainGameTextBonus.text = "Cuckoo Car! Drive as far as you can without crashing your cuckoos!";
        //yield return new WaitForSeconds(2f);
        //mainGameTextBonus.text = "Gain extra yards for pulling cool flips!";
        yield return new WaitForSeconds(2f);
        mainGameTextBonus.enabled = false;
    }
    public void BouncyHouseStuff()
    {
        StartCoroutine(BouncyHouseCoroutine());
    }
    private IEnumerator BouncyHouseCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        gameTextBonus.enabled = true;
        gameTextBonus.text = cuckoosToCollect + " Cuckoos Left";
        mainGameTextBonus.enabled = true;
        mainGameTextBonus.text = "Bouncy House! Find all three cuckoos as fast as you can!";
        //yield return new WaitForSeconds(2f);
        //mainGameTextBonus.text = "Gain extra yards for pulling cool flips!";
        yield return new WaitForSeconds(2f);
        mainGameTextBonus.text = "Ready?";
        yield return new WaitForSeconds(0.5f);
        mainGameTextBonus.text = "3";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "2";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "1";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "GO!";

        yield return new WaitForSeconds(0.5f);
        stillBouncing = true;
        bounces = 0;
        while(stillBouncing) //should probably use Time here... needs more floats and stuff created though...
        {
            bounces += 1;
            mainGameTextBonus.text = bounces + "";
            yield return new WaitForSeconds(1f);
        }
    }

    public void CollectedCuckoo()
    {
        cuckoosToCollect -= 1;
        if(cuckoosToCollect == 1)
        {
            gameTextBonus.text = cuckoosToCollect + "Cuckoo Left";
        }
        else{
            gameTextBonus.text = cuckoosToCollect + "Cuckoos Left";
        }
        if(cuckoosToCollect == 0)
        {
            stillBouncing = false;
            BonusDone.Invoke();
        }
    }

    public void BucketsStuff()
    {
        StartCoroutine(BucketCoroutine());
    }
    private IEnumerator BucketCoroutine()
    {
        GameManagerScript.ChangeGameState(GameState.GetReady);
        yield return new WaitForSeconds(0.1f);
        gameTextBonus.enabled = true;
        gameTextBonus.text = buckets + " buckets";
        mainGameTextBonus.enabled = true;
        mainGameTextBonus.text = "Buckets! Launch as many cuckoos into the pipe as you can before the time runs out!";
        //yield return new WaitForSeconds(2f);
        //mainGameTextBonus.text = "Gain extra yards for pulling cool flips!";
        yield return new WaitForSeconds(2f);
        mainGameTextBonus.text = "Ready?";
        yield return new WaitForSeconds(0.5f);
        mainGameTextBonus.text = "3";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "2";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "1";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "GO!";
        GameManagerScript.ChangeGameState(GameState.Playing);

        yield return new WaitForSeconds(0.5f);
        for(int i = 30; i > 0; i--)
        {
            mainGameTextBonus.text = i + "";
            yield return new WaitForSeconds(1f);
        }
        BonusDone.Invoke();
    }

    public void BuckettedCuckoo()
    {
        buckets += 1;
        gameTextBonus.text = buckets + " buckets";
    }


    public void SortsStuff()
    {
        StartCoroutine(SortsCoroutine());
    }
    private IEnumerator SortsCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        gameTextBonus.enabled = true;
        UpdateSortedCuckooText();
        mainGameTextBonus.enabled = true;
        mainGameTextBonus.text = "Sorting!\nSort cuckoos by paintjob!";
        yield return new WaitForSeconds(2f);
        mainGameTextBonus.text = "Ready?";
        yield return new WaitForSeconds(0.5f);
        mainGameTextBonus.text = "3";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "2";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "1";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "GO!";

        yield return new WaitForSeconds(0.5f);
        mainGameTextBonus.text = "";
    }

    private void UpdateSortedCuckooText()
    {
        gameTextBonus.text = $"Sorted {sorted}\n{sortingChances} chance{(sortingChances == 1 ? "" : "s")} left";
    }

    public void SortedCuckoo()
    {
        sorted += 1;
        UpdateSortedCuckooText();
    }

    public void SortedCuckooMissed(GameObject cuckoo)
    {
        sortingChances -= 1;
        if (sortingChances > 0)
        {
            UpdateSortedCuckooText();
            if (cuckoo.transform.position.y < -4) Instantiate(explosionParticles, cuckoo.transform.position, explosionParticles.transform.rotation);
        }
        else
        {
            GameManagerScript.ChangeGameState(GameState.Oof);
            mainGameTextBonus.text = "";
            Time.timeScale = 0;
            MultiTargetCamera camera = FindObjectOfType<MultiTargetCamera>();
            camera.cameraBounds = new();
            camera.minSize = 5;
            camera.OnCuckooDeath(cuckoo);
            UIManager.S.BlackoutCircleOn(cuckoo);
            StartCoroutine(WaitAndFinish());
        }
    }

    private IEnumerator WaitAndFinish()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        BonusDone.Invoke();
    }

    public void JonesStuff() //for chase level
    {
        StartCoroutine(GearFearCoroutine());
    }

    private IEnumerator GearFearCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        gameTextBonus.enabled = true;
        gameTextBonus.text = flaps + " golden gears";
        mainGameTextBonus.enabled = true;
        mainGameTextBonus.text = "Gear Fear! Collect as many gears as you dare...";
        //yield return new WaitForSeconds(2f);
        //mainGameTextBonus.text = "Gain extra yards for pulling cool flips!";
        yield return new WaitForSeconds(2f);
        mainGameTextBonus.text = "Ready?";
        yield return new WaitForSeconds(0.5f);
        mainGameTextBonus.text = "3";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "2";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "1";
        yield return new WaitForSeconds(1f);
        mainGameTextBonus.text = "GO!";

        yield return new WaitForSeconds(0.5f);
        mainGameTextBonus.enabled = false;
    }

    private void FearGearCollected()
    {
        flaps += 1;
        if(flaps == 1)
        {
            gameTextBonus.text = flaps + " golden gear";
        }
        else{
            gameTextBonus.text = flaps + " golden gears";
        }
    }


}
