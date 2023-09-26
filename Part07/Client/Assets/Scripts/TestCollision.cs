using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestCollision : MonoBehaviour
{
    public Tilemap _tilemap;
    public TileBase _tile;

    // Start is called before the first frame update
    void Start()
    {
        _tilemap.SetTile(new Vector3Int(0, 0, 0), _tile); // test for dynamic tile.
    }

    // Update is called once per frame
    void Update()
    {
        var blocked = new List<Vector3Int>();

        foreach (var pos in _tilemap.cellBounds.allPositionsWithin)
        {
             var tile = _tilemap.GetTile(pos);
             if (tile != null)
             {
                 blocked.Add(pos);
             }
        }
    }
}
