using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class tilemapScript : MonoBehaviour
{
    private Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        var x0y0 = tilemap.GetCellCenterWorld(new Vector3Int(0, 0, 0));
        var x0y1 = tilemap.GetCellCenterWorld(new Vector3Int(0, 1, 0));
        var x0y2 = tilemap.GetCellCenterWorld(new Vector3Int(0, 2, 0));
        var x0y3 = tilemap.GetCellCenterWorld(new Vector3Int(0, 3, 0));

        var x = 1;
        ++x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
