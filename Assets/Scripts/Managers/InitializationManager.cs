using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// initialize objects and data from startup to loading finished state
/// </summary>
public class InitializationManager : MonoBehaviour
{
    /// <summary>
    /// singleton
    /// </summary>
    public static InitializationManager Instance { get; private set; }
    /// <summary>
    /// start position of splines
    /// </summary>
    public Vector3 SplineStartPosition { get; set; }
    /// <summary>
    /// preparations are finished
    /// </summary>
    public UnityEvent LoadingFinishedEvent { get; } = new();
    public bool LoadingFinished { get; private set; }
    /// <summary>
    /// RunMe is the translated matlab code of the celegans simulation
    /// </summary>
    public RunMe RunMe { get; private set; }
    /// <summary>
    /// data (settings) for matlab translated code
    /// </summary>
    public MatlabSerializedData MatlabSerializedData { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        //wait for all start methods to finish
        yield return null;
        
        //spline doesn't start at center
        SplineStartPosition = Data.Instance.MeshSpline.splineContainer.Splines[0][0].Position * new Vector3(0, 1, 1);

        //create a spline for neuron creation that is staight but as the same length and knots as the mesh spline
        Data.Instance.NeuronSpline.CreateSpline(
            Data.Instance.MeshSpline.splineContainer.Splines[0].GetLength(),
            Data.Instance.MeshSpline.splineContainer.Splines[0].Count
        );

        //create neurons and link them to the neuron spline
        NeuronManager.Instance.StartNeuronCreation(SplineStartPosition, Quaternion.Euler(Vector3.up * 90), Data.Instance.NeuronSpline);

        //set neuron spline to same values as mesh spline
        SplineUtil.CopySpline(Data.Instance.MeshSpline, Data.Instance.NeuronSpline);

        //create matlab translated class
        CreateNewRunMe();

        //set data
        SetFramerates();
        Data.Instance.SetTransformationParameters();
        TimingManager.Instance.MaxTime = RunMe.GetMaxTime();
        
        //run one frame to set neurons to new spline position
        TimingManager.Instance.NeedsSoftUpdate = true;
        TimingManager.Instance.PlayOneLoop();

        LoadingFinished = true;
        LoadingFinishedEvent.Invoke();
    }

    public void CreateNewRunMe()
    {
        RunMe = new RunMe();
    }

    private void SetFramerates()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = (int)(1 / RunMe.T);
        
        Debug.Log("Target Framerate: " + Application.targetFrameRate + " fps");
    }

}
