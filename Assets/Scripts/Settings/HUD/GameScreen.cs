// initialises the game scene.
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameScreen : MonoBehaviour
{
    public GameObject cameraObject;
    public GameObject Transition;
    public GridManager gridManagerScript;
    public AIManager AIManagerScript;
    public PCG PCGScript;
    public GameObject portraitIcon;
    public GameObject titleScreen;
    public GameObject Cutscene;
    public GameObject Controls;
    public GameObject eventPointerHome;

    // Plays an initial cutscene.
    public IEnumerator Initialise(GameManager GameManagerScript)
    {
        // If there is a save then change the wording to continue as well as reveal and change the name/portrait.
        gridManagerScript.Initialise(GameManagerScript);
        yield return new WaitUntil(() => PCGScript.Initialised);
        if (PCGScript.gameManagerScript.saveData.Name != "")
        {
            // Loads the FOW from the SaveData
            titleScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "[click to continue game]";
            portraitIcon.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PCGScript.gameManagerScript.saveData.Name;
            portraitIcon.transform.GetChild(0).GetComponent<Image>().sprite = AIManagerScript.playerIcons[PCGScript.gameManagerScript.saveData.Portrait];
            portraitIcon.transform.GetChild(2).gameObject.SetActive(true);
        }
        // Else applies the FOW Trigger to reveal the new map.
        else
        {
            PCGScript.Castles[GameManagerScript.saveData.Castle].transform.GetChild(1).gameObject.SetActive(true);
            PCGScript.Castles[GameManagerScript.saveData.Castle].transform.GetChild(1).GetComponent<FOWTrigger>().Trigger(PCGScript.FOWTilemap, PCGScript.gameManagerScript);
        }
        // Moves the Camera to the Castle.
        cameraObject.transform.GetChild(1).GetComponent<CameraMove>().Castle = PCGScript.Castles[PCGScript.gameManagerScript.saveData.Castle];
        cameraObject.transform.GetChild(0).position =
            new Vector3(PCGScript.Castles[PCGScript.gameManagerScript.saveData.Castle].transform.position.x, cameraObject.transform.GetChild(0).position.y, PCGScript.Castles[PCGScript.gameManagerScript.saveData.Castle].transform.position.z + 1);
        cameraObject.transform.GetChild(1).position =
            new Vector3(PCGScript.Castles[PCGScript.gameManagerScript.saveData.Castle].transform.position.x, PCGScript.Castles[PCGScript.gameManagerScript.saveData.Castle].transform.position.y, PCGScript.Castles[PCGScript.gameManagerScript.saveData.Castle].transform.position.z + 1);
        eventPointerHome.GetComponent<TargetIndicator>().targetPosition = PCGScript.Castles[PCGScript.gameManagerScript.saveData.Castle].transform;
        eventPointerHome.GetComponent<TargetIndicator>().Initialise(PCGScript.Castles[PCGScript.gameManagerScript.saveData.Castle].transform, PCGScript.FOWTilemap, PCGScript.AIManagerScript.storyManagerScript);
        if (PCGScript.gameManagerScript.saveData.Name != "") eventPointerHome.gameObject.SetActive(true);
        // Reveals the Scene.
        LeanTween.alphaCanvas(Transition.GetComponent<CanvasGroup>(), 0, 1.25f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(0.75f);
        LeanTween.alphaCanvas(titleScreen.transform.GetChild(1).GetComponent<CanvasGroup>(), 1, 1f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1f);
        LeanTween.alphaCanvas(titleScreen.transform.GetChild(2).GetComponent<CanvasGroup>(), 1, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.alphaCanvas(portraitIcon.GetComponent<CanvasGroup>(), 1, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(0.5f);
        Transition.SetActive(false);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) && !PCGScript.gameManagerScript.Paused);
        // Initialises the audio checks for WebGL.
        var result = FMODUnity.RuntimeManager.CoreSystem.mixerSuspend(); result = FMODUnity.RuntimeManager.CoreSystem.mixerResume();

        //gameManagerScript.PlaySound("StartGame");
        PCGScript.gameManagerScript.PlayOST("GameOST");
        // If there is a save then continue game, else start cutscene.
        if (PCGScript.gameManagerScript.saveData.Name != "")
        {
            // Activates the cameras and removes the Title Screen.
            LeanTween.alphaCanvas(titleScreen.transform.GetChild(1).GetComponent<CanvasGroup>(), 0, 0.75f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.alphaCanvas(titleScreen.transform.GetChild(2).GetComponent<CanvasGroup>(), 0, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.alphaCanvas(titleScreen.transform.GetChild(0).GetComponent<CanvasGroup>(), 0, 1f).setEase(LeanTweenType.easeInOutQuad);
            cameraObject.transform.GetChild(0).GetComponent<CameraFollow>().enabled = true;
            cameraObject.transform.GetChild(1).GetComponent<CameraMove>().enabled = true;
            yield return new WaitForSeconds(1f);
            PCGScript.gameManagerScript.optionsScript.optionsButton.gameObject.SetActive(true);
            LeanTween.alphaCanvas(PCGScript.gameManagerScript.optionsScript.optionsButton.GetComponent<CanvasGroup>(), 1, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            titleScreen.SetActive(false);
            AIManagerScript.Initialise();
        }
        else
        {
            LeanTween.alphaCanvas(titleScreen.transform.GetChild(1).GetComponent<CanvasGroup>(), 0, 0.75f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.alphaCanvas(titleScreen.transform.GetChild(2).GetComponent<CanvasGroup>(), 0, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            yield return new WaitForSeconds(1f);
            Cutscene.SetActive(true);
            LeanTween.alphaCanvas(Cutscene.GetComponent<CanvasGroup>(), 1, 1.5f).setEase(LeanTweenType.easeInOutQuad);
            PCGScript.gameManagerScript.optionsScript.optionsButton.gameObject.SetActive(true);
            LeanTween.alphaCanvas(PCGScript.gameManagerScript.optionsScript.optionsButton.GetComponent<CanvasGroup>(), 1, 1.5f).setEase(LeanTweenType.easeInOutQuad);
        }
    }    
    // Change the values.
    public void ChangeName(string newName)
    {
        PCGScript.gameManagerScript.saveData.Name = newName;
        if (PCGScript.gameManagerScript.saveData.Name == "") Cutscene.transform.GetChild(1).GetChild(1).GetComponent<Button>().interactable = false;
        else Cutscene.transform.GetChild(1).GetChild(1).GetComponent<Button>().interactable = true;
    }
    public void SetName()
    {
        Cutscene.transform.GetChild(1).GetChild(0).GetComponent<TMP_InputField>().interactable = false;
        Cutscene.transform.GetChild(1).GetChild(1).GetComponent<Button>().interactable = false;
        LeanTween.alphaCanvas(Cutscene.transform.GetChild(1).GetComponent<CanvasGroup>(), 0, 1f).setEase(LeanTweenType.easeInOutQuad);
        PCGScript.gameManagerScript.saveData.Portrait = Random.Range(1, 6);
        portraitIcon.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PCGScript.gameManagerScript.saveData.Name;
        portraitIcon.transform.GetChild(0).GetComponent<Image>().sprite = AIManagerScript.playerIcons[PCGScript.gameManagerScript.saveData.Portrait];
        portraitIcon.transform.GetChild(2).gameObject.SetActive(true);
        PCGScript.gameManagerScript.saveData.buildingsSubTitles[PCGScript.gameManagerScript.saveData.Castle] = PCGScript.gameManagerScript.saveData.Name + "'s Territory";
        PCGScript.Castles[PCGScript.gameManagerScript.saveData.Castle].transform.GetChild(3).GetComponent<selectorInformation>().SubTitle = PCGScript.gameManagerScript.saveData.Name + "'s Territory";
        AIManagerScript.NewGame();
        StartCoroutine(StartGame());
    }
    IEnumerator StartGame()
    {
        // Plays the Dialogue after player sets their name.
        yield return new WaitForSeconds(1);
        string templateText = Cutscene.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text;
        Cutscene.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
        Cutscene.transform.GetChild(2).gameObject.SetActive(true);
        // Reveals the current Cutscene text by text.
        revealText = false;
        foreach (char character in templateText.ToCharArray())
        {
            if (revealText) { Cutscene.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = templateText; break; }
            Cutscene.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text += character;
            yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused);
            yield return new WaitForSeconds(0.04f);
        }
        revealText = false;
        // Loops to the next dialogue option automatically.
        float t = 0; while (t < 1f)
        {
            if (revealText) break;
            t += Time.deltaTime;
            yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused);
            yield return null;
        }
        LeanTween.alphaCanvas(Cutscene.transform.GetChild(2).GetComponent<CanvasGroup>(), 0, 1f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1);
        // Reveals the next Cutscene text.
        templateText = Cutscene.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text;
        Cutscene.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "";
        Cutscene.transform.GetChild(3).gameObject.SetActive(true);
        // Reveals the current Cutscene text by text.
        revealText = false;
        foreach (char character in templateText.ToCharArray())
        {
            if (revealText) { Cutscene.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = templateText; break; }
            Cutscene.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text += character;
            yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused);
            yield return new WaitForSeconds(0.04f);
        }
        revealText = false;
        // Loops to the next dialogue option automatically.
        t = 0; while (t < 1f)
        {
            if (revealText) break;
            t += Time.deltaTime;
            yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused);
            yield return null;
        }
        LeanTween.alphaCanvas(Cutscene.transform.GetChild(3).GetComponent<CanvasGroup>(), 0, 1f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1);
        // Reveals the next Cutscene text.
        templateText = Cutscene.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text;
        Cutscene.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "";
        Cutscene.transform.GetChild(4).gameObject.SetActive(true);
        // Reveals the current Cutscene text by text.
        revealText = false;
        foreach (char character in templateText.ToCharArray())
        {
            if (revealText) { Cutscene.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = templateText; break; }
            Cutscene.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text += character;
            yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused);
            yield return new WaitForSeconds(0.04f);
        }
        revealText = false;
        // Loops to the next dialogue option automatically.
        t = 0; while (t < 1f)
        {
            if (revealText) break;
            t += Time.deltaTime;
            yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused);
            yield return null;
        }
        LeanTween.alphaCanvas(Cutscene.transform.GetChild(4).GetComponent<CanvasGroup>(), 0, 1f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1);
        LeanTween.alphaCanvas(Cutscene.GetComponent<CanvasGroup>(), 0, 1).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1);
        LeanTween.alphaCanvas(titleScreen.transform.GetChild(0).GetComponent<CanvasGroup>(), 0, 1f).setEase(LeanTweenType.easeInOutQuad);
        cameraObject.transform.GetChild(0).GetComponent<CameraFollow>().enabled = true;
        cameraObject.transform.GetChild(1).GetComponent<CameraMove>().enabled = true;
        eventPointerHome.gameObject.SetActive(true);
        // Activates and slowly reveals the Controls.
        Controls.SetActive(true);
        Controls.transform.position = new Vector3(PCGScript.Castles[PCGScript.gameManagerScript.saveData.Castle].transform.position.x, 0, PCGScript.Castles[PCGScript.gameManagerScript.saveData.Castle].transform.position.z + 1.675f);
        LeanTween.value(this.gameObject, SetAlpha, 0, 0.9f, 1.5f).setEase(LeanTweenType.easeInOutQuad);
        Vector3 oldPosition = Camera.main.transform.parent.GetChild(1).transform.position;
        yield return new WaitUntil(() => new Vector3(Mathf.RoundToInt(Camera.main.transform.parent.GetChild(1).transform.position.x), Mathf.RoundToInt(Camera.main.transform.parent.GetChild(1).transform.position.y), Mathf.RoundToInt(Camera.main.transform.parent.GetChild(1).transform.position.z)) != oldPosition);
        yield return new WaitForSeconds(1f);
        titleScreen.SetActive(false); Cutscene.SetActive(false);
        LeanTween.value(this.gameObject, SetAlpha, 0.9f, 0, 2f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(2f);
        AIManagerScript.Initialise();
        yield return new WaitUntil(() => PCGScript.gameManagerScript.saveData.currentEvents.Count != 0 && PCGScript.AIManagerScript.storyManagerScript.Events.childCount != 0);
        LeanTween.value(this.gameObject, SetAlphaLast, 0, 0.95f, 1.5f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1.5f);
        yield return new WaitUntil(() => PCGScript.gameManagerScript.Dialogue);
        LeanTween.value(this.gameObject, SetAlphaLast, 0.95f, 0, 1.5f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1.5f);
        Controls.SetActive(false);
    }
    void SetAlpha(float value) { foreach (Transform child in Controls.transform) if (child != Controls.transform.GetChild(Controls.transform.childCount - 1)) child.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, value); }
    void SetAlphaLast(float value) { Controls.transform.GetChild(Controls.transform.childCount - 1).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, value); }
    bool revealText; void Update()
    {
        if (!revealText && Input.GetMouseButtonDown(0) && Input.mousePosition.x <= 1210 && Input.mousePosition.y >= 50 && !PCGScript.gameManagerScript.Paused)
            revealText = true;
        if (Cutscene.activeSelf && Input.GetKeyDown(KeyCode.KeypadEnter) && Cutscene.transform.GetChild(1).GetChild(0).gameObject.activeSelf && Cutscene.transform.GetChild(1).GetChild(0).GetComponent<TMP_InputField>().interactable && Cutscene.transform.GetChild(1).GetChild(0).GetComponent<TMP_InputField>().text != "")
            SetName();
    }
}