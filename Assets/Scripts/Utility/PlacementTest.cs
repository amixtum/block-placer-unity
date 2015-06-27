using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlacementTest : MonoBehaviour
{
    public GameObject placementTracer;

    public float inputDelay = 0.1f;

    public PlaceMode placeMode = PlaceMode.Flat;

    public Bounds bounds;

    private bool continuousInput = true;

    private GridPlacement gridPlacement;

    private Queue<Vector3> placeQueue = new Queue<Vector3>();

    private List<GameObject> placedObjects = new List<GameObject>();

    private float timeSinceLastPlaced = 0f;
    private float timeSinceLastKeyRemoved = 0f;
    private float timeSinceLastMouseRemoved = 0f;

    // for testing
    private const string prefabName = "TestCube";

    void Start()
    {
        gridPlacement = this.GetComponent<GridPlacement>();
    }

    void Update()
    {
        HandleInput();
        TickTimers();
    }

    private void TickTimers()
    {
        timeSinceLastKeyRemoved += Time.deltaTime;
        timeSinceLastMouseRemoved += Time.deltaTime;
        timeSinceLastPlaced += Time.deltaTime;
    }

    private void HandleInput()
    {
        PositionTracer();

        HandleInputModes();

        EnqueuePlaceEvents();

        DequeuePlaceEvents();

        HandleRemovalInput();
    }

    private void HandleInputModes() 
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            placeMode = PlaceMode.Flat;
        }
        else
        {
            placeMode = PlaceMode.Vertical;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            continuousInput = true;
        }
        else
        {
            continuousInput = false;
        }
    }

    private void HandleRemovalInput()
    {
        if (continuousInput)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                RemoveMostRecentlyPlaced();
            }
            if (Input.GetMouseButton(1))
            {
                RemoveAtMousePosition();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                RemoveMostRecentlyPlaced();
            }
            if (Input.GetMouseButtonDown(1))
            {
                RemoveAtMousePosition();
            }
        }
    }

    private void PositionTracer()
    {
        Vector3 mouseDirection = MouseProjection();

        Vector3 origin = Camera.main.transform.position;

        Vector3 point = gridPlacement.DirectionTo2DGridPosition(origin, mouseDirection);

        //Debug.DrawLine(origin, point);

        if (!ExistsAtPoint(point))
        {
            placementTracer.transform.position = point;
        }
        else
        {
            while (ExistsAtPoint(point))
            {
                point.y += gridPlacement.cellSize.y;
            }

            placementTracer.transform.position = point;
        }
    }

    private void RemoveMostRecentlyPlaced()
    {
        if (timeSinceLastKeyRemoved >= inputDelay)
        {
            if (placedObjects.Count > 0)
            {
                Destroy(placedObjects[placedObjects.Count - 1]);

                placedObjects.RemoveAt(placedObjects.Count - 1);
            }

            timeSinceLastKeyRemoved = 0;
        }
    }

    private void RemoveAtMousePosition()
    {
        Vector3 tracerPosition = placementTracer.transform.position;

        if (timeSinceLastMouseRemoved >= inputDelay)
        {
            if (tracerPosition.y > gridPlacement.gridOrigin.y)
            {
                tracerPosition.y -= gridPlacement.cellSize.y;
            }

            foreach (GameObject obj in placedObjects)
            {
                if (obj.transform.position == tracerPosition)
                {
                    Destroy(obj);

                    placedObjects.Remove(obj);

                    return;
                }
            }

            timeSinceLastMouseRemoved = 0;
        }
    }

    private bool ExistsAtPoint(Vector3 point)
    {
        foreach (GameObject obj in placedObjects)
        {
            if (obj.transform.position == point)
            {
                return true;
            }
        }

        return false;
    } 

    private void DequeuePlaceEvents()
    {
        while (placeQueue.Count > 0)
        {
            Vector3 origin = Camera.main.transform.position;

            Vector3 point = gridPlacement.DirectionTo2DGridPosition(origin, placeQueue.Dequeue());

            if (point.x > bounds.xMin && point.x < bounds.xMax && 
                point.z > bounds.zMin && point.z < bounds.zMax)
            {
                if (!ExistsAtPoint(point))
                {
                    placedObjects.Add(gridPlacement.SpawnAt2DGridPoint(point, prefabName));
                }
                else
                {
                    if (placeMode == PlaceMode.Vertical)
                    {
                        while (ExistsAtPoint(point))
                        {
                            point.y += gridPlacement.cellSize.y;
                        }

                        placedObjects.Add(gridPlacement.SpawnAtPoint(point, prefabName));
                    }
                }
            }
        }
    }

    private void EnqueuePlaceEvents()
    {
        Vector3 mouseClickedDirection = MouseClickProjection();

        if (mouseClickedDirection != Vector3.zero)
        {
            placeQueue.Enqueue(mouseClickedDirection);
        }
    }

    private Vector3 MouseClickProjection()
    {
        Vector3 direction = Vector3.zero;

        if (timeSinceLastPlaced >= inputDelay)
        {
            if (continuousInput)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector3 mousePosition = Input.mousePosition;

                    Ray rayFromCamera = Camera.main.ScreenPointToRay(mousePosition);

                    direction = rayFromCamera.direction;

                    timeSinceLastPlaced = 0;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePosition = Input.mousePosition;

                    Ray rayFromCamera = Camera.main.ScreenPointToRay(mousePosition);

                    direction = rayFromCamera.direction;

                    timeSinceLastPlaced = 0;
                }
            }
        }

        return direction;
    }

    private Vector3 MouseProjection()
    {
        Vector3 direction;

        Vector3 mousePosition = Input.mousePosition;

        Ray rayFromCamera = Camera.main.ScreenPointToRay(mousePosition);

        direction = rayFromCamera.direction;

        return direction;
    }
}

[System.Serializable]
public struct Bounds
{
    public float xMax;
    public float xMin;
    public float zMax;
    public float zMin;
}

public enum PlaceMode
{
    Flat,
    Vertical
}
