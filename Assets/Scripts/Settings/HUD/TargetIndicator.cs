// Reveals where the event/cutscene is happening so players scroll to click on it.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TargetIndicator : MonoBehaviour
{
    [HideInInspector] public StoryManager storyManagerScript;
    public int whichEvent;
    public string[] units;
    public Transform targetPosition;
    public List<Sprite> Sprites;
    float borderSize;
    bool FOW = false;
    bool Event = false;
    // Checks if this event is visible or not to determine if it appears on the HUD. (can replace this with when I create events instead)
    public void Initialise(Transform TargetPosition, Tilemap FOWTilemap, StoryManager StoryManagerScript)
    {
        targetPosition = TargetPosition; int whichTarget;
        if (StoryManagerScript != null) storyManagerScript = StoryManagerScript;
        if (FOWTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(targetPosition.position.x - 3), Mathf.RoundToInt(targetPosition.position.z - 1), 0)) != null &&
           (FOWTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(targetPosition.position.x - 3), Mathf.RoundToInt(targetPosition.position.z - 1), 0)).name == "TinyRTSEnvironment_0" ||
            FOWTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(targetPosition.position.x - 3), Mathf.RoundToInt(targetPosition.position.z - 1), 0)).name == "TinyRTSEnvironment_1"))
        { FOW = true; return; }
        // If the target is a castle, character, or event.     
        if (targetPosition.name == "Castle") { whichTarget = 0; borderSize = (Sprites[0].rect.width / 2); }
        else if (targetPosition.name == "Model") { whichTarget = 1; borderSize = (Sprites[0].rect.width) + (Sprites[1].rect.width / 2); }
        else { whichTarget = 2; borderSize = (Sprites[0].rect.width) + (Sprites[1].rect.width) + (Sprites[2].rect.width / 2); Event = true; }
        transform.GetChild(0).GetComponent<Image>().sprite = Sprites[whichTarget];
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(Sprites[whichTarget].rect.width, Sprites[whichTarget].rect.height);
    }
    void Update()
    {
        if (!FOW)
        {
            Vector3 targetPositionScreenPoint =  Camera.main.WorldToScreenPoint(targetPosition.position);
            targetPositionScreenPoint = new Vector3(targetPositionScreenPoint.x, targetPositionScreenPoint.y, 0);
            bool isOffScreen = targetPositionScreenPoint.x <= 0 || targetPositionScreenPoint.x >= Screen.width || targetPositionScreenPoint.y <= 0 || targetPositionScreenPoint.y >= Screen.height;

            // When off screen follows the position of the event.
            if (isOffScreen)
            {
                if (storyManagerScript.PCGScript.gameManagerScript.Dialogue || (targetPosition.name == "Model" && targetPosition.transform.GetComponent<pathfindingManager>().isIndoors))
                {
                    transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
                    transform.GetChild(0).gameObject.SetActive(false);
                    return;
                }
                else if (transform.GetChild(0).GetComponent<CanvasGroup>().alpha == 0)
                {
                    if (!Event) transform.GetChild(0).gameObject.SetActive(true);
                    transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
                }
                RotatePointer();
                Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
                if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
                else if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
                if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
                else if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;

                transform.GetChild(0).position = cappedTargetScreenPosition;
            }
            // Else if its on screen make it invisible and remain on top of event.
            else if (transform.GetChild(0).gameObject.activeSelf)
            {
                transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
                transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
                transform.GetChild(0).position = targetPositionScreenPoint;
                if (!Event) transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    void RotatePointer()
    {
        Vector3 toPosition = new Vector3(targetPosition.position.x, targetPosition.position.z, 0);
        Vector3 fromPosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.z, 0);
        fromPosition.z = 0;

        // Changes rotation based on the two positions.
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) % 360;
        transform.GetChild(0).localEulerAngles = new Vector3(0, 0, angle);
    }    
    // Triggers the cutscene when clicked.
    public void Triggered()
    {
        if (!Camera.main.transform.parent.GetChild(1).GetComponent<CameraMove>().enabled || storyManagerScript.PCGScript.gameManagerScript.Dialogue) return;
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition.position);
        bool isOffScreen = targetPositionScreenPoint.x <= 0 || targetPositionScreenPoint.x >= Screen.width || targetPositionScreenPoint.y <= 0 || targetPositionScreenPoint.y >= Screen.height;

        // When off screen zooms to the event.
        if (isOffScreen)
        {
            if (targetPosition.name == "Castle") Camera.main.transform.parent.GetChild(1).position = new Vector3(targetPosition.position.x, Camera.main.transform.position.y, targetPosition.position.z + 1f);
            else if (targetPosition.name == "Model") Camera.main.transform.parent.GetChild(1).position = new Vector3(targetPosition.position.x, Camera.main.transform.position.y, targetPosition.position.z);
            else Camera.main.transform.parent.GetChild(1).position = new Vector3(targetPosition.position.x, Camera.main.transform.position.y, targetPosition.position.z);
            return;
        }
        else { Camera.main.transform.parent.GetChild(1).position = new Vector3(targetPosition.position.x, Camera.main.transform.position.y, targetPosition.position.z + 2.5f); }
        // Else its a cutscene
        Camera.main.transform.parent.GetChild(0).GetComponent<CameraFollow>().Reset();
        Camera.main.transform.parent.GetChild(1).position = new Vector3(Mathf.RoundToInt(targetPosition.position.x), 0, Mathf.RoundToInt(targetPosition.position.z + 3f));
        Camera.main.transform.parent.GetChild(1).GetComponent<CameraMove>().enabled = false;
        storyManagerScript.StartDialogue(whichEvent, units);
        Destroy(targetPosition.gameObject);
        Destroy(this.gameObject);
    }
}