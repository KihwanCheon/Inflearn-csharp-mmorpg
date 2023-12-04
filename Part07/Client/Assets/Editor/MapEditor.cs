using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine.Tilemaps;
#endif

public class MapEditor : MonoBehaviour
{
#if UNITY_EDITOR

    // short cut: % - Ctrl, # - Shift, & - Alt
    /*[MenuItem("Tools/GenerateMap %#g")]
    private static void HelloWorld()
    {
        if (EditorUtility.DisplayDialog("Hello World", "Create?", "Create", "Cancel"))
        {
            new GameObject("Hello World");
        }
    }*/

    [MenuItem("Tools/GenerateMap %#g")]
    private static void GenerateMap()
    {
        var goMaps = Resources.LoadAll<GameObject>("Prefabs/Map");
        foreach (var go in goMaps)
        {
            SaveFileFromMap(go);
        }
    }

    private static void SaveFileFromMap(GameObject go)
    {
        var tmBase = Util.FindChild<Tilemap>(go, "Tilemap_Base", true);
        var tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);
        if (tm == null)
        {
            Debug.LogWarning("not found Tilemap_Collision object");
            return;
        }

        // 1. size of map
        // 2. occupied
        using (var writer = File.CreateText($"Assets/Resources/Map/{go.name}.txt"))
        {
            int xMin = tmBase.cellBounds.xMin;
            int xMax = tmBase.cellBounds.xMax;
            int yMin = tmBase.cellBounds.yMin;
            int yMax = tmBase.cellBounds.yMax;

            writer.WriteLine(xMin);
            writer.WriteLine(xMax);
            writer.WriteLine(yMin);
            writer.WriteLine(yMax);

            // left(x < 0) top(y > 0) to right(x > 0) bottom(y < 0)
            for (int y = yMax; y >= yMin; --y)
            {
                for (int x = xMin; x <= xMax; ++x)
                {
                    var tile = tm.GetTile(new Vector3Int(x, y, 0));
                    writer.Write(tile != null ? 1 : 0);
                }

                writer.WriteLine();
            }
        }
    }

#endif
}
