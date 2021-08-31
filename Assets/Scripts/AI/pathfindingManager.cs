// Controls the Pathfinding and Movement for each individual Unit.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathfindingManager : MonoBehaviour
{
    [HideInInspector] public AIManager AIManagerScript;
    public GameObject Target;
    public Transform Animation;
    public Animator Animator;
    public FOWTrigger FOWDraw;
    [HideInInspector] public PCG PCGScript;
    public bool isIndoors; public bool isStuck; public bool isMoving;

    List<Vector2Int> Path;
    Vector3 oldPosition; Vector3 unitPosition;

    public void Initialise (AIManager aiManagerScript, int whatUnit)
    {
        AIManagerScript = aiManagerScript; oldPosition = transform.position; unitPosition = transform.position;
        //if (!isIndoors) AIManagerScript.Grid.SetWall(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        Animation.GetChild(whatUnit).gameObject.SetActive(true);
        Animator = Animation.GetChild(whatUnit).GetComponent<Animator>();
        transform.parent.GetComponent<unitManager>().Initialise();
    }
    // If the Target position is different then the Unit position start moving automatically.
    void Update() { if (oldPosition != null && Target.transform.position != oldPosition) FindPath(); }
    // Finds a Path from the Unit to the Target.
    void FindPath()
    {
        if (isIndoors || AIManagerScript == null) return;
        StopCoroutine("FollowPath");
        isStuck = false; isMoving = false;
        Target.transform.position = new Vector3(Mathf.RoundToInt(Target.transform.position.x), Mathf.RoundToInt(Target.transform.position.y), Mathf.RoundToInt(Target.transform.position.z));
        // Remove the Wall from where the Unit is currently standing to calculate.
        Path = new List<Vector2Int>();
        Path = AIManagerScript.Grid.ReturnPath(
            new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)),
            new Vector2Int(Mathf.RoundToInt(Target.transform.position.x), Mathf.RoundToInt(Target.transform.position.z))
            );

        if (Path == null) { Debug.Log("No path available"); isStuck = true; }
        else
        {
            isMoving = true;
            // Adds a Wall where the Unit will end up to prevent overlapping.
            //AIManagerScript.Grid.RemoveWall(Mathf.RoundToInt(oldPosition.x), Mathf.RoundToInt(oldPosition.z));
            AIManagerScript.Grid.RemoveWall(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
            oldPosition = Target.transform.position;
            if (!transform.parent.GetComponent<unitManager>().Schedule.Contains("Travel"))
                AIManagerScript.Grid.SetWall(Mathf.RoundToInt(Target.transform.position.x), Mathf.RoundToInt(Target.transform.position.z));
            StartCoroutine("FollowPath");
        }
    }
    // Continously Follow that Path until it reaches its destination.
    [HideInInspector] public float movementSpeed;
    IEnumerator FollowPath()
    {
        Debug.Log("Following Path");
        // Removes the current position to prevent backtracking.
        Path.Remove(Path[0]);
        while (Path.Count > 0)
        {           
            // Moves the Unit to the closest path, before removing it and moving on.
            float maxDistance = Time.deltaTime * movementSpeed;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(Path[0].x, 0f, Path[0].y), maxDistance);
            yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused && !PCGScript.gameManagerScript.Dialogue);
            // Plays movement animation.
            if (Mathf.Round(transform.position.x * 10f) / 10f != Mathf.Round(unitPosition.x * 10f) / 10f || Mathf.Round(transform.position.z * 10f) / 10f != Mathf.Round(unitPosition.z * 10f) / 10f)
            {
                Vector2 velocity = 
                    new Vector2((Mathf.Round(transform.position.x * 10f) / 10f) - (Mathf.Round(unitPosition.x * 10f) / 10f), (Mathf.Round(transform.position.z * 10f) / 10f) - (Mathf.Round(unitPosition.z * 10f) / 10f));
                unitPosition = transform.position;
                Animator.SetFloat("SpeedX", -velocity.y); Animator.SetFloat("SpeedY", velocity.x);
            }
            // Upon reaching the destination, subtract one from Paths and repeat.
            if (transform.position == new Vector3(Path[0].x, 0f, Path[0].y))
            {
                // Draws the FOW Filter.
                if (transform.parent.GetComponent<unitManager>().isAlly)
                {
                    FOWDraw.gameObject.SetActive(true);
                    FOWDraw.Trigger(PCGScript.FOWTilemap, PCGScript.gameManagerScript);
                    yield return new WaitUntil(() => !FOWDraw.gameObject.activeSelf);
                }
                Path.Remove(Path[0]);
            }
            yield return null;
        }
        // Resets movement animation and variables.
        transform.parent.GetComponent<unitManager>().UpdatePosition(new Vector2(transform.position.x, transform.position.z));
        Animator.SetFloat("SpeedX", 0); Animator.SetFloat("SpeedY", 0);
        unitPosition = transform.position;
        isMoving = false;
    }
}