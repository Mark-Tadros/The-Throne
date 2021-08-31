// Controls the entire dialogue system as well as reads the data and sets the new events.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Subtegral.DialogueSystem.DataContainers;

public class StoryManager : MonoBehaviour
{
    public PCG PCGScript;
    public GameObject Dialogue;
    public DialogueContainer[] eventDialogue;
    public GameObject eventPointer;
    public GameObject eventTrigger;
    public Transform EventsCanvas;
    public Transform Events;
    public List<GameObject> dialogueTextPrefabs;

    public IEnumerator createEvent(bool newEvent, int whichEvent)
    {
        yield return new WaitUntil(() => !PCGScript.gameManagerScript.Dialogue);
        UnityEngine.Debug.Log("Started Event " + whichEvent);
        if (newEvent)
        {
            PCGScript.gameManagerScript.saveData.Events.Remove(whichEvent);
            PCGScript.gameManagerScript.saveData.currentEvents.Add(whichEvent);
        }
        // If that event has characters then call it and update their positions.
        Vector3 eventPosition = new Vector3(); string[] units = new string[] { };
        for (int i = 0; i < PCGScript.gameManagerScript.saveData.currentEvents.Count; i++)
        {
            if (PCGScript.gameManagerScript.saveData.eventsPeople[i].Contains("Event:" + whichEvent + "."))
            {
                int whichBuilding = -1;
                eventPosition = new Vector3(PCGScript.gameManagerScript.saveData.EventsX[i], 1f, PCGScript.gameManagerScript.saveData.EventsY[i] + 0.5f);
                // Checks if the location is a building or else its a random position = -1.
                for (int x = 0; x < PCGScript.Buildings.transform.childCount; x++)
                {
                    if (PCGScript.Buildings.transform.GetChild(x).transform.position.x == PCGScript.gameManagerScript.saveData.EventsX[i] && 
                        PCGScript.Buildings.transform.GetChild(x).transform.position.z == PCGScript.gameManagerScript.saveData.EventsY[i])
                    {
                        whichBuilding = x;
                        break;
                    }
                }
                string whichUnits = PCGScript.gameManagerScript.saveData.eventsPeople[i].Replace("Event:" + whichEvent + ". ", "");
                units = whichUnits.Split('/');
                // If the location goes to a building set all the units to go to that building.
                if (whichBuilding != -1)
                {
                    for (int x = 0; x < units.Length; x++)
                    {
                        if (PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Model.transform.position.x != PCGScript.gameManagerScript.saveData.EventsX[i] || PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Model.transform.position.z != PCGScript.gameManagerScript.saveData.EventsY[i])
                        {
                            yield return new WaitUntil(() => !PCGScript.gameManagerScript.Dialogue);
                            yield return new WaitUntil(() => PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Schedule == "" || PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Schedule == "Event");
                            PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Schedule = "Travel Building " + whichBuilding;
                            PCGScript.gameManagerScript.saveData.Schedules[int.Parse(units[x])] = "Travel Building " + whichBuilding;
                        }
                    }
                }
                // Else move to a specific location.
                else
                {
                    for (int x = 0; x < units.Length; x++)
                    {
                        if (PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Model.transform.position.x != PCGScript.gameManagerScript.saveData.EventsX[i] || PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Model.transform.position.z != PCGScript.gameManagerScript.saveData.EventsY[i])
                        {
                            yield return new WaitUntil(() => !PCGScript.gameManagerScript.Dialogue);
                            yield return new WaitUntil(() => PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Schedule == "" || PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Schedule == "Event");
                            PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Schedule = "Travel Location " + PCGScript.gameManagerScript.saveData.EventsX[i] + "," + PCGScript.gameManagerScript.saveData.EventsY[i];
                            PCGScript.gameManagerScript.saveData.Schedules[int.Parse(units[x])] = "Travel Location " + PCGScript.gameManagerScript.saveData.EventsX[i] + "," + PCGScript.gameManagerScript.saveData.EventsY[i];
                        }
                    }
                }
                // Waits until each unit arrives
                for (int x = 0; x < units.Length; x++)
                {
                    yield return new WaitUntil(() => !PCGScript.gameManagerScript.Dialogue);
                    yield return new WaitUntil(() => PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Model.transform.position.x == PCGScript.gameManagerScript.saveData.EventsX[i] &&
                                                     PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Model.transform.position.z == PCGScript.gameManagerScript.saveData.EventsY[i]);
                    yield return new WaitUntil(() => PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Schedule == "" || PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Schedule == "Event");
                    PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>().Schedule = "Event";
                    PCGScript.gameManagerScript.saveData.Schedules[int.Parse(units[x])] = "Event";
                }
                break;
            }
        }
        UnityEngine.Debug.Log("Units Arrived");
        
        GameObject EventPointer = Instantiate(eventPointer);
        EventPointer.transform.SetParent(EventsCanvas);

        GameObject EventTrigger = Instantiate(eventTrigger);
        EventTrigger.transform.SetParent(Events);
        EventTrigger.transform.position = eventPosition;

        EventPointer.GetComponent<TargetIndicator>().Initialise(EventTrigger.transform, PCGScript.FOWTilemap, this);
        EventPointer.GetComponent<TargetIndicator>().whichEvent = whichEvent;
        EventPointer.GetComponent<TargetIndicator>().units = units;
    }
    public void StartDialogue(int whichEvent, string[] units)
    {
        PCGScript.gameManagerScript.Dialogue = true;

        DialogueContainer dialogue = eventDialogue[whichEvent]; var narrativeData = dialogue.NodeLinks.First();
        StartCoroutine(startDialogue(true, whichEvent, dialogue, narrativeData.TargetNodeGUID, units));
    }
    bool correctDialogue = true; IEnumerator startDialogue(bool newEvent, int whichEvent, DialogueContainer dialogue, string narrativeDataGUID, string[] units)
    {
        if (newEvent)
        {
            // Activate the Dialogue box and check if theres a narration or normal text.
            Dialogue.SetActive(true);
            LeanTween.alphaCanvas(Dialogue.transform.GetChild(0).GetComponent<CanvasGroup>(), 1, 1.5f).setEase(LeanTweenType.easeInOutQuad);
            Dialogue.transform.GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "click to skip...";
            yield return new WaitForSeconds(1f);
        }
        // Loops through all the Units currently involved in this cutscene.
        List<unitManager> Units = new List<unitManager>();
        for (int x = 0; x < units.Length; x++)
        {
            Units.Add(PCGScript.AIManagerScript.Units.GetChild(int.Parse(units[x])).GetComponent<unitManager>());
        }
        // Assigns text values.
        var text = dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
        var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);

        // Replaces all keywords with appropriate players based on the units.
        string tempString = ProcessProperties(eventDialogue[whichEvent], text);
        tempString = UpdateString(tempString, Units);
        // Creates the dialogue boxes based on each input.
        creatingString = false;
        StartCoroutine(CreateString(tempString, Units));
        yield return new WaitUntil(() => creatingString);        

        // Moves to the next dialogue choice.
        int choicesCount = 0; foreach (NodeLinkData choice in choices) choicesCount++;
        // There is no choice so proceed to next dialogue.
        if (choicesCount == 1)
        {
            foreach (NodeLinkData choice in choices) StartCoroutine(startDialogue(false, whichEvent, dialogue, choice.TargetNodeGUID, units));
            yield break;
        }
        // Else there is an option here activate player text input and create narrative dialogue box.
        else if (choicesCount > 1)
        {
            correctDialogue = false; sentDialogue = false;
            Dialogue.transform.GetChild(2).GetChild(1).GetComponent<TMP_InputField>().interactable = true;
            Dialogue.transform.GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "type response...";
            // Loops through until an answer breaks out of the available choices.
            while (!correctDialogue)
            {
                textInput = ""; string[] Inputs = new string[] { }; int whichInput = 0;
                yield return new WaitUntil(() => sentDialogue);
                //List<string> texts = new List<string>();
                foreach (NodeLinkData choice in choices)
                {
                    whichInput++;
                    // Else the command does not fit into any of the answers play the last one.
                    if (whichInput == 3)
                    {
                        var textResponse = dialogue.DialogueNodeData.Find(x => x.NodeGUID == choice.TargetNodeGUID).DialogueText;
                        // Replaces all keywords with appropriate players based on the units.
                        tempString = ProcessProperties(eventDialogue[whichEvent], textResponse);
                        tempString = UpdateString(tempString, Units);
                        // Creates the dialogue boxes based on each input.
                        creatingString = false;
                        StartCoroutine(CreateString(tempString, Units));
                        break;
                    }
                    // Loops through all the available choices and check against the inputs.
                    else
                    {
                        Inputs = dialogue.DialogueNodeData.Find(x => x.NodeGUID == choice.TargetNodeGUID).DialogueText.Split(',');
                        List<string> InputsLower = Inputs.Select(x => x.ToLower()).ToList();
                        // If it does contain the same trigger then enter that trigger.
                        if (InputsLower.Contains(textInput.ToLower()))
                        {
                            var textChoice = dialogue.DialogueNodeData.Find(x => x.NodeGUID == choice.TargetNodeGUID).DialogueText;
                            var choicesChoice = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == choice.TargetNodeGUID);
                            foreach (NodeLinkData choiceChoice in choicesChoice) StartCoroutine(startDialogue(false, whichEvent, dialogue, choiceChoice.TargetNodeGUID, units));
                            Dialogue.transform.GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "click to skip...";
                            correctDialogue = true;
                            break;
                        }
                    }
                }
                // If they did not send the correct response.
                Dialogue.transform.GetChild(2).GetChild(1).GetComponent<TMP_InputField>().text = "";
                sentDialogue = false;
                yield return null;
            }
            Dialogue.transform.GetChild(2).GetChild(1).GetComponent<TMP_InputField>().interactable = false;
            yield break;
        }
        // Else there is no more dialogue wait until they click exit to continue to code below.
        else
        {
            revealText = false; stopwatch.Reset();
            Dialogue.transform.GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "click to close...";
            yield return new WaitUntil(() => revealText);
        }
        LeanTween.alphaCanvas(Dialogue.transform.GetChild(0).GetComponent<CanvasGroup>(), 0, 1.5f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.alphaCanvas(Dialogue.transform.GetChild(2).GetComponent<CanvasGroup>(), 0, 1.5f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(0.5f);
        // Removes the Events data and adds them to usedEvents once its triggered and finished.
        PCGScript.gameManagerScript.Dialogue = false;
        Camera.main.transform.parent.GetChild(1).GetComponent<CameraMove>().enabled = true;
        PCGScript.gameManagerScript.saveData.currentEvents.Remove(whichEvent);
        for (int i = 0; i < PCGScript.gameManagerScript.saveData.eventsPeople.Count; i++)
        {
            if (PCGScript.gameManagerScript.saveData.eventsPeople[i].Contains("Event:" + whichEvent + "."))
            {
                PCGScript.gameManagerScript.saveData.eventsPeople.Remove(PCGScript.gameManagerScript.saveData.eventsPeople[i]);
                PCGScript.gameManagerScript.saveData.EventsX.Remove(PCGScript.gameManagerScript.saveData.EventsX[i]);
                PCGScript.gameManagerScript.saveData.EventsY.Remove(PCGScript.gameManagerScript.saveData.EventsY[i]);
                break;
            }
        }
        PCGScript.gameManagerScript.saveData.usedEvents.Add(whichEvent);
        yield return new WaitForSeconds(1f);
        foreach (Transform child in Dialogue.transform.GetChild(2).GetChild(0).GetChild(0)) Destroy(child.gameObject);
        //Dialogue.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        LayoutRebuilder.ForceRebuildLayoutImmediate(Dialogue.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>());
        Dialogue.SetActive(false);
    }
    string UpdateString(string tempString, List<unitManager> Units)
    {
        if (tempString.Contains("$Player$")) tempString = tempString.Replace("$Player$", PCGScript.gameManagerScript.saveData.Name);
        if (tempString.Contains("$Name1$")) tempString = tempString.Replace("$Name1$", Units[0].Title);
        if (tempString.Contains("$Title1$")) tempString = tempString.Replace("$Title1$", Units[0].Title);
        if (tempString.Contains("$Name2$")) tempString = tempString.Replace("$Name2$", Units[0].Title);
        if (tempString.Contains("$Title2$")) tempString = tempString.Replace("$Title2$", Units[1].Title);
        return tempString;
    }
    bool creatingString;
    IEnumerator CreateString(string tempString, List<unitManager> Units)
    {
        if (tempString.Contains("$Intro$"))
        {
            // Activates the dialogue boxes.
            Dialogue.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            tempString = tempString.Replace("$Intro$", "");
            // Reveals the current Cutscene text by text.
            Dialogue.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = tempString;
            LeanTween.alphaCanvas(Dialogue.transform.GetChild(1).GetComponent<CanvasGroup>(), 1, 1.5f).setEase(LeanTweenType.easeInOutQuad);
            yield return new WaitForSeconds(1.5f);
            revealText = false; stopwatch.Reset();
            // Loops to the next dialogue option automatically.
            float t = 0; while (t < 3.5f)
            {
                if (revealText) break;
                t += Time.deltaTime;
                yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused);
                yield return null;
            }
            // Activates the chat boxes.
            LeanTween.alphaCanvas(Dialogue.transform.GetChild(1).GetComponent<CanvasGroup>(), 0, 1f).setEase(LeanTweenType.easeInOutQuad);
            yield return new WaitForSeconds(1f);
            Dialogue.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            LeanTween.alphaCanvas(Dialogue.transform.GetChild(2).GetComponent<CanvasGroup>(), 1, 1.5f).setEase(LeanTweenType.easeInOutQuad);
            yield return new WaitForSeconds(1.75f);
        }
        // Else if character one or two
        else if (tempString.Contains("$One$") || tempString.Contains("$Two$"))
        {
            // Creates the Dialogue box based on who spoke.
            GameObject dialogueText;
            if (tempString.Contains("$One$"))
            {
                tempString = tempString.Replace("$One$", "");
                dialogueText = Instantiate(dialogueTextPrefabs[1]);
                dialogueText.transform.GetChild(0).GetComponent<Image>().sprite = PCGScript.AIManagerScript.unitInsideIcons[Units[0].Portrait];
                dialogueText.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Units[0].Name + " " + Units[0].Title;
                if (Units[0].isAlly) { dialogueText.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(0.9019608f, 0.4470588f, 0.1764706f, 1); }
            }
            else
            {
                tempString = tempString.Replace("$Two$", "");
                dialogueText = Instantiate(dialogueTextPrefabs[2]);
                dialogueText.transform.GetChild(0).GetComponent<Image>().sprite = PCGScript.AIManagerScript.unitInsideIcons[Units[1].Portrait];
                dialogueText.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Units[1].Name + " " + Units[1].Title;
                if (Units[1].isAlly) { dialogueText.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(0.9019608f, 0.4470588f, 0.1764706f, 1); }
            }

            dialogueText.transform.SetParent(Dialogue.transform.GetChild(2).GetChild(0).GetChild(0), false);
            LeanTween.alphaCanvas(dialogueText.GetComponent<CanvasGroup>(), 1, 0.5f).setEase(LeanTweenType.easeInQuad);
            dialogueText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";

            revealText = false; stopwatch.Reset(); float currentSize;
            foreach (char character in tempString.ToCharArray())
            {
                currentSize = dialogueText.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta.y;
                if (revealText)
                {
                    dialogueText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = tempString;
                    // Rebuilds the layout and alligns the chat box to the very bottom.
                    LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueText.transform.GetChild(2).GetComponent<RectTransform>());
                    if (currentSize != dialogueText.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta.y)
                    {
                        dialogueText.GetComponent<RectTransform>().sizeDelta = new Vector2(dialogueText.GetComponent<RectTransform>().sizeDelta.x, dialogueText.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y + dialogueText.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta.y + 10);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueText.GetComponent<RectTransform>());
                        LayoutRebuilder.ForceRebuildLayoutImmediate(Dialogue.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>());
                        Dialogue.transform.GetChild(2).GetChild(0).GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
                    }
                    break;
                }
                dialogueText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text += character;
                yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused);
                yield return new WaitForSeconds(0.04f);
                // Rebuilds the layout and alligns the chat box to the very bottom.
                LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueText.transform.GetChild(2).GetComponent<RectTransform>());
                if (currentSize != dialogueText.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta.y)
                {
                    dialogueText.GetComponent<RectTransform>().sizeDelta = new Vector2(dialogueText.GetComponent<RectTransform>().sizeDelta.x, dialogueText.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y + dialogueText.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta.y + 10);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueText.GetComponent<RectTransform>());
                    LayoutRebuilder.ForceRebuildLayoutImmediate(Dialogue.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>());
                    Dialogue.transform.GetChild(2).GetChild(0).GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
                }
            }
            // Loops to the next dialogue option automatically.
            revealText = false; stopwatch.Reset();
            float t = 0; while (t < 0.5f)
            {
                if (revealText) break;
                t += Time.deltaTime;
                yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused);
                yield return null;
            }
        }
        else if (tempString.Contains("$Narrate$"))
        {
            GameObject dialogueText;
            tempString = tempString.Replace("$Narrate$", "");
            dialogueText = Instantiate(dialogueTextPrefabs[0]);
            dialogueText.transform.SetParent(Dialogue.transform.GetChild(2).GetChild(0).GetChild(0), false);

            dialogueText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = tempString;
            LeanTween.alphaCanvas(dialogueText.GetComponent<CanvasGroup>(), 1, 1f).setEase(LeanTweenType.easeInQuad);
            // Rebuilds the layout and alligns the chat box to the very bottom.
            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueText.transform.GetChild(2).GetComponent<RectTransform>());
            dialogueText.GetComponent<RectTransform>().sizeDelta = new Vector2(dialogueText.GetComponent<RectTransform>().sizeDelta.x, dialogueText.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y + dialogueText.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta.y + 25);
            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueText.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(Dialogue.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>());
            Dialogue.transform.GetChild(2).GetChild(0).GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
            yield return new WaitForSeconds(1f);
            revealText = false; stopwatch.Reset();
            // Loops to the next dialogue option automatically.
            float t = 0; while (t < 0.5f)
            {
                if (revealText) break;
                t += Time.deltaTime;
                yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused);
                yield return null;
            }
        }
        // Else creates some new events at the end.
        else if (tempString.Contains("$Event$"))
        {
            tempString = tempString.Replace("$Event$", "");
            string[] eventsString = new string[] { };
            eventsString = tempString.Split(',');
            // Updates the Units Schedules based on the dialogue choices.
            for (int x = 0; x < Units.Count; x++)
            {
                Units[x].Schedule = eventsString[x];
                PCGScript.gameManagerScript.saveData.Schedules[Units[x].Unit] = eventsString[x];
            }            
            // Carry out the other events such as creating units and updating priorities.
            if (eventsString[2] != "")
            {
                string[] events = new string[] { };
                events = eventsString[2].Split('-');
                for (int i = 0; i < events.Length; i++)
                {
                    if (events[i].Contains("Create Unit"))
                    {
                        PCGScript.AIManagerScript.CreateUnit(true, int.Parse(events[i].Replace("Create Unit ", "")), PCGScript.gameManagerScript.saveData.Castle, PCGScript.AIManagerScript.Buildings.GetChild(PCGScript.gameManagerScript.saveData.Castle).position, true);
                    }
                    else if (events[i].Contains("Event:"))
                    {
                        string[] eventConditions = new string[] { };
                        eventConditions = events[i].Split('.');
                        if (eventConditions[1].Contains("$Unit0$")) eventConditions[1] = eventConditions[1].Replace("$Unit0$", Units[0].Unit.ToString());
                        if (eventConditions[1].Contains("$Unit1$")) eventConditions[1] = eventConditions[1].Replace("$Unit1$", Units[1].Unit.ToString());
                        PCGScript.gameManagerScript.saveData.Events.Add(int.Parse(eventConditions[0].Replace("Event:", "")));
                        PCGScript.gameManagerScript.saveData.eventsPeople.Add(eventConditions[0] + ". " + eventConditions[1]);
                        if (eventConditions[2] == "Village")
                        {
                            int whichVillage;
                            if (PCGScript.gameManagerScript.saveData.Castle == 0) whichVillage = 9;
                            else if (PCGScript.gameManagerScript.saveData.Castle == 1) whichVillage = 10;
                            else whichVillage = 11;

                            PCGScript.gameManagerScript.saveData.EventsX.Add(Mathf.RoundToInt(PCGScript.AIManagerScript.Buildings.GetChild(whichVillage).position.x));
                            PCGScript.gameManagerScript.saveData.EventsY.Add(Mathf.RoundToInt(PCGScript.AIManagerScript.Buildings.GetChild(whichVillage).position.z));
                        }
                        else if (eventConditions[2] == "Farm")
                        {
                            int whichFarm;
                            if (PCGScript.gameManagerScript.saveData.Castle == 0) whichFarm = Random.Range(21, 23);
                            else if (PCGScript.gameManagerScript.saveData.Castle == 1) whichFarm = Random.Range(24, 26);
                            else whichFarm = Random.Range(27, 29);

                            PCGScript.gameManagerScript.saveData.EventsX.Add(Mathf.RoundToInt(PCGScript.AIManagerScript.Buildings.GetChild(whichFarm).position.x));
                            PCGScript.gameManagerScript.saveData.EventsY.Add(Mathf.RoundToInt(PCGScript.AIManagerScript.Buildings.GetChild(whichFarm).position.z));
                        }
                    }
                }
            }
        }
        creatingString = true;
    }    
    // Change the values.
    string textInput; public void ChangeText(string newText)
    {
        textInput = newText;
        if (textInput == "" || textInput == " ") Dialogue.transform.GetChild(2).GetChild(2).GetComponent<Button>().interactable = false;
        else Dialogue.transform.GetChild(2).GetChild(2).GetComponent<Button>().interactable = true;
    }
    bool sentDialogue; public void SendText() { sentDialogue = true; Dialogue.transform.GetChild(2).GetChild(2).GetComponent<Button>().interactable = false; }
    // Updates the click transition but without detecting dragging.
    Stopwatch stopwatch = new Stopwatch(); bool revealText; void Update()
    {
        if (!revealText && Input.mousePosition.x <= 1210 && Input.mousePosition.y >= 50 && !PCGScript.gameManagerScript.Paused)
        {
            if (Input.GetMouseButtonDown(0)) stopwatch.Start();
            else if (Input.GetMouseButtonUp(0))
            {
                if (stopwatch.ElapsedMilliseconds > 0 && stopwatch.ElapsedMilliseconds < 0.5f * 1000) revealText = true;
                stopwatch.Reset();
            }
        }
        if (!correctDialogue && Dialogue.transform.GetChild(2).GetChild(2).GetComponent<Button>().interactable) { if (Input.GetKeyDown(KeyCode.Return)) { SendText(); } }
    }
    public string ProcessProperties(DialogueContainer dialogue, string text)
    {
        foreach (var exposedProperty in dialogue.ExposedProperties)
        {
            text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue);
        }
        return text;
    }
}