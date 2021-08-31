// Controls the FOWChild script to create a fog.
using UnityEngine;
using UnityEngine.Tilemaps;

public class FOWTrigger : MonoBehaviour
{
    public void Trigger(Tilemap FOWTilemap, GameManager gameManagerScript)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<FOWChild>().Trigger(FOWTilemap, gameManagerScript);
        }
        gameObject.SetActive(false);
    }
}