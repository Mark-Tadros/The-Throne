// Controls the map and obstacles.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PCG : MonoBehaviour
{
    [HideInInspector] public GameManager gameManagerScript;
    public AIManager AIManagerScript;
    public GridManager Grid;
    public Tilemap waterTilemap; public Tilemap obstaclesTilemap;
    public Tilemap resourcesTilemap;
    public List<Tile> resourcesSprites;
    public Tilemap FOWTilemap;
    public List<Tile> FOWSprites;
    public GameObject Buildings;
    public List<GameObject> buildingsAssets;
    public List<GameObject> Castles;
    [HideInInspector] public bool Initialised;

    public void Initialise(GameManager GameManagerScript)
    {
        gameManagerScript = GameManagerScript;
        // Updates resources visuals based on save data.
        for (int i = 0; i < gameManagerScript.saveData.Resources.Count; i++)
        {
            if (gameManagerScript.saveData.Resources[i] == -1) resourcesTilemap.SetTile(new Vector3Int(gameManagerScript.saveData.ResourcesX[i], gameManagerScript.saveData.ResourcesY[i], 0), null);
            else resourcesTilemap.SetTile(new Vector3Int(gameManagerScript.saveData.ResourcesX[i], gameManagerScript.saveData.ResourcesY[i], 0), resourcesSprites[gameManagerScript.saveData.Resources[i]]);
        }
        // Updates the FOW Script based on the save data.
        for (int i = 0; i < gameManagerScript.saveData.FOW.Count; i++)
        {
            if (gameManagerScript.saveData.FOW[i] == -1) FOWTilemap.SetTile(new Vector3Int(gameManagerScript.saveData.FOWX[i], gameManagerScript.saveData.FOWY[i], 0), null);
            else FOWTilemap.SetTile(new Vector3Int(gameManagerScript.saveData.FOWX[i], gameManagerScript.saveData.FOWY[i], 0), FOWSprites[gameManagerScript.saveData.FOW[i]]);
        }
        // Updates the buildings information based on the save data.
        for (int i = 0; i < gameManagerScript.saveData.Buildings.Count; i++)
        {
            if (gameManagerScript.saveData.Buildings[i] == -1) { Buildings.transform.GetChild(i).gameObject.SetActive(false); }
            else
            {
                GameObject Building = 
                    Instantiate(buildingsAssets[gameManagerScript.saveData.Buildings[i]], Buildings.transform.GetChild(i).transform.position, Buildings.transform.GetChild(i).transform.rotation, Buildings.transform.GetChild(i).transform);
                Building.GetComponent<selectorInformation>().Title = gameManagerScript.saveData.buildingsTitles[i];
                Building.GetComponent<selectorInformation>().SubTitle = gameManagerScript.saveData.buildingsSubTitles[i];
                if (gameManagerScript.saveData.Buildings[i] == 2 || gameManagerScript.saveData.Buildings[i] == 7 || gameManagerScript.saveData.Buildings[i] == 13)
                    Buildings.transform.GetChild(i).GetChild(2).transform.localPosition = new Vector3(-0.5f, 0, 2.25f);
                else if (gameManagerScript.saveData.Buildings[i] == 9) Buildings.transform.GetChild(i).GetChild(2).transform.localPosition = new Vector3(0, 0, 2.25f);
                else if (gameManagerScript.saveData.Buildings[i] == 5 || gameManagerScript.saveData.Buildings[i] == 6)
                    Buildings.transform.GetChild(i).GetChild(2).transform.localPosition = new Vector3(0.5f, 0, 2.25f);
                for (int x = 0; x < Building.transform.childCount; x++)
                {
                    if (Building.transform.GetChild(x).GetComponent<BoxCollider>())
                        Grid.SetWall(Mathf.RoundToInt(Building.transform.GetChild(x).transform.position.x), Mathf.RoundToInt(Building.transform.GetChild(x).transform.position.z));
                }
            }
        }
        // Creates the pathfinding based on the Water, Obstacles, and updated Resources TileMaps.
        BoundsInt boundsWater = waterTilemap.cellBounds; TileBase[] allTilesWater = waterTilemap.GetTilesBlock(boundsWater);
        for (int x = 0; x < boundsWater.size.x; x++)
        {
            for (int y = 0; y < boundsWater.size.y; y++)
            {
                TileBase tile = allTilesWater[x + y * boundsWater.size.x];
                if (tile != null) Grid.SetWall(x, y - 2);
            }
        }
        BoundsInt boundsObstacle = obstaclesTilemap.cellBounds; TileBase[] allTilesObstacle = obstaclesTilemap.GetTilesBlock(boundsObstacle);
        for (int x = 0; x < boundsObstacle.size.x; x++)
        {
            for (int y = 0; y < boundsObstacle.size.y; y++)
            {
                TileBase tile = allTilesObstacle[x + y * boundsObstacle.size.x];
                if (tile != null) Grid.SetWall(x, y);
            }
        }        
        BoundsInt boundsResources = resourcesTilemap.cellBounds; TileBase[] allTilesResources = resourcesTilemap.GetTilesBlock(boundsResources);
        for (int x = 0; x < boundsResources.size.x; x++)
        {
            for (int y = 0; y < boundsResources.size.y; y++)
            {
                TileBase tile = allTilesResources[x + y * boundsResources.size.x];
                if (tile != null && tile.name != "TinyRTSEnvironment_26" && tile.name != "TinyRTSEnvironment_27" && tile.name != "TinyRTSEnvironment_28" &&
                    tile.name != "TinyRTSEnvironment_42" && tile.name != "TinyRTSEnvironment_45" && tile.name != "TinyRTSEnvironment_50" && tile.name != "TinyRTSEnvironment_53" && tile.name != "TinyRTSEnvironment_55")
                    Grid.SetWall(x, y);
            }
        }
        AIManagerScript.InitialiseValues();
        // Creates the Units based on the save data.
        for (int i = 0; i < gameManagerScript.saveData.Names.Count; i++)
        {
            AIManagerScript.CreateUnit(false, -1, -1, new Vector3(gameManagerScript.saveData.UnitsX[i], 0, gameManagerScript.saveData.UnitsY[i]), gameManagerScript.saveData.Allies[i]);
        }
        // Creates the Events based on the save data.
        for (int i = 0; i < gameManagerScript.saveData.currentEvents.Count; i++)
        {
            AIManagerScript.CreateEvent(false, gameManagerScript.saveData.currentEvents[i]);
        }
        // Allows the GameScreen.cs script to continue.
        Initialised = true;
    }
    public void UpdateTile(string tileName, Vector3 tilePos)
    {
        if (tileName == "TinyRTSEnvironment_31")
        {
            gameManagerScript.saveData.Resources.Add(-1);
            resourcesTilemap.SetTile(new Vector3Int(Mathf.RoundToInt(tilePos.x - 3), Mathf.RoundToInt(tilePos.z - 1), 0), null);
        }
        else if (tileName == "TinyRTSEnvironment_39" || tileName == "TinyRTSEnvironment_40" || tileName == "TinyRTSEnvironment_47" || tileName == "TinyRTSEnvironment_43" || tileName == "TinyRTSEnvironment_46")
        {
            int randomValue;
            if (Random.value < 0.75) randomValue = 0;
            else randomValue = 1;
            gameManagerScript.saveData.Resources.Add(randomValue);
            resourcesTilemap.SetTile(new Vector3Int(Mathf.RoundToInt(tilePos.x - 3), Mathf.RoundToInt(tilePos.z - 1), 0), resourcesSprites[randomValue]);
        }
        else if (tileName == "TinyRTSEnvironment_41" || tileName == "TinyRTSEnvironment_44" || tileName == "TinyRTSEnvironment_49" || tileName == "TinyRTSEnvironment_52" || tileName == "TinyRTSEnvironment_55")
        {
            int randomValue;
            if (Random.value < 0.75) randomValue = 1;
            else randomValue = 0;
            gameManagerScript.saveData.Resources.Add(randomValue);
            resourcesTilemap.SetTile(new Vector3Int(Mathf.RoundToInt(tilePos.x - 3), Mathf.RoundToInt(tilePos.z - 1), 0), resourcesSprites[randomValue]);
            // Also adds their little top piece.
            if (resourcesTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(tilePos.x - 3), Mathf.RoundToInt(tilePos.z), 0)) != null)
            {
                if (resourcesTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(tilePos.x - 3), Mathf.RoundToInt(tilePos.z), 0)).name == "TinyRTSEnvironment_42" || resourcesTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(tilePos.x - 3), Mathf.RoundToInt(tilePos.z), 0)).name == "TinyRTSEnvironment_45" ||
                    resourcesTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(tilePos.x - 3), Mathf.RoundToInt(tilePos.z), 0)).name == "TinyRTSEnvironment_50" || resourcesTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(tilePos.x - 3), Mathf.RoundToInt(tilePos.z), 0)).name == "TinyRTSEnvironment_53" || resourcesTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(tilePos.x - 3), Mathf.RoundToInt(tilePos.z), 0)).name == "TinyRTSEnvironment_55")
                {
                    gameManagerScript.saveData.Resources.Add(-1);
                    resourcesTilemap.SetTile(new Vector3Int(Mathf.RoundToInt(tilePos.x - 3), Mathf.RoundToInt(tilePos.z), 0), null);
                    gameManagerScript.saveData.ResourcesX.Add(Mathf.RoundToInt(tilePos.x - 3));
                    gameManagerScript.saveData.ResourcesY.Add(Mathf.RoundToInt(tilePos.z));
                }
            }
        }
        else if (tileName == "TinyRTSEnvironment_48" || tileName == "TinyRTSEnvironment_51" || tileName == "TinyRTSEnvironment_57")
        {
            int randomValue;
            if (Random.value < 0.75) randomValue = 1;
            else randomValue = 0;
            gameManagerScript.saveData.Resources.Add(randomValue);
            resourcesTilemap.SetTile(new Vector3Int(Mathf.RoundToInt(tilePos.x - 3), Mathf.RoundToInt(tilePos.z - 1), 0), resourcesSprites[randomValue]);
        }
        // Sets the values for the save data.
        gameManagerScript.saveData.ResourcesX.Add(Mathf.RoundToInt(tilePos.x - 3));
        gameManagerScript.saveData.ResourcesY.Add(Mathf.RoundToInt(tilePos.z - 1));
        Grid.RemoveWall(Mathf.RoundToInt(tilePos.x), Mathf.RoundToInt(tilePos.z));
    }
}