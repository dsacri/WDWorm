using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// records (renders) meshes (if not live data) and replays them
/// </summary>
public class TimingManager : MonoBehaviour
{
    /// <summary>
    /// singleton
    /// </summary>
    public static TimingManager Instance { get; private set; }
    /// <summary>
    /// play time
    /// </summary>
    public int Time { get => time; set { time = value; NeedsHardUpdate = true; } }
    /// <summary>
    /// max time of play
    /// </summary>
    public int MaxTime { get; set; }
    /// <summary>
    /// is play stopped?
    /// </summary>
    public bool IsPaused { get => isPaused; set { isPaused = value; PlayPauseChangedEvent.Invoke(); } }
    /// <summary>
    /// changed from play to pause or from pause to play
    /// </summary>
    public UnityEvent PlayPauseChangedEvent { get; } = new();
    public bool IsPlayModeActive { get; private set; } = false;
    /// <summary>
    /// refresh vertices to last calculation
    /// </summary>
    public bool NeedsSoftUpdate { get; set; }
    /// <summary>
    /// do new calculation and do soft update
    /// </summary>
    public bool NeedsHardUpdate { get; set; }
    public bool ResetNextUpdate { get; set; }

    private int time = 0;
    private bool isPaused = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializationManager.Instance.LoadingFinishedEvent.AddListener(LoadingFinishedListener);
    }

    private void Update()
    {
        if (IsPlayModeActive)
        {
            PlayOneLoop();
        }
    }

    private void LoadingFinishedListener()
    {
        Mapping.MoveToSplinePosition(Data.Instance.NeuronSpline);
        
        IsPlayModeActive = true;
        Time = 0;
    }

    /// <summary>
    /// one play loop
    /// </summary>
    public void PlayOneLoop()
    {
        if (!IsPaused)
        {
            Time++;
        }

        if (Time >= MaxTime)
        {
            Time = 0;
        }

        if (ResetNextUpdate)
        {
            Time = 0;
            ResetNextUpdate = false;
            InitializationManager.Instance.CreateNewRunMe();
            NeedsHardUpdate = true;
        }

        if (NeedsSoftUpdate || NeedsHardUpdate)
        {
            if (NeedsHardUpdate)
            {
                InitializationManager.Instance.RunMe.RunOneLoop(Time);
            }

            if (Time == MaxTime - 1)
            {
                InitializationManager.Instance.CreateNewRunMe();
            }

            NeuronManager.Instance.ManualUpdate();
        }

        NeedsSoftUpdate = false;
        NeedsHardUpdate = false;
    }

}
