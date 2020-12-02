using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CubicCoordinate = System.ValueTuple<int, int, int>;


public class GraticlePopulator
{
    [MenuItem("HexMap/Populate Graticule")]
    public static void Create()
    {
        foreach (var graticule in Object.FindObjectsOfType<Map.Graticule>())
        {
            DestroyChildren(graticule);
            ReallocateGraticuleStorage(graticule);
            CreateCells(graticule);
        }
    }

    public static void ReallocateGraticuleStorage(Map.Graticule graticule)
    {
        graticule.InitStorage();
    }

    public static void DestroyChildren(Map.Graticule graticule)
    {
        var transform = graticule.transform;
        do  // weirdly just iterating over the children does not ensure the complete deletion
        {
            foreach (Transform child in transform)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        } while (transform.childCount > 0);
    }

    public static void CreateCells(Map.Graticule graticule)
    {
        graticule.Apply((CubicCoordinate loc) => graticule[loc] = CreateCell(loc, graticule));
    }


    public static Map.Cell CreateCell(CubicCoordinate coord, Map.Graticule parent)
    {
        (int r, int s, int t) c = coord;
        GameObject obj = GameObject.Instantiate(original: Map.Cell.BaseModel.Value, parent: parent.transform);
        obj.name = "HexCell" + coord;
        Map.Cell cell = obj.AddComponentWithInit<Map.Cell>(u => u.CoordVector = new Vector3Int(c.r, c.s, c.t));
        MeshCollider collider = obj.AddComponent<MeshCollider>();
        collider.sharedMesh = null; // apparently this is required by unity.
        collider.sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
        return cell;
    }
}
