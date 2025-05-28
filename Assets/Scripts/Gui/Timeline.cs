using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// control time
/// </summary>
public class Timeline : MonoBehaviour
{
    /// <summary>
    /// play pause button
    /// </summary>
    [SerializeField] protected Button playPause;
    /// <summary>
    /// images to switch play pause button to
    /// </summary>
    [SerializeField] protected GuiImages playPauseImages;
    /// <summary>
    /// frame counter
    /// </summary>
    [SerializeField] protected TextMeshProUGUI text;
    /// <summary>
    /// game object that is active when recording
    /// </summary>
    [SerializeField] protected GameObject recording;

    private void Awake()
    {
        recording.SetActive(true);
    }

    private void Start()
    {
        //show correct icon at start
        if (TimingManager.Instance.IsPaused)
            Pause();
        else
            Play();

        InitializationManager.Instance.LoadingFinishedEvent.AddListener(LoadingFinishedListener);
        TimingManager.Instance.PlayPauseChangedEvent.AddListener(PlayPauseChangedListener);
    }

    private void Update()
    {
        if (Application.targetFrameRate > 0)
        {
            text.text = Util.TimelineTime((TimingManager.Instance.Time + 1) / (float)Application.targetFrameRate) + " / " + Util.TimelineTime(TimingManager.Instance.MaxTime / (float)Application.targetFrameRate);
        }

        if (Input.GetKeyDown(KeyCode.Space) && TimingManager.Instance.IsPlayModeActive)
        {
            if (TimingManager.Instance.IsPaused)
                Play();
            else
                Pause();
        }
    }

    public void ResetTime()         //gui
    {
        TimingManager.Instance.ResetNextUpdate = true;
    }

    public void PlayPause()          //gui
    {
        if (TimingManager.Instance.IsPaused)
            Play();
        else 
            Pause();
    }

    private void Play()
    {
        TimingManager.Instance.IsPaused = false;
    }

    private void Pause()
    {
        TimingManager.Instance.IsPaused = true;
    }

    private void LoadingFinishedListener()
    {
        recording.SetActive(false);
    }

    private void PlayPauseChangedListener()
    {
        if (TimingManager.Instance.IsPaused)
        {
            playPause.image.sprite = playPauseImages.Sprite1;
        }
        else
        {
            playPause.image.sprite = playPauseImages.Sprite2;
        }
    }

}
