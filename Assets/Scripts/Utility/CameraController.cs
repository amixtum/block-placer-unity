using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float rotateSpeed = 1f;
    public float zoomSpeed = 1f;

    public KeyCode[] rotateLeftKeys = {KeyCode.Q};
    public KeyCode[] rotateRightKeys = {KeyCode.E};

    public KeyCode[] moveForwardKeys = {KeyCode.W, KeyCode.UpArrow};
    public KeyCode[] moveBackKeys = {KeyCode.S, KeyCode.DownArrow};
    public KeyCode[] moveLeftKeys = {KeyCode.A, KeyCode.LeftArrow};
    public KeyCode[] moveRightKeys = {KeyCode.D, KeyCode.RightArrow};

    public KeyCode[] zoomInKeys = {KeyCode.Equals, KeyCode.Plus};
    public KeyCode[] zoomOutKeys = {KeyCode.Minus, KeyCode.Underscore};

    private Dictionary<CameraEvent, UnityAction> actionMap = new Dictionary<CameraEvent, UnityAction>();

    private Dictionary<KeyCode, CameraEvent> keyMap = new Dictionary<KeyCode, CameraEvent>();

    void Start()
    {
        MapActions();
        MapKeys();
    }

    void Update()
    {
        ExecuteKeyEvents();
    }

    private void ExecuteKeyEvents()
    {
        foreach (KeyCode key in keyMap.Keys)
        {
            if (Input.GetKey(key))
            {
                if (keyMap.ContainsKey(key))
                {
                    CameraEvent e = keyMap[key];

                    if (actionMap.ContainsKey(e))
                    {
                        actionMap[e].Invoke();
                    }
                }
            }
        }
    }

    private void MapKeys()
    {
        foreach (KeyCode key in rotateLeftKeys)
        {
            keyMap.Add(key, CameraEvent.RotateLeft);
        }
        foreach (KeyCode key in rotateRightKeys)
        {
            keyMap.Add(key, CameraEvent.RotateRight);
        }

        foreach (KeyCode key in moveForwardKeys)
        {
            keyMap.Add(key, CameraEvent.MoveForward);
        }
        foreach (KeyCode key in moveBackKeys)
        {
            keyMap.Add(key, CameraEvent.MoveBack);
        }
        foreach (KeyCode key in moveLeftKeys)
        {
            keyMap.Add(key, CameraEvent.MoveLeft);
        }
        foreach (KeyCode key in moveRightKeys)
        {
            keyMap.Add(key, CameraEvent.MoveRight);
        }

        foreach (KeyCode key in zoomInKeys)
        {
            keyMap.Add(key, CameraEvent.ZoomIn);
        }
        foreach (KeyCode key in zoomOutKeys)
        {
            keyMap.Add(key, CameraEvent.ZoomOut);
        }
    }

    private void MapActions()
    {
        actionMap.Add(CameraEvent.MoveBack, GetMoveBackAction());
        actionMap.Add(CameraEvent.MoveForward, GetMoveForwardAction());
        actionMap.Add(CameraEvent.MoveLeft, GetMoveLeftAction());
        actionMap.Add(CameraEvent.MoveRight, GetMoveRightAction());

        actionMap.Add(CameraEvent.RotateLeft, GetRotateLeftAction());
        actionMap.Add(CameraEvent.RotateRight, GetRotateRightAction());

        actionMap.Add(CameraEvent.ZoomIn, GetZoomInAction());
        actionMap.Add(CameraEvent.ZoomOut, GetZoomOutAction());
    }

    private UnityAction GetRotateLeftAction()
    {
        UnityAction rotateLeft = new UnityAction(() =>
        {
            this.transform.Rotate(-Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        });

        return rotateLeft;
    }

    private UnityAction GetRotateRightAction()
    {
        UnityAction rotateRight = new UnityAction(() =>
        {
            this.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        });

        return rotateRight;
    }

    private UnityAction GetMoveForwardAction()
    {
        UnityAction moveForward = new UnityAction(() =>
        {
            Vector3 axis = (new Vector3(this.transform.forward.x, 0, this.transform.forward.z)).normalized;

            this.transform.Translate(axis * Time.deltaTime * moveSpeed, Space.World);
        });

        return moveForward;
    }

    private UnityAction GetMoveBackAction()
    {
        UnityAction moveBack = new UnityAction(() =>
        {
            Vector3 axis = -(new Vector3(this.transform.forward.x, 0, this.transform.forward.z)).normalized;

            this.transform.Translate(axis * Time.deltaTime * moveSpeed, Space.World);
        });

        return moveBack;
    }

    private UnityAction GetMoveLeftAction()
    {
        UnityAction moveLeft = new UnityAction(() =>
        {
            Vector3 axis = -(new Vector3(this.transform.right.x, 0, this.transform.right.z)).normalized;

            this.transform.Translate(axis * Time.deltaTime * moveSpeed, Space.World);
        });

        return moveLeft;
    }

    private UnityAction GetMoveRightAction()
    {
        UnityAction moveRight = new UnityAction(() =>
        {
            Vector3 axis = (new Vector3(this.transform.right.x, 0, this.transform.right.z)).normalized;

            this.transform.Translate(axis * Time.deltaTime * moveSpeed, Space.World);
        });

        return moveRight;
    }

    private UnityAction GetZoomInAction()
    {
        UnityAction zoomIn = new UnityAction(() =>
        {
            this.transform.Translate(this.transform.forward * Time.deltaTime * zoomSpeed, Space.World);
        });

        return zoomIn;
    }

    private UnityAction GetZoomOutAction()
    {
        UnityAction zoomOut = new UnityAction(() =>
        {
            this.transform.Translate(-this.transform.forward * Time.deltaTime * zoomSpeed, Space.World);
        });

        return zoomOut;
    }
}

public enum CameraEvent
{
    MoveLeft,
    MoveRight,
    MoveForward,
    MoveBack,
    RotateLeft,
    RotateRight,
    ZoomIn,
    ZoomOut
}