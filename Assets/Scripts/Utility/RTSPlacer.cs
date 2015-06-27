using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RTSPlacer : MonoBehaviour
{
    public int gridWidth = 30;

    public int gridHeight = 30;

    private GridPlacement gridPlacement;

    private Dictionary<Vector2, GameObject> discreteGrid = new Dictionary<Vector2, GameObject>();

    private Dictionary<string, Vector2> nameDimensionMap = new Dictionary<string, Vector2>();

    void Start()
    {
        gridPlacement = this.GetComponent<GridPlacement>();

        foreach (RTSPrefab rtsP in gridPlacement.prefabs)
        {
            nameDimensionMap.Add(rtsP.Name, rtsP.Dimensions);
        }

        for (int i = 0; i < gridWidth; ++i)
        {
            for (int k = 0; k < gridHeight; ++k)
            {
                discreteGrid.Add(new Vector2(i, k), null);
            }
        }
    }

    public bool PlaceFromMouseDirection(string objName)
    {
        List<Vector2> cellsOccupied = new List<Vector2>();

        Vector3 continuousPosition = GetPointOnPlaneFromMouseDirection();

        Vector2 discretePosition = GetDiscretePositionFromPoint(continuousPosition);

        Vector2 dimensions = nameDimensionMap[objName];

        for (int i = (int)(discretePosition.x - dimensions.x); 
                 i < discretePosition.x + dimensions.x; ++i)
        {
            for (int k = (int)(discretePosition.y - dimensions.y); 
                     k < discretePosition.y + dimensions.y; ++k)
            {
                Vector2 occupiedCell = new Vector2(i, k);

                cellsOccupied.Add(occupiedCell);
            }
        }

        foreach (Vector2 v in cellsOccupied)
        {
            if (discreteGrid.ContainsKey(v))
            {
                if (discreteGrid[v] != null)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        GameObject toPlace = gridPlacement.SpawnAtPoint(continuousPosition, objName);

        foreach (Vector2 v in cellsOccupied)
        {
            if (discreteGrid.ContainsKey(v))
            {
                discreteGrid[v] = toPlace;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private Vector2 GetDiscretePositionFromPoint(Vector3 point)
    {
        int x = (int)(point.x / gridPlacement.cellSize.x);
        int y = (int)(point.y / gridPlacement.cellSize.y);

        return new Vector2(x, y);
    }

    private Vector3 GetPointOnPlaneFromMouseDirection()
    {
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = GetMouseDirection();

        return gridPlacement.DirectionTo2DGridPosition(origin, direction);
    }

    private Vector3 GetMouseDirection()
    {
        Vector3 direction;

        Vector3 mousePosition = Input.mousePosition; 

        Ray rayFromCamera = Camera.main.ScreenPointToRay(mousePosition);

        direction = rayFromCamera.direction;

        return direction;
    }
}