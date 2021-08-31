// Contains the writing/reseting system commands.
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveMenu : MonoBehaviour
{
    public GameManager gameManagerScript;
    public TitleScreen titleScreenScript;
    public PCG PCGScript; public GameObject autosavingObject; public GameObject Controls;
    [DllImport("__Internal")] private static extern void SyncFiles();
    void Start() { gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>(); gameManagerScript.saveMenuScript = this; if (autosavingObject != null) StartCoroutine(TriggerAutosave()); }
    public void Reset()
    {
        gameManagerScript.saveData.Name = "";
        gameManagerScript.saveData.Portrait = -1;
        gameManagerScript.saveData.Castle = Random.Range(0, 3);
        gameManagerScript.saveData.FOW = new List<int>(); gameManagerScript.saveData.FOWX = new List<int>(); gameManagerScript.saveData.FOWY = new List<int>();
        gameManagerScript.saveData.Buildings = new List<int>();
        gameManagerScript.saveData.buildingsTitles = new List<string>(); gameManagerScript.saveData.buildingsSubTitles = new List<string>();
        // Main Castles.
        gameManagerScript.saveData.Buildings.Add(0); gameManagerScript.saveData.Buildings.Add(0); gameManagerScript.saveData.Buildings.Add(3);
        gameManagerScript.saveData.buildingsTitles.Add(CreateName("Castle "));  gameManagerScript.saveData.buildingsTitles.Add(CreateName("Castle ")); gameManagerScript.saveData.buildingsTitles.Add(CreateName("Castle "));
        gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory");
        // Sub Villages.
        gameManagerScript.saveData.Buildings.Add(6); gameManagerScript.saveData.Buildings.Add(7); gameManagerScript.saveData.Buildings.Add(8);
        gameManagerScript.saveData.buildingsTitles.Add(CreateName("Outpost ")); gameManagerScript.saveData.buildingsTitles.Add(CreateName("Castle ")); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Keep");
        gameManagerScript.saveData.buildingsSubTitles.Add("Trading Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Trading Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory");
        // Mage Towers
        gameManagerScript.saveData.Buildings.Add(9); gameManagerScript.saveData.Buildings.Add(9); gameManagerScript.saveData.Buildings.Add(9);
        gameManagerScript.saveData.buildingsTitles.Add(CreateName("The ")); gameManagerScript.saveData.buildingsTitles.Add(CreateName("The ")); gameManagerScript.saveData.buildingsTitles.Add(CreateName("The "));
        gameManagerScript.saveData.buildingsSubTitles.Add("Mage Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Mage Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Mage Territory");
        // Villages
        gameManagerScript.saveData.Buildings.Add(10); gameManagerScript.saveData.Buildings.Add(10); gameManagerScript.saveData.Buildings.Add(10);
        gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Village"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Motte"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Hill");
        gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory");
        // Towers
        gameManagerScript.saveData.Buildings.Add(11); gameManagerScript.saveData.Buildings.Add(11); gameManagerScript.saveData.Buildings.Add(11); gameManagerScript.saveData.Buildings.Add(11); gameManagerScript.saveData.Buildings.Add(12); gameManagerScript.saveData.Buildings.Add(12);
        gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Tower"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Tower"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Tower"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Tower"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Tower"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Tower");
        gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory");
        // Mines
        gameManagerScript.saveData.Buildings.Add(13); gameManagerScript.saveData.Buildings.Add(13); gameManagerScript.saveData.Buildings.Add(13);
        gameManagerScript.saveData.buildingsTitles.Add(CreateName("Mine ")); gameManagerScript.saveData.buildingsTitles.Add(CreateName("Mine ")); gameManagerScript.saveData.buildingsTitles.Add(CreateName("Mine "));
        gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory");
        // Farms & Windmills
        gameManagerScript.saveData.Buildings.Add(14); gameManagerScript.saveData.Buildings.Add(14); gameManagerScript.saveData.Buildings.Add(15);
        gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Farm"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Farm"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Mill");
        gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory");
        gameManagerScript.saveData.Buildings.Add(14); gameManagerScript.saveData.Buildings.Add(14); gameManagerScript.saveData.Buildings.Add(15);
        gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Farm"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Farm"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Mill");
        gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory");
        gameManagerScript.saveData.Buildings.Add(14); gameManagerScript.saveData.Buildings.Add(14); gameManagerScript.saveData.Buildings.Add(15);
        gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Farm"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Farm"); gameManagerScript.saveData.buildingsTitles.Add(CreateName("") + " Mill");
        gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory"); gameManagerScript.saveData.buildingsSubTitles.Add("Neutral Territory");
        // Resources
        gameManagerScript.saveData.Resources = new List<int>(); gameManagerScript.saveData.ResourcesX = new List<int>(); gameManagerScript.saveData.ResourcesY = new List<int>();
        // Units
        gameManagerScript.saveData.Homes = new List<int>(); gameManagerScript.saveData.Names = new List<string>(); gameManagerScript.saveData.Titles = new List<string>(); gameManagerScript.saveData.Portraits = new List<int>(); gameManagerScript.saveData.Allies = new List<bool>();
        gameManagerScript.saveData.UnitsX = new List<int>(); gameManagerScript.saveData.UnitsY = new List<int>(); gameManagerScript.saveData.Schedules = new List<string>();
        // Story
        gameManagerScript.saveData.usedEvents = new List<int>(); gameManagerScript.saveData.currentEvents = new List<int>();
        gameManagerScript.saveData.Events = new List<int>(); gameManagerScript.saveData.EventsX = new List<int>(); gameManagerScript.saveData.EventsY = new List<int>(); gameManagerScript.saveData.eventsPeople = new List<string>();
        // Settings
        gameManagerScript.saveData.masterVolume = 7; gameManagerScript.saveData.musicVolume = 3; gameManagerScript.saveData.sfxVolume = 5;
        Save();
    }
    string CreateName(string Title)
    {
        // Creates the name.
        string name = ""; int nameLength = Random.Range(2, 5);
        string[] firstComponent = new string[] { "B", "H", "V", "S", "T", "W", "O", "G", "A", "C", "R", "L", "M", "E", "H", "P" };
        string[] secondComponent = new string[] { "lac", "ole", "ale", "iew", "had", "arn", "ell", "old", "uth", "rch", "ran", "in" };
        string[] thirdComponent = new string[] { "en", "er", "on", "ow", "s", "th", "g", "ra", "mo", "em", "st", "als", "an", "k", "pth" };
        string[] fourthComponent = new string[] { "ess", "ine", "ere", "ead", "len", "am", "uth", "glen", "ell", "thy", "irk", "by", "ley" };
        for (int i = 0; i < nameLength; i++)
        {
            if (i == 0) name += firstComponent[Random.Range(0, firstComponent.Length)];
            if (i == 1) name += secondComponent[Random.Range(0, secondComponent.Length)];
            if (i == 2) name += thirdComponent[Random.Range(0, thirdComponent.Length)];
            if (i == 3) name += fourthComponent[Random.Range(0, fourthComponent.Length)];
        }
        // Creates a second word.
        nameLength = Random.Range(-5, 4);
        if (nameLength > 0) name += " "; if (nameLength == 1) nameLength = 2;
        for (int i = 0; i < nameLength; i++)
        {
            if (i == 0) name += firstComponent[Random.Range(0, firstComponent.Length)];
            if (i == 1) name += secondComponent[Random.Range(0, secondComponent.Length)];
            if (i == 2) name += fourthComponent[Random.Range(0, fourthComponent.Length)];
        }
        return Title + name;
    }
    //Saves the Save Files.
    public void Save()
    {
        BinaryFormatter binaryformatter = new BinaryFormatter(); FileStream file;
        file = File.Create(Application.persistentDataPath + "/Save.dat");
        binaryformatter.Serialize(file, gameManagerScript.saveData);
        file.Close();
        if (Application.platform == RuntimePlatform.WebGLPlayer) { SyncFiles(); }
        if (titleScreenScript != null) titleScreenScript.firstLoading = true;
        if (autosavingObject != null && !gameManagerScript.Paused) StartCoroutine(Autosaving());
    }
    IEnumerator TriggerAutosave()
    {
        yield return new WaitUntil(() => PCGScript.AIManagerScript.Initialised && !Controls.activeSelf);
        while (true)
        {
            float t = 0; while (t < 30f)
            {
                t += Time.deltaTime;
                if (gameManagerScript.Dialogue && t > 15f) t -= 7.5f;
                yield return new WaitUntil(() => !gameManagerScript.Paused && !gameManagerScript.Dialogue);
                yield return null;
            }
            Save();
            yield return null;
        }
    }
    IEnumerator Autosaving()
    {
        LeanTween.alphaCanvas(autosavingObject.GetComponent<CanvasGroup>(), 1, 1).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(2.5f);
        LeanTween.alphaCanvas(autosavingObject.GetComponent<CanvasGroup>(), 0, 1).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1f);
    }
    //Clears the Save Files.
    public void ClearSave()
    {
        if (File.Exists(Application.persistentDataPath + "/Save.dat")) File.Delete(Application.persistentDataPath + "/Save.dat");
        Reset();
    }
}