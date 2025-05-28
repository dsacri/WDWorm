using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// controller of all matlab menus
/// </summary>
public class MatlabSettings : MonoBehaviour
{
    [SerializeField] private GameObject content;

    /// <summary>
    /// singleton
    /// </summary>
    public static MatlabSettings Instance { get; private set; }
    public List<MatlabMenu> MatlabMenus = new();

    private void Awake()
    {
        Instance = this;

        StartCoroutine(HideMenues());
    }

    private IEnumerator HideMenues()
    {
        yield return null;

        foreach (MatlabMenu matlabMenu in MatlabMenus)
        {
            if (matlabMenu.OpenedAtStart)
            {
                matlabMenu.OpenMenu();
            }
            else
            {
                matlabMenu.CloseMenu();
            }
        }

        content.SetActive(false);
    }

    public void Open()      //gui
    {
        content.SetActive(true);
    }

    public void Close()     //gui
    {
        if (RunMe.Instance != null)
            RunMe.Instance.ApplySettings();

        content.SetActive(false);
    }

    /// <summary>
    /// matlab menu registration
    /// </summary>
    public void AddMatlabMenu(MatlabMenu matlabMenu)
    {
        MatlabMenus.Add(matlabMenu);
    }

    /// <summary>
    /// </summary>
    /// <param name="except">can be null</param>
    public void CloseAllMenues(MatlabMenu except)
    {
        foreach (MatlabMenu matlabMenu in MatlabMenus)
        {
            if (matlabMenu != except)
                matlabMenu.CloseMenu();
        }
    }

}
