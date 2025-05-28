using UnityEngine;

public class Window : MonoBehaviour
{
    /// <summary>
    /// min distance the window should have to the screen border
    /// </summary>
    [SerializeField] protected Vector2 borderDistance;
    [SerializeField] protected ButtonInteractionHelper openCloseButton;
    [SerializeField] protected GraphicInteractionHelper windowCloseButton;


    /// <summary>
    /// is user hovering over the window
    /// </summary>
    private bool isHovering;
    /// <summary>
    /// is user dragging the window
    /// </summary>
    private bool isDragging;
    /// <summary>
    /// mouse pointer offset at start of dragging
    /// </summary>
    private Vector3 draggingOffset;
    /// <summary>
    /// size of window
    /// </summary>
    private Vector2 size;
    /// <summary>
    /// size of window / 2
    /// </summary>
    private Vector2 sizeHalf;

    private void Awake()
    {
        CalculateSize();

        Close();
        openCloseButton.ButtonLeftPressedEvent.AddListener(ButtonLeftPressedListener);
    }

    private void Update()
    {
        CalculateSize();

        //drag start
        if (isHovering || isDragging)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                draggingOffset = transform.position - Input.mousePosition;
            }
        }

        Vector3 refreshedPosition = transform.position;

        //dragging
        if (isDragging)
        {
            refreshedPosition = Input.mousePosition + draggingOffset;

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }

        //correct window position
        transform.position = CorrectPosition(refreshedPosition);

    }

    private void ButtonLeftPressedListener(ButtonPressedEvent buttonPressedEvent)
    {
        OpenClose();
    }

    /// <summary>
    /// calculates window size
    /// </summary>
    private void CalculateSize()
    {
        size = GetComponent<RectTransform>().sizeDelta;
        size += borderDistance;
        size *= CanvasScale.Scale;
        sizeHalf = size / 2;
    }

    /// <summary>
    /// correct position that window doesn't leave screen
    /// </summary>
    private Vector3 CorrectPosition(Vector3 position)
    {
        float x = position.x;
        if (x - sizeHalf.x < 0)
            x = sizeHalf.x;
        else if (x + sizeHalf.x > Screen.width)
            x = Screen.width - sizeHalf.x;

        float y = position.y;
        if (y - sizeHalf.y < 0)
            y = sizeHalf.y;
        else if (y + sizeHalf.y > Screen.height)
            y = Screen.height - sizeHalf.y;

        return new Vector3(x, y, 0);
    }

    public void OpenClose()     //gui
    {
        if (gameObject.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void Open()
    {
        gameObject.SetActive(true);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    public void WindowClose()       //gui
    {
        openCloseButton.SetSelect(false);
        isDragging = false;
        windowCloseButton.OnPointerExit();
    }

    public void PointerEnter()      //gui
    {
        isHovering = true;
    }

    public void PointerExit()      //gui
    {
        isHovering = false;
    }

}
