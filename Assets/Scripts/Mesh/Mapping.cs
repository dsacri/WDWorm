using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// map a object to a spline (save relative position) and apply it later (set to relative position after spline has changed)
/// </summary>
public class Mapping : MonoBehaviour
{
    private bool isInitialized;
    private float positionOnSpline;
    private Vector3 offsetToSpline;

    /// <summary>
    /// saves the position and offset of each mapping relative to the spline
    /// </summary>
    public static void MapToSpline(SplineUtil.SingleSpline singleSpline)
    {
        foreach (Mapping mapping in singleSpline.Mappings)
        {
            if (mapping.isInitialized)
                continue;

            mapping.isInitialized = true;

            SplineUtility.GetNearestPoint(singleSpline.splineContainer.Splines[0], mapping.transform.position, out float3 p, out float t, Data.Instance.SplineResolution, Data.Instance.SplineIterations);
            Vector3 rotatedOffset = mapping.transform.position - (Vector3)p;
            Vector3 offset = (Vector3)Util.RotateVectorY(rotatedOffset, SplineUtil.GetRotationOnSpline(singleSpline, t).eulerAngles.y);

            mapping.positionOnSpline = t;
            mapping.offsetToSpline = offset;
        }
    }

    /// <summary>
    /// modifies the position of each mapping to fit the changed spline position
    /// </summary>
    public static void MoveToSplinePosition(SplineUtil.SingleSpline singleSpline)
    {
        foreach (Mapping mapping in singleSpline.Mappings)
        {
            if (!mapping.gameObject.activeSelf || !mapping.transform.parent.gameObject.activeSelf)
                continue;

            float rotation = -SplineUtil.GetRotationOnSpline(singleSpline, mapping.positionOnSpline).eulerAngles.y;
            Vector3 rotatedOffset = (Vector3)Util.RotateVectorY(mapping.offsetToSpline, rotation);
            mapping.transform.position = SplineUtil.GetPositionOnSpline(singleSpline, mapping.positionOnSpline) + rotatedOffset;
        }
    }

}
