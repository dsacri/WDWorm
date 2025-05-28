using UnityEngine;

/// <summary>
/// data container and handler
/// </summary>
public class Data : MonoBehaviour
{
    [SerializeField] protected int splineResolution;
    [SerializeField] protected int splineIterations;
    [SerializeField] protected SplineUtil.SingleSpline meshSpline;
    [SerializeField] protected SplineUtil.SingleSpline neuronSpline;
    [SerializeField] protected float minNeuronValue;
    [SerializeField] protected float maxNeuronValue;

    /// <summary>
    /// singleton
    /// </summary>
    public static Data Instance { get; private set; }

    /// <summary>
    /// spline that is inside of the celegans
    /// </summary>
    public SplineUtil.SingleSpline MeshSpline => meshSpline;
    /// <summary>
    /// spline to move neurons to mesh spline
    /// </summary>
    public SplineUtil.SingleSpline NeuronSpline => neuronSpline;
    /// <summary>
    /// spline quality (see SplineUtility)
    /// </summary>
    public int SplineResolution => splineResolution;
    public int SplineIterations => splineIterations;

    //values to convert to unity coordinate system and color space
    public float MatlabNeuronActivityAdd { get; private set; }
    public float MatlabNeuronActivityMultiply { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// convert min and max neuron value to values to calculate with
    /// </summary>
    public void SetTransformationParameters()
    {
        MatlabNeuronActivityMultiply = 1 / (maxNeuronValue - minNeuronValue);
        MatlabNeuronActivityAdd = -minNeuronValue * MatlabNeuronActivityMultiply;
    }

}
