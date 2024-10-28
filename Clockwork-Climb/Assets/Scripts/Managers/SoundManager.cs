using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioClip cuckooWalk, cannonSound, valveSound, startJingle, trampolineBounce, trampolinePlacement, cuckooDeathSound, memeDeathSound, soundForUIOne, soundForUITwo, winJingle, goldenGearSound, windUpKey, intenseIntro, clubIntro, anthemIntro;
    private AudioSource audiox;
    public AudioSource intense, club, anthem;
    public static SoundManager sound; //singleton for sound
    public AudioMixer backgroundMixer, sfxMixer;
    public string mixer;

    private void Awake()
    {
        if(sound)
        {
            Destroy(this.gameObject);
        }
        else{
            sound = this;
        }

        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        audiox = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

//sfx
    public void MakeStartJingleSound()
    {
        audiox.PlayOneShot(startJingle);
    }
    public void MakeTrampolineBounceSound()
    {
        audiox.PlayOneShot(trampolineBounce);
    }
    public void MakeTrampolinePlacementSound()
    {
        audiox.PlayOneShot(trampolinePlacement);
    }
    public void MakeSoundForUIOne()
    {
        audiox.PlayOneShot(soundForUIOne);
    }
    public void MakeSoundForUITwo()
    {
        audiox.PlayOneShot(soundForUITwo);
    }
    public void MakeWinJingle()
    {
        audiox.PlayOneShot(winJingle);
    }
    public void MakeCuckooDeathSound()
    {
        if(Random.value > 0.02f)
        {
            audiox.PlayOneShot(cuckooDeathSound);
        }
        else
        {
            audiox.PlayOneShot(memeDeathSound);   
        }
    }
    public void MakeGoldenGearSound()
    {
        audiox.PlayOneShot(goldenGearSound);
    }
    public void MakeWindUpSound()
    {
        audiox.PlayOneShot(windUpKey);
    }
    public void MakeCannonSound()
    {
        audiox.PlayOneShot(cannonSound);
    }
    public void MakeValveSound()
    {
        audiox.PlayOneShot(valveSound);
    }
    public void TurnSteamOn()
    {
        sfxMixer.FindSnapshot("SteamOn").TransitionTo(0.5f);
    }
    public void TurnSteamOff()
    {
        sfxMixer.FindSnapshot("SteamOff").TransitionTo(0.5f);
    }


//music
    public void PlayMenuMusic()
    {
        backgroundMixer.FindSnapshot("MenuOn").TransitionTo(0.3f);
        mixer = "MenuOn";
    }
    public void PlayGameplayMusic()
    {
        StartCoroutine(ChooseMusic());
    }

    private IEnumerator ChooseMusic()
    {
        string lastScene = SceneManager.GetActiveScene().name;
        yield return new WaitForSecondsRealtime(0.1f);
        string currentScene = SceneManager.GetActiveScene().name;
        //clock
        if(currentScene == "Cutscene" || currentScene == "1.6" || currentScene == "1.1" || currentScene == "1.2" || currentScene == "1.3" || currentScene == "1.4" || currentScene == "1.5")
        {
            PlayClockMusic();
        }
        //inside house
        else if(currentScene == "Cutscene 2" || currentScene == "2.1" || currentScene == "2.2" || currentScene == "2.3" || currentScene == "2.4" || currentScene == "2.5" || currentScene == "2.6")
        {
            PlayHomeMusic();
        }
        //front yard
        else if(currentScene == "cutscene 3" || currentScene == "3.1" || currentScene == "3.2" || currentScene == "3.3" || currentScene == "3.4")
        {
            PlayYardMusic();
        }
        //sewers
        else if(currentScene == "cutscene 4" || currentScene == "3.5" || currentScene == "3.6")
        {
            PlaySewersMusic();
        }
        //muffled club
        else if(currentScene == "Cutscene 5" || currentScene == "4.1")
        {
            PlayMuffledClubMusic();
        }
        //club
        else if(currentScene == "4.2" || currentScene == "4.3" || currentScene == "4.4" || currentScene == "4.5" || currentScene == "4.6")
        {
            PlayClubMusic();
        }
        //high rise
        else if(currentScene == "cutscene 6" || currentScene == "5.1" || currentScene == "5.2" || currentScene == "5.3" || currentScene == "5.4" || currentScene == "5.5" || currentScene == "5.6")
        {
            PlayHighRiseMusic();
        }
        //clock tower
        else if(currentScene == "Cutscene 7" || currentScene == "6.1" || currentScene == "6.2" || currentScene == "6.3" || currentScene == "6.4" || currentScene == "6.5" || currentScene == "6.6")
        {
            PlayIntenseMusic();
        }
        //other... bonus?
        else{
            PlayAnthemMusic();
        }
    }


    public void PlayClockMusic()
    {
        backgroundMixer.FindSnapshot("Clock").TransitionTo(0.3f);
        mixer = "Clock";
    }
    public void PlayHomeMusic()
    {
        backgroundMixer.FindSnapshot("Home").TransitionTo(0.3f);
        mixer = "Home";
    }
    public void PlayYardMusic()
    {
        backgroundMixer.FindSnapshot("Yard").TransitionTo(0.3f);
        mixer = "Yard";
    }
    public void PlaySewersMusic()
    {
        backgroundMixer.FindSnapshot("Sewers").TransitionTo(0.3f);
        mixer = "Sewers";
    }
    public void PlayMuffledClubMusic()
    {
        backgroundMixer.FindSnapshot("Muffled").TransitionTo(0.3f);
        mixer = "Muffled";
    }
    public void PlayHighRiseMusic()
    {
        backgroundMixer.FindSnapshot("HighRise").TransitionTo(0.3f);
        mixer = "HighRise";
    }


    public void PlayIntenseMusic()
    {
        if(mixer != "Intense")
        {
            //StopCoroutine(PlayIntenseMusicWithIntro());
            StopAllCoroutines();
            StartCoroutine(PlayIntenseMusicWithIntro());
        }
    }
    private IEnumerator PlayIntenseMusicWithIntro()
    {
        intense.Stop();
        yield return new WaitForSecondsRealtime(0.1f);
        intense.PlayOneShot(intenseIntro);
        backgroundMixer.FindSnapshot("IntenseOn").TransitionTo(0.1f);
        mixer = "Intense";
        yield return new WaitForSecondsRealtime(intenseIntro.length); //NEEDS TO BE FIXED: without the .3f it doesn't play the first tiny bit and clips
        intense.Play(0);
    }
    public void PlayClubMusic()
    {
        if(mixer != "Club")
        {
            //StopCoroutine(PlayClubMusicWithIntro());
            StopAllCoroutines();
            StartCoroutine(PlayClubMusicWithIntro());
        }
    }
    private IEnumerator PlayClubMusicWithIntro()
    {
        club.Stop();
        yield return new WaitForSecondsRealtime(0.1f);
        club.PlayOneShot(clubIntro);
        backgroundMixer.FindSnapshot("ClubOn").TransitionTo(0.1f);
        mixer = "Club";
        yield return new WaitForSecondsRealtime(clubIntro.length);
        club.Play(0);
    }
    public void PlayAnthemMusic()
    {
        if(mixer != "Anthem")
        {
            //StopCoroutine(PlayAnthemMusicWithIntro());
            StopAllCoroutines();
            StartCoroutine(PlayAnthemMusicWithIntro());
        }
    }
    private IEnumerator PlayAnthemMusicWithIntro()
    {
        anthem.Stop();
        yield return new WaitForSecondsRealtime(0.1f);
        anthem.PlayOneShot(anthemIntro);
        backgroundMixer.FindSnapshot("AnthemOn").TransitionTo(0.1f);
        mixer = "Anthem";
        yield return new WaitForSecondsRealtime(anthemIntro.length);
        anthem.Play(0);
    }



//settings

    public void SetSFXVolume(float sfxVolume)
    {
        sfxMixer.SetFloat("sfxVolume", Mathf.Log10(sfxVolume) * 20);
    }
    public void SetMusicVolume(float musicVolume)
    {
        backgroundMixer.SetFloat("backgroundVolume", Mathf.Log10(musicVolume) * 20);
    }


}
