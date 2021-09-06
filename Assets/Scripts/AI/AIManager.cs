// Controls the script for creating new Units.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AIManager : MonoBehaviour
{
    public GridManager Grid;
    public StoryManager storyManagerScript;
    public GameObject Actions;
    public GameObject actionsTextPrefab;
    public Transform UnitPrefab;
    public GameObject eventPointer;
    public PCG PCGScript;
    public Transform EventsCanvas;
    public Transform Buildings;
    public Transform Units;

    public List<Sprite> playerIcons;
    List<int> availablePlayerIcons;
    public List<Sprite> unitIcons;
    List<int> availablePriestIcons = new List<int>();
    List<int> availableMageIcons = new List<int>();
    List<int> availableKnightIcons = new List<int>();
    List<int> availableOtherIcons = new List<int>();
    public List<Sprite> unitInsideIcons;

    [HideInInspector] public bool Initialised;

    private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();
    public void InitialiseValues()
    {        
        // Creates the available Icons for other kings/units.
        availablePlayerIcons = new List<int> { 1, 2, 3, 4, 5 };
        if (PCGScript.gameManagerScript.saveData.Portrait < 6)
        {
            availablePlayerIcons.Remove(PCGScript.gameManagerScript.saveData.Portrait + 5);
            availablePlayerIcons.Remove(PCGScript.gameManagerScript.saveData.Portrait);
        }
        else
        {
            availablePlayerIcons.Remove(PCGScript.gameManagerScript.saveData.Portrait);
            availablePlayerIcons.Remove(PCGScript.gameManagerScript.saveData.Portrait - 5);
        }
        availablePriestIcons = new List<int> { 1, 2, 3, 4 };
        availableMageIcons = new List<int> { 5, 6, 7, 8, 9, 10 };
        availableKnightIcons = new List<int> { 11, 12, 13, 14, 15, 16 };
        availableOtherIcons = new List<int> { 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };

        StartCoroutine(CoroutineCoordinator());
    }
    // Gets called after all other checks to start the game (universal).
    public void Initialise()
    {
        Initialised = true;
        triggerLoop = 5;
        InvokeRepeating("TriggerEvents", 2.5f, 10f);
    }
    int triggerLoop; void TriggerEvents()
    {
        if (PCGScript.gameManagerScript.Paused || PCGScript.gameManagerScript.Dialogue || PCGScript.gameManagerScript.saveData.currentEvents.Count > 3) { triggerLoop = 0; return; }
        else if (triggerLoop < Random.Range(3, 5)) triggerLoop++;
        else
        {
            Debug.Log("Trigger Event");
            if (PCGScript.gameManagerScript.saveData.Events.Count != 0 && !PCGScript.gameManagerScript.saveData.currentEvents.Contains(0) && !PCGScript.gameManagerScript.saveData.currentEvents.Contains(1) && !PCGScript.gameManagerScript.saveData.currentEvents.Contains(2) && !PCGScript.gameManagerScript.saveData.currentEvents.Contains(4))
            {
                triggerLoop = 0;
                CreateEvent(true, PCGScript.gameManagerScript.saveData.Events[Random.Range(0, PCGScript.gameManagerScript.saveData.Events.Count)]);
            }
            else triggerLoop = 2;
        }
    }
    // Only happens when a New Game is created.
    public void NewGame()
    {
        // Creates a Hand, Captain and Bishop.
        CreateUnit(true, 0, PCGScript.gameManagerScript.saveData.Castle, Buildings.GetChild(PCGScript.gameManagerScript.saveData.Castle).position, true);
        CreateUnit(true, 3, PCGScript.gameManagerScript.saveData.Castle, Buildings.GetChild(PCGScript.gameManagerScript.saveData.Castle).position, true);
        CreateUnit(true, 4, PCGScript.gameManagerScript.saveData.Castle, Buildings.GetChild(PCGScript.gameManagerScript.saveData.Castle).position, true);
    }
    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (coroutineQueue.Count > 0) yield return StartCoroutine(coroutineQueue.Dequeue());
            yield return null;
        }
    }
    public void CreateEvent(bool newEvent, int whichEvent) { coroutineQueue.Enqueue(storyManagerScript.createEvent(newEvent, whichEvent)); }
    public void CreateUnit(bool newUnit, int whatUnit, int home, Vector3 startPos, bool isAlly) { coroutineQueue.Enqueue(createUnit(newUnit, whatUnit, home, startPos, isAlly)); }
    IEnumerator createUnit(bool newUnit, int whatUnit, int home, Vector3 startPos, bool isAlly)
    {
        Debug.Log("Creating Unit");
        //yield return new WaitUntil(() => !PCGScript.Grid.CheckWall(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.z)));
        Transform Unit = Instantiate(UnitPrefab) as Transform;
        Unit.transform.position = startPos;
        // if inside castle then deactivate model and do icon inside maybe?
        for (int i = 0; i < Buildings.childCount; i++)
        {
            if (startPos == Buildings.GetChild(i).transform.position)
            {
                Unit.GetComponent<unitManager>().Model.GetComponent<pathfindingManager>().isIndoors = true;
            }
        }
        if (!Unit.GetComponent<unitManager>().Model.GetComponent<pathfindingManager>().isIndoors)
            yield return new WaitUntil(() => !PCGScript.Grid.CheckWall(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.z)));
        Unit.parent = Units;
        Unit.GetComponent<unitManager>().Unit = Units.childCount - 1;
        // If its a new unit set its data.
        if (newUnit)
        {
            Unit.GetComponent<unitManager>().Home = home;
            PCGScript.gameManagerScript.saveData.Homes.Add(home);
            string Name = "";
            if (whatUnit == 0)
            {
                // Updates Names
                Name = "Constable " + CreateName();
                Unit.GetComponent<unitManager>().Name = Name;
                PCGScript.gameManagerScript.saveData.Names.Add(Name);
                Unit.GetComponent<unitManager>().Title = "The Thrones Hand";
                PCGScript.gameManagerScript.saveData.Titles.Add("The Thrones Hand");
                // Updates Portrait
                int random = Random.Range(0, availableOtherIcons.Count);
                Unit.GetComponent<unitManager>().Portrait = availableOtherIcons[random];
                PCGScript.gameManagerScript.saveData.Portraits.Add(availableOtherIcons[random]);
                availableOtherIcons.Remove(availableOtherIcons[random]);
            }
            else if (whatUnit == 1)
            {
                Name = "Treasurer " + CreateName();
                Unit.GetComponent<unitManager>().Name = Name;
                PCGScript.gameManagerScript.saveData.Names.Add(Name);
                Unit.GetComponent<unitManager>().Title = "The Master of Coin";
                PCGScript.gameManagerScript.saveData.Titles.Add("The Master of Coin");
                // Updates Portrait
                int random = Random.Range(0, availableOtherIcons.Count);
                Unit.GetComponent<unitManager>().Portrait = availableOtherIcons[random];
                PCGScript.gameManagerScript.saveData.Portraits.Add(availableOtherIcons[random]);
                availableOtherIcons.Remove(availableOtherIcons[random]);
            }
            else if (whatUnit == 2)
            {
                Name = "Ser " + CreateName();
                Unit.GetComponent<unitManager>().Name = Name;
                PCGScript.gameManagerScript.saveData.Names.Add(Name);
                Unit.GetComponent<unitManager>().Title = "The Drill Master";
                PCGScript.gameManagerScript.saveData.Titles.Add("The Drill Master");
                // Updates Portrait
                int random = Random.Range(0, availableKnightIcons.Count);
                Unit.GetComponent<unitManager>().Portrait = availableKnightIcons[random];
                PCGScript.gameManagerScript.saveData.Portraits.Add(availableKnightIcons[random]);
                availableKnightIcons.Remove(availableKnightIcons[random]);
            }
            else if (whatUnit == 3)
            {
                Name = "Ser " + CreateName();
                Unit.GetComponent<unitManager>().Name = Name;
                PCGScript.gameManagerScript.saveData.Names.Add(Name);
                Unit.GetComponent<unitManager>().Title = "The Thrones Guard";
                PCGScript.gameManagerScript.saveData.Titles.Add("The Thrones Guard");
                // Updates Portrait
                int random = Random.Range(0, availableKnightIcons.Count);
                Unit.GetComponent<unitManager>().Portrait = availableKnightIcons[random];
                PCGScript.gameManagerScript.saveData.Portraits.Add(availableKnightIcons[random]);
                availableKnightIcons.Remove(availableKnightIcons[random]);
                Unit.GetComponent<unitManager>().Schedule = "Patrol Home";

                if (isAlly)
                {
                    for (int i = 0; i < PCGScript.gameManagerScript.saveData.Allies.Count; i++)
                    {
                        if (PCGScript.gameManagerScript.saveData.Allies[i] && PCGScript.gameManagerScript.saveData.Titles[i] == "The Thrones Hand")
                        {
                            PCGScript.gameManagerScript.saveData.Events.Add(1);
                            PCGScript.gameManagerScript.saveData.eventsPeople.Add("Event:1. " + i + "/" + (Units.childCount - 1));
                            PCGScript.gameManagerScript.saveData.EventsX.Add(Mathf.RoundToInt(Buildings.GetChild(PCGScript.gameManagerScript.saveData.Castle).position.x));
                            PCGScript.gameManagerScript.saveData.EventsY.Add(Mathf.RoundToInt(Buildings.GetChild(PCGScript.gameManagerScript.saveData.Castle).position.z));
                        }
                    }
                }
            }
            else if (whatUnit == 4)
            {
                Name = "Reverend " + CreateName();
                Unit.GetComponent<unitManager>().Name = Name;
                PCGScript.gameManagerScript.saveData.Names.Add(Name);
                Unit.GetComponent<unitManager>().Title = "The Archbishop";
                PCGScript.gameManagerScript.saveData.Titles.Add("The Archbishop");
                // Updates Portrait
                int random = Random.Range(0, availablePriestIcons.Count);
                Unit.GetComponent<unitManager>().Portrait = availablePriestIcons[random];
                PCGScript.gameManagerScript.saveData.Portraits.Add(availablePriestIcons[random]);
                availablePriestIcons.Remove(availablePriestIcons[random]);
                // If there is a hand and Archbishop that belong to the King
                if (isAlly)
                {
                    for (int i = 0; i < PCGScript.gameManagerScript.saveData.Allies.Count; i++)
                    {
                        if (PCGScript.gameManagerScript.saveData.Allies[i] && PCGScript.gameManagerScript.saveData.Titles[i] == "The Thrones Hand")
                        {
                            PCGScript.gameManagerScript.saveData.Events.Add(0);
                            PCGScript.gameManagerScript.saveData.eventsPeople.Add("Event:0. " + i + "/" + (Units.childCount - 1));
                            PCGScript.gameManagerScript.saveData.EventsX.Add(Mathf.RoundToInt(Buildings.GetChild(PCGScript.gameManagerScript.saveData.Castle).position.x));
                            PCGScript.gameManagerScript.saveData.EventsY.Add(Mathf.RoundToInt(Buildings.GetChild(PCGScript.gameManagerScript.saveData.Castle).position.z));
                        }
                    }
                }
            }
            // Create a peasant to collect resources.
            else if (isAlly)
            {
                Name = CreateName();
                Unit.GetComponent<unitManager>().Name = Name;
                PCGScript.gameManagerScript.saveData.Names.Add(Name);
                if (whatUnit == 7)
                {
                    Unit.GetComponent<unitManager>().Schedule = "Patrol Home";
                    Unit.GetComponent<unitManager>().Title = "Warrior";
                    PCGScript.gameManagerScript.saveData.Titles.Add("Warrior");
                }
                else
                {
                    Unit.GetComponent<unitManager>().Title = "Peasant";
                    PCGScript.gameManagerScript.saveData.Titles.Add("Peasant");
                }
                // Updates Portrait
                int random = Random.Range(0, availableOtherIcons.Count);
                Unit.GetComponent<unitManager>().Portrait = availableOtherIcons[random];
                PCGScript.gameManagerScript.saveData.Portraits.Add(availableOtherIcons[random]);
                availableOtherIcons.Remove(availableOtherIcons[random]);
                if (whatUnit == 5) Unit.GetComponent<unitManager>().Schedule = "Gather Resources";
                if (whatUnit == 6) Unit.GetComponent<unitManager>().Schedule = "Gather Mine";
                

            }
            // Else make enemies and other AI (need to test FOW and no pointer etc)
            else
            {



            }

            Unit.GetComponent<unitManager>().isAlly = isAlly;
            PCGScript.gameManagerScript.saveData.Allies.Add(isAlly);
            PCGScript.gameManagerScript.saveData.UnitsX.Add(Mathf.RoundToInt(startPos.x)); PCGScript.gameManagerScript.saveData.UnitsY.Add(Mathf.RoundToInt(startPos.z));
            PCGScript.gameManagerScript.saveData.Schedules.Add(Unit.GetComponent<unitManager>().Schedule);
        }
        else
        {
            // Calls the save data to set the units.
            Unit.GetComponent<unitManager>().Home = PCGScript.gameManagerScript.saveData.Homes[Units.childCount - 1];
            Unit.GetComponent<unitManager>().Name = PCGScript.gameManagerScript.saveData.Names[Units.childCount - 1];
            Unit.GetComponent<unitManager>().Title = PCGScript.gameManagerScript.saveData.Titles[Units.childCount - 1];
            Unit.GetComponent<unitManager>().Portrait = PCGScript.gameManagerScript.saveData.Portraits[Units.childCount - 1];
            // Removes the Portraits
            if (Unit.GetComponent<unitManager>().Portrait < 5) availablePriestIcons.Remove(Unit.GetComponent<unitManager>().Portrait);
            else if (Unit.GetComponent<unitManager>().Portrait >= 5 && Unit.GetComponent<unitManager>().Portrait < 11) availableMageIcons.Remove(Unit.GetComponent<unitManager>().Portrait);
            else if (Unit.GetComponent<unitManager>().Portrait >= 11 && Unit.GetComponent<unitManager>().Portrait < 17) availableKnightIcons.Remove(Unit.GetComponent<unitManager>().Portrait);
            else if (Unit.GetComponent<unitManager>().Portrait >= 17) availableOtherIcons.Remove(Unit.GetComponent<unitManager>().Portrait);
            Unit.GetComponent<unitManager>().isAlly = PCGScript.gameManagerScript.saveData.Allies[Units.childCount - 1];
            Unit.GetComponent<unitManager>().Schedule = PCGScript.gameManagerScript.saveData.Schedules[Units.childCount - 1];
        }

        // Sets the other values.
        Unit.GetComponent<unitManager>().PCGScript = PCGScript;
        Unit.GetComponent<unitManager>().Model.GetComponent<pathfindingManager>().movementSpeed = 1.75f;
        Unit.GetComponent<unitManager>().Model.GetComponent<pathfindingManager>().PCGScript = PCGScript;

        // Creates pointer towards AI.
        if (Unit.GetComponent<unitManager>().isAlly)
        {
            GameObject EventPointer = Instantiate(eventPointer);
            EventPointer.transform.SetParent(EventsCanvas);
            EventPointer.GetComponent<TargetIndicator>().Initialise(Unit.GetComponent<unitManager>().Model.transform, PCGScript.FOWTilemap, storyManagerScript);
        }
        else { }
        // Initialises the AI.
        int whatVisual;
        // Updates the model visuals based on what the portrait shows.
        if (Unit.GetComponent<unitManager>().Portrait == 1 || Unit.GetComponent<unitManager>().Portrait == 2) whatVisual = 0;
        else if (Unit.GetComponent<unitManager>().Portrait == 3 || Unit.GetComponent<unitManager>().Portrait == 4) whatVisual = 2;
        else if (Unit.GetComponent<unitManager>().Portrait == 5 || Unit.GetComponent<unitManager>().Portrait == 6 || Unit.GetComponent<unitManager>().Portrait == 7 || Unit.GetComponent<unitManager>().Portrait == 11 ||
            Unit.GetComponent<unitManager>().Portrait == 12 || Unit.GetComponent<unitManager>().Portrait == 13) whatVisual = 1;
        else if (Unit.GetComponent<unitManager>().Portrait == 8 || Unit.GetComponent<unitManager>().Portrait == 9 || Unit.GetComponent<unitManager>().Portrait == 10 || Unit.GetComponent<unitManager>().Portrait == 14 ||
            Unit.GetComponent<unitManager>().Portrait == 15 || Unit.GetComponent<unitManager>().Portrait == 16) whatVisual = 3;
        else
        {
            if (Unit.GetComponent<unitManager>().Portrait % 2 == 0) whatVisual = 2;
            else whatVisual = 0;
        }
        Unit.GetComponent<unitManager>().Model.GetComponent<pathfindingManager>().Initialise(this, whatVisual);
        if (newUnit) yield return new WaitForSeconds(1.5f);
    }
    // Randomly generates names for each Unit.
    public string CreateName()
    {
        string name = "";
        int nameLength = Random.Range(2, 5);
        string[] firstComponent = new string[] { "W", "J", "R", "H", "T", "A", "M", "I", "E", "B", "C", "Y", "P" };
        string[] secondComponent = new string[] { "ill", "ohn", "ich", "ob", "en", "al", "om", "alt", "og", "u", "ali", "at", "agn", "ar", "o", "isa", "ea", "ab", "ec", "ul", "el", "es" };
        string[] thirdComponent = new string[] { "ia", "ai", "ar", "er", "ry", "ph", "as", "er", "gh", "ce", "il", "es", "gar", "an", "bella", "ma", "tri", "el", "ilia", "y", "iz", "h" };
        string[] fourthComponent = new string[] { "m", "n", "d", "t", "y", "h", "s", "r", "e", "a", "l" };
        for (int i = 0; i < nameLength; i++)
        {
            if (i == 0) name += firstComponent[Random.Range(0, firstComponent.Length)];
            if (i == 1) name += secondComponent[Random.Range(0, secondComponent.Length)];
            if (i == 2) name += thirdComponent[Random.Range(0, thirdComponent.Length)];
            if (i == 3) name += fourthComponent[Random.Range(0, fourthComponent.Length)];
        }
        return name;
    }
    public void ActionText(string text) { StartCoroutine(actionText(text)); }
    IEnumerator actionText(string text)
    {
        yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused && !PCGScript.gameManagerScript.Dialogue);
        GameObject tempText = Instantiate(actionsTextPrefab);
        LeanTween.alphaCanvas(tempText.GetComponent<CanvasGroup>(), 1, 0.5f).setEase(LeanTweenType.easeInQuad);
        tempText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        tempText.transform.SetParent(Actions.transform.GetChild(0), false);
        yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused && !PCGScript.gameManagerScript.Dialogue);
        yield return new WaitForSeconds(10f);
        yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused && !PCGScript.gameManagerScript.Dialogue);
        LeanTween.alphaCanvas(tempText.GetComponent<CanvasGroup>(), 0, 2.5f).setEase(LeanTweenType.easeInQuad);
        yield return new WaitForSeconds(2.5f);
        Destroy(tempText);
    }
}