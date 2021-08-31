// Controls and moves the Player Camera as well as prevents it from reaching outside the play area.
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject Castle;
    public float panSpeed;
    Vector3 mousePosition;
    GameManager gameManagerScript;
    void Start() { gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>(); }
    void Update()
    {
        Vector3 fakeNewPos = transform.position;
        // If player is holding space then snap back to castle.
        if (!gameManagerScript.Paused && !gameManagerScript.Dialogue)
        {
            if (Input.GetKey(KeyCode.Space)) transform.position = new Vector3(Castle.transform.position.x, Castle.transform.position.y, Castle.transform.position.z + 1);
            // Controls camera movement.
            else
            {
                Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
                //Check if the Mouse Wheel is clicked, and then move the Camera when held down.
                if (screenRect.Contains(Input.mousePosition) && Input.GetMouseButtonDown(2)) mousePosition = Input.mousePosition;
                if (Input.GetMouseButton(2))
                {
                    if (Input.mousePosition.y > mousePosition.y) fakeNewPos -= (transform.forward * (mousePosition.y - Input.mousePosition.y) * 0.0025f) * Time.deltaTime / Time.timeScale * panSpeed;
                    if (Input.mousePosition.y < mousePosition.y) fakeNewPos += (transform.forward * (Input.mousePosition.y - mousePosition.y) * 0.0025f) * Time.deltaTime / Time.timeScale * panSpeed;
                    if (Input.mousePosition.x > mousePosition.x) fakeNewPos -= (transform.right * (mousePosition.x - Input.mousePosition.x) * 0.0025f) * Time.deltaTime / Time.timeScale * panSpeed;
                    if (Input.mousePosition.x < mousePosition.x) fakeNewPos += (transform.right * (Input.mousePosition.x - mousePosition.x) * 0.0025f) * Time.deltaTime / Time.timeScale * panSpeed;
                }
                else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)
                    || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) fakeNewPos += transform.forward * Time.deltaTime / Time.timeScale * panSpeed;
                    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) fakeNewPos -= transform.forward * Time.deltaTime / Time.timeScale * panSpeed;
                    if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) fakeNewPos += transform.right * Time.deltaTime / Time.timeScale * panSpeed;
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) fakeNewPos -= transform.right * Time.deltaTime / Time.timeScale * panSpeed;
                }
                else fakeNewPos = new Vector3(Mathf.Round(fakeNewPos.x), Mathf.Round(fakeNewPos.y), Mathf.Round(fakeNewPos.z));
                // Updates the values.
                transform.position = fakeNewPos;
            }
        }
        if (Input.GetMouseButtonUp(2)) mousePosition = Vector3.zero;

        // Prevents the Camera exceeding the play area.
        if (transform.position.x <= 0)
            transform.position = new Vector3(0, transform.position.y, transform.position.z);    
        else if (transform.position.x >= 50)
            transform.position = new Vector3(50, transform.position.y, transform.position.z);

        if (transform.position.z <= 0)
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        else if (transform.position.z >= 100)
            transform.position = new Vector3(transform.position.x, transform.position.y, 100);
    }
}