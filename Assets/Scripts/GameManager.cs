// Controls and holds all of the projects variables and save data.
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool Paused = false;
    public bool Dialogue = false;
    public Options optionsScript;
    public SaveData saveData;
    public SaveMenu saveMenuScript;
    // Detects each time a new scene is triggered.
    public FMOD.Studio.Bus Master; public FMOD.Studio.Bus Music; public FMOD.Studio.Bus SFX;
    void OnApplicationFocus(bool hasFocus) { Master.setMute(!hasFocus); Music.setMute(!hasFocus); SFX.setMute(!hasFocus); }
    void OnEnable() { SceneManager.sceneLoaded += OnLevelFinishedLoading; }
    void OnDisable() { SceneManager.sceneLoaded -= OnLevelFinishedLoading; }
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Paused = false; Dialogue = false;
        if (scene.name != "gameScene") return;
        optionsScript = GameObject.Find("Options").GetComponent<Options>();
        optionsScript.Initialise(this);
        //FMOD.Studio.EventInstance OST = FMODUnity.RuntimeManager.CreateInstance("event:/Music/GameOST");
        //OST.start();
    }
    public void PlayOST(string musicClip)
    {
        FMOD.Studio.EventInstance OST = FMODUnity.RuntimeManager.CreateInstance("event:/Music/" + musicClip);
        OST.start();
    }
    // Audio commands to play SFX sounds and mute them.
    public void PlaySound(string soundClip)
    {
        FMOD.Studio.EventInstance SE = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/" + soundClip);
        SE.start();
    }
    // Mutes all Audio.
    public void MuteSound()
    {
        Master.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); Music.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); SFX.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    // At the very start of the game, initiates the sound banks after they've loaded.
    IEnumerator Start()
    {
        DontDestroyOnLoad(this.gameObject);
        yield return new WaitUntil(() => 
            FMODUnity.RuntimeManager.HasBankLoaded("Master Bank") && FMODUnity.RuntimeManager.HasBankLoaded("Master Bank.strings") && FMODUnity.RuntimeManager.HasBankLoaded("Music") && FMODUnity.RuntimeManager.HasBankLoaded("SFX"));
        yield return new WaitUntil(() => GameObject.Find("Canvas").GetComponent<TitleScreen>().firstLoading);
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master"); Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music"); SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
        GameObject.Find("Canvas").GetComponent<TitleScreen>().secondLoading = true;
    }
    bool tweenRunning = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Screen.fullScreen && optionsScript != null) TriggerOptions();
    }
    public void TriggerOptions()
    {
        if (!tweenRunning)
        {
            if (optionsScript.transform.GetChild(1).gameObject.activeSelf)
            {
                tweenRunning = true;
                StartCoroutine(TriggerOptions(!optionsScript.gameObject.activeSelf));
            }
            else if (optionsScript.transform.GetChild(2).gameObject.activeSelf)
            {
                optionsScript.CancelButton();
                optionsScript.transform.GetChild(2).gameObject.SetActive(false);
                optionsScript.transform.GetChild(1).gameObject.SetActive(true);
            }
            else if (optionsScript.transform.GetChild(3).gameObject.activeSelf)
            {
                optionsScript.transform.GetChild(3).gameObject.SetActive(false);
                optionsScript.transform.GetChild(1).gameObject.SetActive(true);
            }
            else if (optionsScript.transform.GetChild(4).gameObject.activeSelf)
            {
                optionsScript.transform.GetChild(4).gameObject.SetActive(false);
                optionsScript.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }
    // Conditions to open and close the options menu.
    IEnumerator TriggerOptions(bool Pause)
    {
        if (!Paused && !optionsScript.optionsButton.activeSelf) { tweenRunning = false; yield break; }
        Paused = Pause;
        if (Pause)
        {
            optionsScript.gameObject.SetActive(true);
            LeanTween.alphaCanvas(optionsScript.GetComponent<CanvasGroup>(), 1, 0.25f).setEase(LeanTweenType.easeInOutQuad);
            optionsScript.optionsButton.GetComponent<CanvasGroup>().alpha = 0;
            optionsScript.optionsButton.SetActive(false);
        }
        else
        {
            optionsScript.optionsButton.SetActive(true);
            LeanTween.alphaCanvas(optionsScript.GetComponent<CanvasGroup>(), 0, 0.25f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.alphaCanvas(optionsScript.optionsButton.GetComponent<CanvasGroup>(), 1, 0.25f).setEase(LeanTweenType.easeInOutQuad);
        }
        yield return new WaitForSeconds(0.25f);
        if (!Pause) optionsScript.gameObject.SetActive(false);
        tweenRunning = false;
    }
    public IEnumerator Quit()
    {
        optionsScript.gameScreenScript.Transition.SetActive(true);
        MuteSound();
        LeanTween.alphaCanvas(optionsScript.gameScreenScript.Transition.GetComponent<CanvasGroup>(), 1, 1f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public IEnumerator Reset()
    {
        optionsScript.gameScreenScript.Transition.SetActive(true);
        MuteSound();
        LeanTween.alphaCanvas(optionsScript.gameScreenScript.Transition.GetComponent<CanvasGroup>(), 1, 1f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1f);
        saveMenuScript.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

// Holds all of the SaveData.
[System.Serializable]
public class SaveData
{
    public string Name; public int Portrait; public int Castle;
    public List<int> FOW; public List<int> FOWX; public List<int> FOWY;
    public List<int> Buildings;
    public List<string> buildingsTitles; public List<string> buildingsSubTitles;
    public List<int> Resources; public List<int> ResourcesX; public List<int> ResourcesY;
    public List<int> Homes; public List<string> Names; public List<string> Titles; public List<int> Portraits; public List<bool> Allies; public List<int> UnitsX; public List<int> UnitsY; public List<string> Schedules;
    public List<int> usedEvents; public List<int> currentEvents; public List<int> Events; public List<int> EventsX; public List<int> EventsY; public List<string> eventsPeople;
    public float masterVolume = -1; public float musicVolume = -1; public float sfxVolume = -1;
}