using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// simulation control menu
/// </summary>
public class ControlMenu : MonoBehaviour
{
    [SerializeField] protected GameObject content;
    /// <summary>
    /// show debug text
    /// </summary>
    [SerializeField] protected Toggle showCompleteDebug; 
    [SerializeField] protected Toggle showAllNeurons;
    [SerializeField] protected GameObject closeButton;
    [SerializeField] protected GameObject showDebug;

    /// <summary>
    /// deactivated childs (gui elements) during initialization
    /// </summary>
    private List<Transform> deactivatedChild = new();
    /// <summary>
    /// is menu opened?
    /// </summary>
    private bool isVisible;

    private void Awake()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Transform child = content.transform.GetChild(i);

            if (child.gameObject.activeSelf && child.gameObject != closeButton.gameObject && child.gameObject != showDebug)
            {
                deactivatedChild.Add(child);
                child.gameObject.SetActive(false);
            }
        }

        Hide();
    }

    private void Start()
    {
        NeuronManager.Instance.ShowAllNeuronsChangedEvent.AddListener(ShowAllNeuronsChangedListener);

        InitializationManager.Instance.LoadingFinishedEvent.AddListener(LoadingFinishedListener);
    }

    private void ShowAllNeuronsChangedListener()
    {
        showAllNeurons.isOn = NeuronManager.Instance.ShowAllNeurons;
    }

    private void LoadingFinishedListener()
    {
        foreach (Transform child in deactivatedChild)
        {
            if (child == null)
                continue;

            child.gameObject.SetActive(true);
        }
    }

    public void ResetCamera()       //gui
    {
        CameraController.Instance.ResetCamera();
    }

    public void ShowHide()      //gui
    {
        isVisible = !isVisible;

        if (isVisible)
            Show();
        else
            Hide();
    }

    /// <summary>
    /// show menu
    /// </summary>
    private void Show()
    {
        isVisible = true;
        content.SetActive(true);
    }

    /// <summary>
    /// hide menu
    /// </summary>
    private void Hide()
    {
        isVisible = false;
        content.SetActive(false);
    }

    public void ShowCompleteDebug()     //gui
    {
        GuiConsole.Show = showCompleteDebug.isOn;
    }

    public void ShowAllNeurons()      //gui
    {
        NeuronManager.Instance.ShowAllNeurons = showAllNeurons.isOn;
    }

    public void Quit()      //gui
    {
        Application.Quit();
    }

}
