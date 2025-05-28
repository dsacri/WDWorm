using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// controlls camera
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] protected float scrollSpeed;
    [SerializeField] protected float rotationSpeed;

    /// <summary>
    /// singleton
    /// </summary>
    public static CameraController Instance { get; private set; }

    /// <summary>
    /// mouse position in world of last frame (for movement)
    /// </summary>
    private Vector3 dragMoveLastPosition;
    /// <summary>
    /// is user currently moving (with dragging)
    /// </summary>
    private bool isDraggingMove;
    /// <summary>
    /// mouse position in world of last frame (for rotation)
    /// </summary>
    private Vector3 dragRotateLastPosition;
    /// <summary>
    /// is user currently rotating (with dragging)
    /// </summary>
    private bool isDraggingRotate;
    private Vector3 parentStartPosition;
    private Quaternion parentStartRotation;
    private float startZoom;

    private void Awake()
    {
        Instance = this;
        startZoom = transform.localPosition.z;
    }

    private void Start()
    {
        parentStartPosition = transform.parent.position;
        parentStartRotation = transform.parent.rotation;
    }

    private void Update()
    {
        Move(InputUtil.InputDown2, InputUtil.InputActive2, InputUtil.InputUp2, InputUtil.GetMousePosition, false, InputUtil.OverGui);
        Rotation(InputUtil.InputDown, InputUtil.InputActive, InputUtil.InputUp, InputUtil.GetMousePosition, true, InputUtil.OverGui);

        if (TimingManager.Instance.IsPlayModeActive && !EventSystem.current.IsPointerOverGameObject())
            Zoom(InputUtil.Zoom);
    }
    
    /// <summary>
    /// zoom camera
    /// </summary>
    private void Zoom(float value)
    {
        if (value != 0)
        {
            float zoomValue = value * -transform.localPosition.z * scrollSpeed;

            Vector3 newPosition = transform.localPosition + zoomValue * Vector3.forward;

            if (zoomValue <= 0 || zoomValue > 0 && newPosition.z <= -0.1f)
                transform.localPosition = newPosition;
        }
    }

    /// <summary>
    /// move camera
    /// </summary>
    private void Move(bool inputDown, bool inputActive, bool inputUp, InputUtil.Action<bool> mousePosition, bool screenPosition, bool overGui)
    {
        //input state (start, update, end)
        if (inputDown)
        {
            if (!overGui)
            {
                isDraggingMove = true;
                dragMoveLastPosition = mousePosition.Invoke(screenPosition);
            }
        }
        else if (inputActive && isDraggingMove)
        {
            Vector3 moveDelta = mousePosition.Invoke(screenPosition) - dragMoveLastPosition;
            moveDelta.y = 0;
            transform.parent.position -= moveDelta;

            dragMoveLastPosition = mousePosition.Invoke(screenPosition);
        }
        else if (inputUp)
        {
            isDraggingMove = false;
        }
    }

    /// <summary>
    /// rotate camera
    /// </summary>
    private void Rotation(bool inputDown, bool inputActive, bool inputUp, InputUtil.Action<bool> mousePosition, bool screenPosition, bool overGui)
    {
        //input state (start, update, end)
        if (inputDown)
        {
            if (!overGui)
            {
                isDraggingRotate = true;
                dragRotateLastPosition = mousePosition.Invoke(screenPosition);
            }
        }
        else if (inputActive && isDraggingRotate)
        {
            Vector3 moveDelta = mousePosition.Invoke(screenPosition) - dragRotateLastPosition;
            moveDelta = new(moveDelta.x / Screen.width, moveDelta.y / Screen.height, 0);

            float x = transform.parent.rotation.eulerAngles.x - moveDelta.y * rotationSpeed;
            float y = transform.parent.rotation.eulerAngles.y + moveDelta.x * rotationSpeed;

            if (x > 90 && x < 180)
                x = 90;
            if (x < 10 || x > 270)
                x = 10;

            transform.parent.rotation = Quaternion.Euler(new Vector3(x, y, 0));
            
            dragRotateLastPosition = mousePosition.Invoke(screenPosition);

        }
        else if (inputUp)
        {
            isDraggingRotate = false;
        }
    }
    
    /// <summary>
    /// reset camera to start position
    /// </summary>
    public void ResetCamera()
    {
        transform.parent.SetPositionAndRotation(parentStartPosition, parentStartRotation);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, startZoom);
    }
}