// Simple script to follow the CameraMove.cs wherever it goes.
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraFollow : MonoBehaviour
{
    Camera mainCamera;
    public AIManager AIManagerScript;
    public Transform cameraTransform;
    [HideInInspector] public Vector3 _cameraOffset;
    public PCG PCGScript;
    public GameObject informationBorder;
    public GameObject Selector;
    GameManager gameManagerScript;

    void Start()
    {
        _cameraOffset = transform.position - cameraTransform.position;
        mainCamera = this.GetComponent<Camera>();
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    // Updates the position of the Camera based on the CameraMove.cs inputs.
    Transform unitPosition;
    void Update()
    {
        Vector3 newPos = cameraTransform.position + _cameraOffset;
        if (!gameManagerScript.Paused)
        {
            float distancePos = Vector2.Distance(transform.position, newPos);
            if (distancePos < 5f) distancePos = 5f; else if (distancePos > 20f) distancePos = 20f;
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * distancePos);
            //transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, Time.deltaTime * distancePos);

            if (Input.GetMouseButtonDown(0) && Input.mousePosition.x <= 1210 && Input.mousePosition.y >= 50 && !gameManagerScript.Paused && !gameManagerScript.Dialogue)
            {
                unitPosition = null;
                int layerMaskUI = 1 << 5; int layerMaskBuilding = 1 << 10; int layerMaskUnit = 1 << 11;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskUI)) Reset();
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskBuilding))
                {
                    if (PCGScript.FOWTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(hit.transform.parent.position.x - 3), Mathf.RoundToInt(hit.transform.parent.position.z - 1), 0)) != null && 
                       (PCGScript.FOWTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(hit.transform.parent.position.x - 3), Mathf.RoundToInt(hit.transform.parent.position.z - 1), 0)).name == "TinyRTSEnvironment_0" ||
                        PCGScript.FOWTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(hit.transform.parent.position.x - 3), Mathf.RoundToInt(hit.transform.parent.position.z - 1), 0)).name == "TinyRTSEnvironment_1"))
                    { Reset(); return; }
                    // Updates the information and sets the Selector based on size and position.
                    informationBorder.SetActive(true);
                    informationBorder.transform.GetChild(0).GetComponent<Image>().sprite = AIManagerScript.unitIcons[0];
                    informationBorder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = hit.transform.parent.GetComponent<selectorInformation>().Title;
                    informationBorder.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = hit.transform.parent.GetComponent<selectorInformation>().SubTitle;

                    informationBorder.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color(0.627451f, 0.5960785f, 0.6156863f, 1);
                    if (hit.transform.parent.GetComponent<selectorInformation>().SubTitle == gameManagerScript.saveData.Name + "'s Territory") { informationBorder.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color(0.9019608f, 0.4470588f, 0.1764706f, 1); }

                    //Selector.transform.position = new Vector3(Mathf.RoundToInt(hit.transform.parent.position.x), 0, Mathf.RoundToInt(hit.transform.parent.position.z));
                    unitPosition = hit.transform.parent;
                    Selector.SetActive(true);
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskUnit))
                {
                    if (PCGScript.FOWTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(hit.transform.parent.position.x - 3), Mathf.RoundToInt(hit.transform.parent.position.z - 1), 0)) != null && 
                       (PCGScript.FOWTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(hit.transform.parent.position.x - 3), Mathf.RoundToInt(hit.transform.parent.position.z - 1), 0)).name == "TinyRTSEnvironment_0" ||
                        PCGScript.FOWTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(hit.transform.parent.position.x - 3), Mathf.RoundToInt(hit.transform.parent.position.z - 1), 0)).name == "TinyRTSEnvironment_1"))
                    { Reset(); return; }
                    // Updates the information and sets the Selector based on size and position.
                    informationBorder.SetActive(true);
                    informationBorder.transform.GetChild(0).GetComponent<Image>().sprite = AIManagerScript.unitIcons[hit.transform.parent.GetComponent<unitManager>().Portrait];
                    informationBorder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = hit.transform.parent.GetComponent<unitManager>().Name;
                    informationBorder.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = hit.transform.parent.GetComponent<unitManager>().Title;

                    informationBorder.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color(0.627451f, 0.5960785f, 0.6156863f, 1);
                    if (hit.transform.parent.GetComponent<unitManager>().isAlly) { informationBorder.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color(0.9019608f, 0.4470588f, 0.1764706f, 1); }
                    unitPosition = hit.transform;
                    Selector.SetActive(true);
                }
                else Reset();
            }
            if (unitPosition != null && Selector.activeSelf) Selector.transform.position = new Vector3(unitPosition.transform.position.x, 0, unitPosition.transform.position.z);
            else if (Selector.activeSelf && unitPosition == null) Reset();
        }
    }
    public void Reset()
    {
        // Resets the information Tile and the Selectors
        informationBorder.SetActive(false);
        informationBorder.transform.GetChild(0).GetComponent<Image>().sprite = AIManagerScript.unitIcons[0];
        informationBorder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
        informationBorder.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
        Selector.SetActive(false);
        unitPosition = null;
    }
}