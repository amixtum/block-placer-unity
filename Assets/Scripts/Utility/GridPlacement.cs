using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridPlacement : MonoBehaviour
{
    public Vector3 gridOrigin = Vector3.zero;

    public Vector3 cellSize = new Vector3(1, 1, 1);

    public PivotPoint pivot = PivotPoint.Center;

    public RTSPrefab[] prefabs;

    public Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();

    void Start()
    {
        foreach (RTSPrefab prefab in prefabs)
        {
            prefabMap.Add(prefab.Name, prefab.Prefab);
        }
    }

    public Vector3 DirectionTo3DGridPosition(Vector3 observer, Vector3 direction, float yPlane)
    {
        direction = direction.normalized;

        Matrix4x4 mat = new Matrix4x4();

        float multiple = (yPlane - observer.y) / direction.y;

        mat.SetTRS(direction * multiple, Quaternion.identity, Vector3.one);

        Vector3 newPoint = mat.MultiplyPoint(observer);

        return GetNearest3DGridPoint(newPoint);
    }

    public Vector3 DirectionTo2DGridPosition(Vector3 observer, Vector3 direction)
    {
        direction = direction.normalized;

        Matrix4x4 mat = new Matrix4x4();

        float multiple = (gridOrigin.y - observer.y) / direction.y;

        mat.SetTRS(direction * multiple, Quaternion.identity, Vector3.one);

        Vector3 newPoint = mat.MultiplyPoint(observer);

        return GetNearest2DGridPoint(newPoint);
    }

    public GameObject SpawnAt2DGridPoint(Vector3 point, string name)
    {
        return Instantiate(prefabMap[name], GetNearest2DGridPoint(point), Quaternion.identity) as GameObject;
    }

    public GameObject SpawnAt2DGridPoint(Vector3 point, int index)
    {
        return Instantiate(prefabs[index].Prefab, GetNearest2DGridPoint(point), Quaternion.identity) as GameObject;
    }

    public GameObject SpawnAt3DGridPoint(Vector3 point, string name)
    {
        return Instantiate(prefabMap[name], GetNearest3DGridPoint(point), Quaternion.identity) as GameObject;
    }

    public GameObject SpawnAt3DGridPoint(Vector3 point, int index)
    {
        return Instantiate(prefabs[index].Prefab, GetNearest3DGridPoint(point), Quaternion.identity) as GameObject;
    }

    public GameObject SpawnAt2DGridPoint(Vector3 point, string name, Quaternion rotation)
    {
        return Instantiate(prefabMap[name], GetNearest2DGridPoint(point), rotation) as GameObject;
    }

    public GameObject SpawnAt2DGridPoint(Vector3 point, int index, Quaternion rotation)
    {
        return Instantiate(prefabs[index].Prefab, GetNearest2DGridPoint(point), rotation) as GameObject;
    }

    public GameObject SpawnAt3DGridPoint(Vector3 point, string name, Quaternion rotation)
    {
        return Instantiate(prefabMap[name], GetNearest3DGridPoint(point), rotation) as GameObject;
    }

    public GameObject SpawnAt3DGridPoint(Vector3 point, int index, Quaternion rotation)
    {
        return Instantiate(prefabs[index].Prefab, GetNearest3DGridPoint(point), rotation) as GameObject;
    }

    public GameObject SpawnAtPoint(Vector3 point, string name)
    {
        return Instantiate(prefabMap[name], point, Quaternion.identity) as GameObject;
    }

    public GameObject SpawnAtPoint(Vector3 point, int index)
    {
        return Instantiate(prefabs[index].Prefab, point, Quaternion.identity) as GameObject;
    }

    public GameObject SpawnAtPoint(Vector3 point, string name, Quaternion rotation)
    {
        return Instantiate(prefabMap[name], point, rotation) as GameObject;
    }

    public GameObject SpawnAtPoint(Vector3 point, int index, Quaternion rotation)
    {
        return Instantiate(prefabs[index].Prefab, point, rotation) as GameObject;
    }

    public Vector3 GetNearest3DGridPoint(Vector3 point)
    {
        Vector3 pivotMods = GetPivotModifiers();

        float x = GetNearestLowerMultiple(cellSize.x, point.x, pivotMods.x);
        float y = GetNearestLowerMultiple(cellSize.y, point.y, pivotMods.y);
        float z = GetNearestLowerMultiple(cellSize.z, point.z, pivotMods.z);

        return new Vector3(x, y, z);
    }

    public Vector3 GetNearest2DGridPoint(Vector3 point)
    {
        Vector3 pivotMods = GetPivotModifiers();

        float x = GetNearestLowerMultiple(cellSize.x, point.x, pivotMods.x);
        float y = gridOrigin.y;
        float z = GetNearestLowerMultiple(cellSize.z, point.z, pivotMods.z);

        return new Vector3(x, y, z);
    }

    public float GetNearestLowerMultiple(float partition, float input, float pivotMod)
    {
        int multipleOfPartition;

        multipleOfPartition = Mathf.RoundToInt(input / partition);

        return (partition * multipleOfPartition);
    }

    public Vector3 GetPivotModifiers()
    {
        switch (pivot)
        {
            case PivotPoint.BottomLeft:
                return new Vector3(0, 0, 0);
            case PivotPoint.BottomRight:
                return new Vector3(cellSize.x, 0, 0);
            case PivotPoint.Center:
                return new Vector3(cellSize.x / 2, cellSize.y / 2, cellSize.z / 2);
            case PivotPoint.TopLeft:
                return new Vector3(0, 0, cellSize.z);
            case PivotPoint.TopRight:
                return new Vector3(cellSize.x, 0, cellSize.z);
            default:
                return new Vector3(cellSize.x / 2, cellSize.y / 2, cellSize.z / 2);
        }
    }
}

[System.Serializable]
public struct RTSPrefab
{

    public GameObject Prefab;
    public Vector2 Dimensions;
    public string Name;
}

public enum PivotPoint
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center
}
