using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineUtil
{

    [Serializable]
    public class SingleSpline
    {
        public SplineContainer splineContainer;

        public List<Mapping> Mappings { get; } = new();

        public void CreateSpline(float totalSplineLength, int bezierKnotCount)
        {
            float knotLength = totalSplineLength / (bezierKnotCount - 1);

            CreateBezierKnots(bezierKnotCount);

            for (int i = 0; i < bezierKnotCount; i++)
            {
                splineContainer.Splines[0].SetKnot(i, new(InitializationManager.Instance.SplineStartPosition + i * knotLength * Vector3.forward));
            }
        }

        private void CreateBezierKnots(int numberOfSplines)
        {
            Spline spline = new();

            for (int i = 0; i < numberOfSplines; i++)
                spline.Add(new BezierKnot());

            splineContainer.Splines = new List<Spline>() { spline };
        }
    }
    
    /// <summary>
    /// t == 0: start of spline
    /// t == 1: end of spline
    /// </summary>
    public static Vector3 GetPositionOnSpline(SingleSpline singleSpline, float t)    //t: 0-1, parallel
    {
        return singleSpline.splineContainer.EvaluatePosition(t);
    }

    /// <summary>
    /// t == 0: start of spline
    /// t == 1: end of spline
    /// </summary>
    public static Quaternion GetRotationOnSpline(SingleSpline singleSpline, float t)
    {
        Vector3 forward = singleSpline.splineContainer.EvaluateTangent(t);

        if (forward == Vector3.zero)
            return Quaternion.Euler(Vector3.zero);

        return Quaternion.LookRotation(forward, Vector3.up);
    }

    public static void CopySpline(SingleSpline fromSpline, SingleSpline toSpline)     //need same number of knots
    {
        for (int i = 0; i < fromSpline.splineContainer.Spline.Count; i++)
        {
            BezierKnot bezierKnot = fromSpline.splineContainer.Spline[i];
            toSpline.splineContainer.Spline[i] = bezierKnot;
        }
    }
}