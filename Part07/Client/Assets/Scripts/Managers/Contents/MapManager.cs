using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager
{
    public Grid CurreGrid { get; private set; }

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    private bool[,] _collision;

    public bool CanGo(Vector3Int cellPos)
    {
        var cellPosX = cellPos.x;
        if (cellPosX < MinX || cellPosX > MaxX)
            return false;

        var cellPosY = cellPos.y;
        if (cellPosY < MinY || cellPosY > MaxY)
            return false;

        int x = cellPosX - MinX;
        int y = MaxY - cellPosY;
        return !_collision[y, x];
    }

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
        // Collision
        var txt = Managers.Resource.Load<TextAsset>($"Map/{mapName}"); //not txt extend.(TextAsset)
        using (var reader = new StringReader(txt.text))
        {
            int.TryParse(reader.ReadLine(), out var minX); MinX = minX;
            int.TryParse(reader.ReadLine(), out var maxX); MaxX = maxX;
            int.TryParse(reader.ReadLine(), out var minY); MinY = minY;
            int.TryParse(reader.ReadLine(), out var maxY); MaxY = maxY;

            int xCount = MaxX - MinX + 1;
            int yCount = MaxY - MinY + 1;

            _collision = new bool[yCount, xCount];

            for (int y = 0; y < yCount; ++y)
            {
                string line = reader.ReadLine();
                if (line == null)
                    continue;

                for (int x = 0; x < xCount; ++x)
                {
                    _collision[y, x] = line[x] == '1';
                }
            }
        }
    }

    public void DestroyMap()
    {
        var map = GameObject.Find("Map");
        if (map != null)
        {
            Object.Destroy(map);
        }
    }
}
