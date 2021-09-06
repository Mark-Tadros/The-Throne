// Individual Unit behaviour trees that take commands from the kingdomManager.cs.
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class unitManager : MonoBehaviour
{
    [HideInInspector] public PCG PCGScript;
    public GameObject Model;
    public int Unit;
    public int Home;
    public string Name;
    public string Title;
    public int Portrait;
    public bool isAlly;
    public string Schedule;

    public Transform iconPrefab;
    GameObject iconObject;

    // Gets called once that Unit is created to carry out their schedule. (If no schedule stays in castle)
    public void Initialise()
    {
        if (Model.GetComponent<pathfindingManager>().isIndoors) IsIndoors();
        // Upon loading the game checks if inside the Mine or Building to prevent inconsistencies.
        if (Schedule.Contains("Mine")) StartCoroutine(Mine());
        else if (Schedule.Contains("Travel"))
        {
            Vector3 travelPos = new Vector3();
            if (Schedule.Contains("Home")) travelPos = PCGScript.Buildings.transform.GetChild(Home).GetChild(0).position;
            else if (Schedule.Contains("Building"))
            {
                string whichBuilding = Schedule.Replace("Travel Building ", "");
                int WhichBuilding; int.TryParse(whichBuilding, out WhichBuilding);
                travelPos = PCGScript.Buildings.transform.GetChild(WhichBuilding).GetChild(0).position;
            }

            StartCoroutine(Travel(travelPos));
        }
        else OnSchedule();

        StartCoroutine(TriggerAnimation());
    }
    void OnSchedule()
    {
        if (Schedule == "" || Schedule == "Event") StartCoroutine(noSchedule());
        else if (Model.GetComponent<pathfindingManager>().isIndoors) StartCoroutine(LeaveBuilding());
        else if (Schedule.Contains("Patrol"))
        {
            Vector3 roamPos = new Vector3();
            if (Schedule.Contains("Home")) roamPos = PCGScript.Buildings.transform.GetChild(Home).GetChild(0).position;

            StartCoroutine(Patrol(roamPos));
        }
        else if (Schedule.Contains("Travel"))
        {
            Vector3 travelPos = new Vector3();
            if (Schedule.Contains("Home")) travelPos = PCGScript.Buildings.transform.GetChild(Home).GetChild(0).position;
            else if (Schedule.Contains("Building"))
            {
                string whichBuilding = Schedule.Replace("Travel Building ", "");
                int WhichBuilding; int.TryParse(whichBuilding, out WhichBuilding);
                travelPos = PCGScript.Buildings.transform.GetChild(WhichBuilding).GetChild(0).position;
            }

            StartCoroutine(Travel(travelPos));
        }
        else if (Schedule.Contains("Move"))
        {
            string whichPositions = Schedule.Replace("Move ", "");
            string[] Positions = new string[] { };
            Positions = whichPositions.Split(',');

            StartCoroutine(Move(new Vector3(int.Parse(Positions[0]), 0, int.Parse(Positions[1]))));
        }
        else if (Schedule.Contains("Gather"))
        {
            if (Schedule.Contains("Resources")) StartCoroutine(Resource());
            if (Schedule.Contains("Mine")) StartCoroutine(Mine());
        }
    }
    IEnumerator noSchedule()
    {
        yield return new WaitUntil(() => Schedule != "" || Schedule != "Event");
        OnSchedule();
    }
    IEnumerator LeaveBuilding()
    {
        yield return new WaitUntil(() => PCGScript.AIManagerScript.Initialised);
        if (Schedule.Contains("Patrol") || Schedule.Contains("Gather")) yield return new WaitForSeconds(Random.Range(5f, 10f));
        yield return new WaitForSeconds(1.5f);
        yield return new WaitUntil(() => !PCGScript.Grid.CheckWall(Mathf.RoundToInt(Model.transform.position.x), Mathf.RoundToInt(Model.transform.position.z - 1)));
        if (Schedule.Contains("Event")) { OnSchedule(); yield break; }
        if (Schedule.Contains("Patrol"))
        {
            if (Title == "Warrior") PCGScript.AIManagerScript.ActionText(Name + " is going to go out on a short patrol.");
            else PCGScript.AIManagerScript.ActionText(Name + " " + Title.ToLower() + " is going to go out on a short patrol.");
        }
        if (Schedule.Contains("Resources")) PCGScript.AIManagerScript.ActionText(Name + " is going to go gather some resources.");
        if (Schedule.Contains("Mine")) PCGScript.AIManagerScript.ActionText(Name + " is going to go work in the mines.");
        IsOutdoors();
        yield return new WaitForSeconds(1.5f);
        OnSchedule();
    }
    IEnumerator Patrol(Vector3 Position)
    {
        yield return new WaitUntil(() => PCGScript.AIManagerScript.Initialised);
        yield return new WaitUntil(() => !Model.GetComponent<pathfindingManager>().isMoving);
        // Finds a valid position around the building to begin roaming towards.
        while (!Model.GetComponent<pathfindingManager>().isMoving)
        {
            int x = Random.Range(Mathf.RoundToInt(Position.x - 3f), Mathf.RoundToInt(Position.x + 4f));
            int y = Random.Range(Mathf.RoundToInt(Position.z - 4f), Mathf.RoundToInt(Position.z + 2f));
            while (PCGScript.Grid.CheckWall(x, y) && new Vector3(x, 0, y) != PCGScript.Buildings.transform.GetChild(Home).GetChild(0).position)
            {
                x = Random.Range(Mathf.RoundToInt(Position.x - 3f), Mathf.RoundToInt(Position.x + 4f));
                y = Random.Range(Mathf.RoundToInt(Position.z - 4f), Mathf.RoundToInt(Position.z + 2f));
                yield return null;
            }
            Model.GetComponent<pathfindingManager>().Target.transform.position = new Vector3(x, 0f, y);
            yield return new WaitForSeconds(0.5f);
            yield return null;
        }
        yield return new WaitUntil(() => !Model.GetComponent<pathfindingManager>().isMoving);
        yield return new WaitForSeconds(Random.Range(5f, 20f));
        // Returns Home.
        if (Schedule.Contains("Gather")) StartCoroutine(Travel(PCGScript.Buildings.transform.GetChild(Home).GetChild(0).position));
        else
        {
            if (!Schedule.Contains("Patrol")) OnSchedule();
            if (Random.value < 0.5f) StartCoroutine(Travel(PCGScript.Buildings.transform.GetChild(Home).GetChild(0).position));
            else StartCoroutine(Patrol(PCGScript.Buildings.transform.GetChild(Home).GetChild(0).position));
        }
    }
    IEnumerator Travel(Vector3 Position)
    {
        yield return new WaitUntil(() => PCGScript.AIManagerScript.Initialised);
        yield return new WaitUntil(() => !Model.GetComponent<pathfindingManager>().isMoving || new Vector3(Mathf.RoundToInt(Position.x), 0f, Mathf.RoundToInt(Position.z + 1)) == Model.transform.position);
        // Moves to the position directly below the building.
        while (!Model.GetComponent<pathfindingManager>().isMoving)
        {
            Model.GetComponent<pathfindingManager>().Target.transform.position = new Vector3(Mathf.RoundToInt(Position.x), 0f, Mathf.RoundToInt(Position.z));
            if (Model.GetComponent<pathfindingManager>().Target.transform.position == Model.transform.position) break;
            if (new Vector3(Mathf.RoundToInt(Position.x), 0f, Mathf.RoundToInt(Position.z + 1)) == Model.transform.position) break;
            yield return new WaitForSeconds(0.1f);
            yield return null;
        }
        yield return new WaitUntil(() => !Model.GetComponent<pathfindingManager>().isMoving);
        yield return new WaitForSeconds(0.25f);
        Model.transform.position = new Vector3(Position.x, 0, Position.z + 1);
        if (!Model.GetComponent<pathfindingManager>().isIndoors) IsIndoors();
        yield return new WaitForSeconds(Random.Range(7.5f, 15f));
        if (!Schedule.Contains("Patrol") && !Schedule.Contains("Gather"))
        {
            Schedule = "";
            PCGScript.gameManagerScript.saveData.Schedules[Unit] = "";
        }
        OnSchedule();
    }
    IEnumerator Move(Vector3 Position)
    {
        yield return new WaitUntil(() => PCGScript.AIManagerScript.Initialised);
        yield return new WaitUntil(() => !Model.GetComponent<pathfindingManager>().isMoving);
        // Moves to the position.
        while (!Model.GetComponent<pathfindingManager>().isMoving)
        {
            Model.GetComponent<pathfindingManager>().Target.transform.position = new Vector3(Mathf.RoundToInt(Position.x), 0f, Mathf.RoundToInt(Position.z));
            if (Model.GetComponent<pathfindingManager>().Target.transform.position == Model.transform.position) break;
            yield return new WaitForSeconds(0.1f);
            yield return null;
        }
        yield return new WaitUntil(() => !Model.GetComponent<pathfindingManager>().isMoving);
        yield return new WaitForSeconds(0.5f);
        Schedule = "";
        PCGScript.gameManagerScript.saveData.Schedules[Unit] = "";
        OnSchedule();
    }
    IEnumerator Resource()
    {
        yield return new WaitUntil(() => PCGScript.AIManagerScript.Initialised);
        yield return new WaitUntil(() => !Model.GetComponent<pathfindingManager>().isMoving);
        BoundsInt boundsResources = PCGScript.resourcesTilemap.cellBounds; TileBase[] allTilesResources = PCGScript.resourcesTilemap.GetTilesBlock(boundsResources);

        Vector3 Object = new Vector3(); float minDistance = Mathf.Infinity; bool lookRight = new bool(); string tileName = "";
        // Loops through and locates the closest available resource.
        for (int x = 0; x < boundsResources.size.x; x++)
        {
            for (int y = 0; y < boundsResources.size.y; y++)
            {
                TileBase tile = allTilesResources[x + y * boundsResources.size.x];
                if (tile != null && tile.name != "TinyRTSEnvironment_26" && tile.name != "TinyRTSEnvironment_27" && tile.name != "TinyRTSEnvironment_28" &&
                    tile.name != "TinyRTSEnvironment_42" && tile.name != "TinyRTSEnvironment_45" && tile.name != "TinyRTSEnvironment_50" && tile.name != "TinyRTSEnvironment_53" && tile.name != "TinyRTSEnvironment_55")
                {
                    float Distance = Vector3.Distance(new Vector3(x, 0, y), PCGScript.AIManagerScript.Buildings.GetChild(Home).transform.position);
                    if (Distance < minDistance)
                    {
                        if (!PCGScript.Grid.CheckWall(x + 1, y) || (x + 1 == Model.transform.position.x && y == Model.transform.position.z))
                        {
                            minDistance = Distance; lookRight = false; tileName = tile.name;
                            Object = new Vector3(x + 1, 0, y);
                        }
                        else if (!PCGScript.Grid.CheckWall(x - 1, y) || (x - 1 == Model.transform.position.x && y == Model.transform.position.z))
                        {
                            minDistance = Distance; lookRight = true; tileName = tile.name;
                            Object = new Vector3(x - 1, 0, y);
                        }
                    }
                }
            }
        }
        // If there are no more resources return home.
        if (minDistance >= 11 || Object == new Vector3())
        {
            StartCoroutine(Travel(PCGScript.Buildings.transform.GetChild(Home).GetChild(0).position));
            Schedule = "Travel Home";
            PCGScript.gameManagerScript.saveData.Schedules[Unit] = "Travel Home";
            yield break;
        }
        while (!Model.GetComponent<pathfindingManager>().isMoving)
        {
            // Move the AI to the available object location.
            Model.GetComponent<pathfindingManager>().Target.transform.position = Object;
            if (Model.GetComponent<pathfindingManager>().Target.transform.position == Model.transform.position) break;
            yield return new WaitForSeconds(0.1f);
            yield return null;
        }
        yield return new WaitUntil(() => !Model.GetComponent<pathfindingManager>().isMoving);
        yield return new WaitForSeconds(0.5f);
        // Starts playing the animation
        if (lookRight) Model.GetComponent<pathfindingManager>().Animator.SetBool("HRight", true);
        else Model.GetComponent<pathfindingManager>().Animator.SetBool("HLeft", true);
        yield return new WaitForSeconds(Random.Range(20f, 50f));
        // Removes the resource and adds it to the save data.
        Model.GetComponent<pathfindingManager>().Animator.SetBool("HRight", false); Model.GetComponent<pathfindingManager>().Animator.SetBool("HLeft", false);
        if (lookRight) PCGScript.UpdateTile(tileName, new Vector3(Object.x + 1, 0, Object.z));
        else PCGScript.UpdateTile(tileName, new Vector3(Object.x - 1, 0, Object.z));
        // Returns Home.
        if (Random.value < 0.75f) StartCoroutine(Travel(PCGScript.Buildings.transform.GetChild(Home).GetChild(0).position));
        else StartCoroutine(Resource());
    }
    IEnumerator Mine()
    {
        int whichBuilding = -1;
        if (Home == 0) whichBuilding = 18; else if (Home == 1) whichBuilding = 19; else if (Home == 2) whichBuilding = 20;
        Vector3 Position = PCGScript.Buildings.transform.GetChild(whichBuilding).GetChild(0).position;
        yield return new WaitUntil(() => PCGScript.AIManagerScript.Initialised);
        if (Model.transform.position != new Vector3(Position.x, 0, Position.z + 1))
        {
            if (Model.GetComponent<pathfindingManager>().isIndoors == true) { OnSchedule(); yield break; }
            yield return new WaitUntil(() => !Model.GetComponent<pathfindingManager>().isMoving);
            while (!Model.GetComponent<pathfindingManager>().isMoving)
            {
                Model.GetComponent<pathfindingManager>().Target.transform.position = new Vector3(Mathf.RoundToInt(Position.x), 0f, Mathf.RoundToInt(Position.z));
                if (Model.GetComponent<pathfindingManager>().Target.transform.position == Model.transform.position) break;
                yield return new WaitForSeconds(0.1f);
                yield return null;
            }
            yield return new WaitUntil(() => !Model.GetComponent<pathfindingManager>().isMoving);
            yield return new WaitForSeconds(0.25f);
            Model.transform.position = new Vector3(Position.x, 0, Position.z + 1);
            IsIndoors();
        }
        // Activates mine visuals.
        PCGScript.Buildings.transform.GetChild(whichBuilding).GetChild(3).GetChild(1).gameObject.SetActive(false);
        PCGScript.Buildings.transform.GetChild(whichBuilding).GetChild(3).GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(Random.Range(20f, 50f));
        // After return home.
        yield return new WaitUntil(() => !PCGScript.Grid.CheckWall(Mathf.RoundToInt(Model.transform.position.x), Mathf.RoundToInt(Model.transform.position.z - 1)));
        PCGScript.Buildings.transform.GetChild(whichBuilding).GetChild(3).GetChild(1).gameObject.SetActive(true);
        PCGScript.Buildings.transform.GetChild(whichBuilding).GetChild(3).GetChild(0).gameObject.SetActive(false);
        IsOutdoors();
        yield return new WaitUntil(() => !Model.GetComponent<pathfindingManager>().isIndoors);
        StartCoroutine(Travel(PCGScript.Buildings.transform.GetChild(Home).GetChild(0).position));
    }
    // Upon entering a village create a portrait icon and become deactivated.
    void IsIndoors()
    {
        Model.SetActive(false);
        UpdatePosition(new Vector2(Model.transform.position.x, Model.transform.position.z));
        Transform Icon = Instantiate(iconPrefab) as Transform;
        // Creates the Portrait in the first available spot for the Building.
        int whichBuilding = -1;
        for (int i = 0; i < Model.GetComponent<pathfindingManager>().AIManagerScript.Buildings.childCount; i++)
        {
            if (Model.transform.position == Model.GetComponent<pathfindingManager>().AIManagerScript.Buildings.GetChild(i).transform.position)
            {
                PCGScript.AIManagerScript.Grid.RemoveWall(Mathf.RoundToInt(Model.GetComponent<pathfindingManager>().AIManagerScript.Buildings.GetChild(i).GetChild(0).transform.position.x), Mathf.RoundToInt(Model.GetComponent<pathfindingManager>().AIManagerScript.Buildings.GetChild(i).GetChild(0).transform.position.z));
                Model.GetComponent<pathfindingManager>().isIndoors = true;
                for (int x = 0; x < 6; x++)
                {
                    if (Model.GetComponent<pathfindingManager>().AIManagerScript.Buildings.GetChild(i).GetChild(2).GetChild(x).childCount == 0)
                    {
                        whichBuilding = i;
                        if (isAlly)
                        {
                            Model.GetComponent<pathfindingManager>().AIManagerScript.Buildings.GetChild(whichBuilding).GetChild(1).gameObject.SetActive(true);
                            Model.GetComponent<pathfindingManager>().AIManagerScript.Buildings.GetChild(whichBuilding).GetChild(1).GetComponent<FOWTrigger>().Trigger(PCGScript.FOWTilemap, PCGScript.gameManagerScript);
                        }
                        if (Title == "Peasant" || Title == "Warrior") { Destroy(Icon.gameObject); return; }
                        Icon.transform.position = Model.GetComponent<pathfindingManager>().AIManagerScript.Buildings.GetChild(i).GetChild(2).GetChild(x).transform.position;
                        Icon.transform.parent = Model.GetComponent<pathfindingManager>().AIManagerScript.Buildings.GetChild(i).GetChild(2).GetChild(x).transform;
                        break;
                    }
                }
                break;
            }
        }
        Icon.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Model.GetComponent<pathfindingManager>().AIManagerScript.unitInsideIcons[Portrait];
        Icon.GetComponent<unitManager>().Unit = Unit;
        Icon.GetComponent<unitManager>().Name = Name;
        Icon.GetComponent<unitManager>().Title = Title;
        Icon.GetComponent<unitManager>().Portrait = Portrait;
        Icon.GetComponent<unitManager>().isAlly = isAlly;
        if (isAlly)
        {
            Model.GetComponent<pathfindingManager>().AIManagerScript.Buildings.GetChild(whichBuilding).GetChild(1).gameObject.SetActive(true);
            Model.GetComponent<pathfindingManager>().AIManagerScript.Buildings.GetChild(whichBuilding).GetChild(1).GetComponent<FOWTrigger>().Trigger(PCGScript.FOWTilemap, PCGScript.gameManagerScript);
        }
        iconObject = Icon.gameObject;
    }
    void IsOutdoors()
    {
        Model.transform.position = new Vector3(Model.transform.position.x, Model.transform.position.y, Model.transform.position.z - 1);
        Model.SetActive(true);
        UpdatePosition(new Vector2(Model.transform.position.x, Model.transform.position.z));
        PCGScript.AIManagerScript.Grid.SetWall(Mathf.RoundToInt(Model.transform.position.x), Mathf.RoundToInt(Model.transform.position.z));
        Model.GetComponent<pathfindingManager>().isIndoors = false;
        if (Title == "Peasant" || Title == "Warrior") return;
        // Moves all the portraits up one once this one is removed.
        Transform Portraits = iconObject.transform.parent.parent;
        iconObject.transform.parent = Portraits.parent;
        Destroy(iconObject);
        for (int i = 1; i < Portraits.childCount; i++)
        {
            if (Portraits.GetChild(i - 1).childCount == 0 && Portraits.GetChild(i).childCount != 0)
            {
                Portraits.GetChild(i).GetChild(0).parent = Portraits.GetChild(i - 1).transform;
                Portraits.GetChild(i - 1).GetChild(0).localPosition = Vector3.zero;
            }
        }
    }
    // Calls the gameManagerScript to update the units position.
    public void UpdatePosition(Vector2 updatePos)
    {
        PCGScript.gameManagerScript.saveData.UnitsX[Unit] = Mathf.RoundToInt(updatePos.x);
        PCGScript.gameManagerScript.saveData.UnitsY[Unit] = Mathf.RoundToInt(updatePos.y);
    }
    IEnumerator TriggerAnimation()
    {
        while (true)
        {
            yield return new WaitUntil(() => Model.activeSelf);
            yield return new WaitUntil(() => PCGScript.gameManagerScript.Paused || PCGScript.gameManagerScript.Dialogue);
            Model.GetComponent<pathfindingManager>().Animator.enabled = false;
            yield return new WaitUntil(() => !PCGScript.gameManagerScript.Paused && !PCGScript.gameManagerScript.Dialogue);
            Model.GetComponent<pathfindingManager>().Animator.enabled = true;
            yield return null;
        }
    }
}