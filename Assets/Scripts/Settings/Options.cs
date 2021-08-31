// Controls all of the visual and options settings.
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Options : MonoBehaviour
{
    GameManager gameManagerScript;
    public GameScreen gameScreenScript;

    public GameObject optionsButton;
    public GameObject[] optionsValues;
    public Sprite[] optionsSprites;

    bool fullScreen; int qualityLevel;
    float masterVolume; float musicVolume; float sfxVolume;

    public void Initialise(GameManager GameManagerScript)
    {
        // Resets the options visuals.
        gameManagerScript = GameManagerScript;
        CancelButton();
        // Resets all values and reveals scene through GameScreen.cs.
        gameManagerScript.Master.setVolume(masterVolume / 10f); gameManagerScript.Music.setVolume(musicVolume / 10f); gameManagerScript.SFX.setVolume(sfxVolume / 10f);
        gameScreenScript.StartCoroutine(gameScreenScript.Initialise(gameManagerScript));
        gameObject.SetActive(false);
    }
    public void TriggerOptions() { gameManagerScript.TriggerOptions(); }
    public void FullScreen()
    {
        fullScreen = !fullScreen;
        if (!fullScreen) optionsValues[0].GetComponent<Image>().sprite = optionsSprites[0];
        else optionsValues[0].GetComponent<Image>().sprite = optionsSprites[1];
    }
    public void CheckFullScreen()
    {
        fullScreen = Screen.fullScreen;
        if (!Screen.fullScreen) optionsValues[0].GetComponent<Image>().sprite = optionsSprites[0];
        else optionsValues[0].GetComponent<Image>().sprite = optionsSprites[1];
    }
    public void QualityLevel(int value)
    {
        qualityLevel += value;
        if (qualityLevel <= 0) { qualityLevel = 0; optionsValues[1].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[3]; }
        else optionsValues[1].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[2];
        if (qualityLevel >= 5) { qualityLevel = 5; optionsValues[1].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[3]; }
        else optionsValues[1].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[2];
        optionsValues[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = qualityLevel.ToString();
    }
    public void MasterLevel(int value)
    {
        masterVolume += value;
        if (masterVolume <= 0) { masterVolume = 0; optionsValues[2].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[3]; }
        else optionsValues[2].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[2];
        if (masterVolume >= 10) { masterVolume = 10; optionsValues[2].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[3]; }
        else optionsValues[2].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[2];
        optionsValues[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = masterVolume.ToString();
    }
    public void MusicLevel(int value)
    {
        musicVolume += value;
        if (musicVolume <= 0) { musicVolume = 0; optionsValues[3].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[3]; }
        else optionsValues[3].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[2];
        if (musicVolume >= 10) { musicVolume = 10; optionsValues[3].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[3]; }
        else optionsValues[3].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[2];
        optionsValues[3].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = musicVolume.ToString();
    }
    public void SFXLevel(int value)
    {
        sfxVolume += value;
        if (sfxVolume <= 0) { sfxVolume = 0; optionsValues[4].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[3]; }
        else optionsValues[4].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[2];
        if (sfxVolume >= 10) { sfxVolume = 10; optionsValues[4].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[3]; }
        else optionsValues[4].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[2];
        optionsValues[4].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = sfxVolume.ToString();
    }
    public void ConfirmButton()
    {
        Screen.fullScreen = fullScreen;
        QualitySettings.SetQualityLevel(qualityLevel, true);
        gameManagerScript.saveData.masterVolume = masterVolume; gameManagerScript.saveData.musicVolume = musicVolume; gameManagerScript.saveData.sfxVolume = sfxVolume;
        gameManagerScript.Master.setVolume(masterVolume / 10f); gameManagerScript.Music.setVolume(musicVolume / 10f); gameManagerScript.SFX.setVolume(sfxVolume / 10f);
    }
    public void CancelButton()
    {
        fullScreen = Screen.fullScreen;
        qualityLevel = QualitySettings.GetQualityLevel();
        masterVolume = gameManagerScript.saveData.masterVolume; musicVolume = gameManagerScript.saveData.musicVolume; sfxVolume = gameManagerScript.saveData.sfxVolume;
        ResetValues();
    }
    void ResetValues()
    {
        // Full Screen
        if (!Screen.fullScreen) optionsValues[0].GetComponent<Image>().sprite = optionsSprites[0];
        else optionsValues[0].GetComponent<Image>().sprite = optionsSprites[1];
        // Quality
        optionsValues[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = QualitySettings.GetQualityLevel().ToString();
        if (QualitySettings.GetQualityLevel() == 0) optionsValues[1].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[3];
        else optionsValues[1].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[2];
        if (QualitySettings.GetQualityLevel() == 5) optionsValues[1].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[3];
        else optionsValues[1].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[2];
        // Master
        optionsValues[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = gameManagerScript.saveData.masterVolume.ToString();
        if (gameManagerScript.saveData.masterVolume == 0) optionsValues[2].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[3];
        else optionsValues[2].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[2];
        if (gameManagerScript.saveData.masterVolume == 10) optionsValues[2].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[3];
        else optionsValues[2].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[2];
        // Music
        optionsValues[3].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = gameManagerScript.saveData.musicVolume.ToString();
        if (gameManagerScript.saveData.musicVolume == 0) optionsValues[3].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[3];
        else optionsValues[3].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[2];
        if (gameManagerScript.saveData.musicVolume == 10) optionsValues[3].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[3];
        else optionsValues[3].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[2];
        // SFX
        optionsValues[4].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = gameManagerScript.saveData.sfxVolume.ToString();
        if (gameManagerScript.saveData.sfxVolume == 0) optionsValues[4].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[3];
        else optionsValues[4].transform.GetChild(0).GetComponent<Image>().sprite = optionsSprites[2];
        if (gameManagerScript.saveData.sfxVolume == 10) optionsValues[4].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[3];
        else optionsValues[4].transform.GetChild(2).GetComponent<Image>().sprite = optionsSprites[2];
    }
    public void Quit() { gameManagerScript.StartCoroutine(gameManagerScript.Quit()); }
    public void Reset() { gameManagerScript.StartCoroutine(gameManagerScript.Reset()); }
}