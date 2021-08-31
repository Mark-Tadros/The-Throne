// Holds all Pathfinding variables and instantiates the AI after the PCG is finished.
using System.Collections.Generic;
using UnityEngine;
using AlanZucconi.AI.PF;

public class GridManager : MonoBehaviour
{
    Grid2D Grid;
    public PCG PCGScript;    
    public void Initialise(GameManager GameManagerScript) { Grid = new Grid2D(new Vector2Int(51, 101)); PCGScript.Initialise(GameManagerScript); }
    // Gets called when the Grid is updated or changed.
    public void SetWall(int x, int y) { Grid.SetWall(new Vector2Int(x, y)); }
    public void RemoveWall(int x, int y) { Grid.RemoveWall(new Vector2Int(x, y)); }
    public bool CheckWall(int x, int y)
    {
        if (x < 0 || x > 50 || y < 0 || y > 100) return false;
        else return Grid.IsWall(new Vector2Int(x, y));
    }
    // Calculates and returns the most efficient pathway for the AI.
    public List<Vector2Int> ReturnPath(Vector2Int Start, Vector2Int End)
    {
        List<Vector2Int> Path = Grid.BreadthFirstSearch(Start, End);
        if (Path == null) return null;
        else return Path;
    }
}