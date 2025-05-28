using UnityEngine;

/// <summary>
/// control window for settings
/// </summary>
public class MatlabMenu : MonoBehaviour
{
    [SerializeField] private ButtonInteractionHelper button;
    [SerializeField] private GameObject content;
    [SerializeField] private bool openedAtStart;

    public bool OpenedAtStart => openedAtStart;

    private MatlabSettings matlabSettings;
    private bool buttonListenerPaused;

    private void Start()
    {
        matlabSettings = GetComponentInParent<MatlabSettings>();
        matlabSettings.AddMatlabMenu(this);

        button.ButtonLeftPressedEvent.AddListener(ButtonPressedListener);
    }

    private void ButtonPressedListener(ButtonPressedEvent buttonPressedEvent)
    {
        if (buttonListenerPaused)
            return;

        matlabSettings.CloseAllMenues(this);
        content.SetActive(buttonPressedEvent.ButtonInteractionHelper.IsSelected);
    }

    public void CloseMenu()
    {
        buttonListenerPaused = true;
        button.SetSelect(false);
        content.SetActive(false);
        buttonListenerPaused = false;
    }

    public void OpenMenu()
    {
        buttonListenerPaused = true;
        button.SetSelect(true);
        content.SetActive(true);
        buttonListenerPaused = false;
    }

}
