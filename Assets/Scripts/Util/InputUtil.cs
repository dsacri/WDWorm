using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// all user inputs except colliders and gui
/// </summary>
public class InputUtil : MonoBehaviour
{
    /// <summary>
    /// left mouse button / touch down and wasn't down last frame
    /// </summary>
    public static bool InputDown { get; private set; }
    /// <summary>
    /// left mouse button / touch down and was down last frame
    /// </summary>
    public static bool InputActive { get; private set; }
    /// <summary>
    /// left mouse button / touch up and was down last frame
    /// </summary>
    public static bool InputUp { get; private set; }
    /// <summary>
    /// right mouse button down and wasn't down last frame
    /// </summary>
    public static bool InputDown2 { get; private set; }
    /// <summary>
    /// right mouse button down and was down last frame
    /// </summary>
    public static bool InputActive2 { get; private set; }
    /// <summary>
    /// right mouse button and was down last frame
    /// </summary>
    public static bool InputUp2 { get; private set; }
    /// <summary>
    /// zoom value from last frame to this frame
    /// </summary>
    public static float Zoom { get; private set; }
    /// <summary>
    /// is mouse / touch over gui
    /// </summary>
    public static bool OverGui 
    { 
        get 
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }

    public delegate Vector3 Action<T0>(T0 arg0);

    /// <summary>
    /// horizontal plane at y == 0
    /// </summary>
    private static Plane yZeroPlane = new(Vector3.up, 0);

    private void Update()
    {
        SetClick();
        SetZoom();
        SetClick2();
    }

    /// <summary>
    /// left click
    /// </summary>
    private void SetClick()
    {
        InputDown = Input.GetMouseButtonDown(0);
        InputActive = Input.GetMouseButton(0);
        InputUp = Input.GetMouseButtonUp(0);
    }

    /// <summary>
    /// right click
    /// </summary>
    private void SetClick2()
    {
        InputDown2 = Input.GetMouseButtonDown(1);
        InputActive2 = Input.GetMouseButton(1);
        InputUp2 = Input.GetMouseButtonUp(2);
    }

    /// <summary>
    /// zoom
    /// </summary>
    private void SetZoom()
    {
        Zoom = Input.mouseScrollDelta.y;
    }

    public static Vector3 GetMousePosition(bool screenPosition)
    {
        if (screenPosition)
        {
            return Input.mousePosition;
        }
        else
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            return GetYZeroPoint(mouseRay, 0) + Vector3.back * 0.1f;
        }
    }

    /// <summary>
    /// get point where a ray (e.g. mouse) hits y == 0
    /// </summary>
    private static Vector3 GetYZeroPoint(Ray ray, float yAdd)
    {
        yZeroPlane.SetNormalAndPosition(Vector3.up, Vector3.up * yAdd);

        if (yZeroPlane.Raycast(ray, out float dist))
        {
            return ray.GetPoint(dist);
        }

        float angle = Vector3.Angle(Vector3.down, ray.direction);
        float distance = Camera.main.transform.position.y / Mathf.Sin((90 - angle) * Mathf.Deg2Rad);

        return ray.GetPoint(distance);
    }
}
