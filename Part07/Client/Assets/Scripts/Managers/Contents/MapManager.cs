using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public Grid CurreGrid { get; private set; }

    public void LoadMap(int mapId)
    {
        DestroyMap();
        string mapName = "Map_" + mapId.ToString("000");
        var go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = "Map";

        var collision = Util.FindChild(go, "Tilemap_Collision", true);
        if (collision != null)
            collision.SetActive(false);

        CurreGrid = go.GetComponent<Grid>();
    }

    public void DestroyMap()
    {
        var map = GameObject.Find("Map");
        if (map != null)
        {
            GameObject.Destroy(map);
        }
    }
}
