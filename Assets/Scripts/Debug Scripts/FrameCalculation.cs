using TMPro;
using UnityEngine;

/// <summary>
/// calculate framerate
/// </summary>
public class FrameCalculation : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI frames;

    private static string lastFps;
    private static float time;
    private static float frameCount;
    /// <summary>
    /// every x times per second
    /// </summary>
    private const float refreshRate = 2f;

    private void Update()
    {
        time += Time.deltaTime;
        frameCount++;

        if (time > 1 / refreshRate)
        {
            lastFps = (frameCount * refreshRate).ToString() + " fps";
            time = 0;
            frameCount = 0;
        }

        frames.text = lastFps;
    }

}
