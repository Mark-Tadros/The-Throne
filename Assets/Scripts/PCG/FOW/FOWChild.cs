// Gets called by FOWTrigger to print on the Asset.
using UnityEngine;
using UnityEngine.Tilemaps;

public class FOWChild : MonoBehaviour
{
    public Tile FOWTile;
    public void Trigger(Tilemap FOWTilemap, GameManager gameManagerScript)
    {
        // If that tile is already empty then break out of the loop.
        if (FOWTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(transform.position.x - 3), Mathf.RoundToInt(transform.position.z - 1), 0)) == null) return;
        // Else if that slot has dotted Tile don't draw the new tile.
        else if (FOWTile != null && FOWTile.name == "TinyRTSEnvironment_1" 
            && FOWTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(transform.position.x - 3), Mathf.RoundToInt(transform.position.z - 1), 0)).name == "TinyRTSEnvironment_2") return;

        gameManagerScript.saveData.FOWX.Add(Mathf.RoundToInt(transform.position.x - 3)); gameManagerScript.saveData.FOWY.Add(Mathf.RoundToInt(transform.position.z - 1));
        // Draw the empty or current tile.
        if (FOWTile == null)
        {
            gameManagerScript.saveData.FOW.Add(-1);
            FOWTilemap.SetTile(new Vector3Int(Mathf.RoundToInt(transform.position.x - 3), Mathf.RoundToInt(transform.position.z - 1), 0), null);
        }
        else
        {
            if (FOWTile.name == "TinyRTSEnvironment_1") gameManagerScript.saveData.FOW.Add(1);
            else gameManagerScript.saveData.FOW.Add(0);
            FOWTilemap.SetTile(new Vector3Int(Mathf.RoundToInt(transform.position.x - 3), Mathf.RoundToInt(transform.position.z - 1), 0), FOWTile);
        }
    }
}