using UnityEngine;

/// <summary>
/// get scale of canvas
/// </summary>
public class CanvasScale : MonoBehaviour
{
    public static float Scale { get; private set; }

    private void Awake()
    {
        Scale = transform.localScale.x;
    }

    private void Update()
    {
        Scale = transform.localScale.x;
    }
}
