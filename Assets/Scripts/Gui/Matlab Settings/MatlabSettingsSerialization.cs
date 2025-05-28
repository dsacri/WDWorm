using SimpleFileBrowser;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

/// <summary>
/// save / load matlab settings
/// </summary>
public class MatlabSettingsSerialization : MonoBehaviour
{
    [SerializeField] private TMP_Text filename;

    private const string extension = ".json";

    private void Awake()
    {
        filename.text = "";
    }

    public void Save()      //gui
    {
        StartCoroutine(SaveAsIEnumerator());
    }

    private IEnumerator SaveAsIEnumerator()
    {
        //check permissions
        if (FileBrowser.CheckPermission() != FileBrowser.Permission.Granted)
        {
            Debug.Log("Permission failure: " + FileBrowser.CheckPermission() + ", abort save");
            yield break;
        }

        //show save dialog
        FileBrowser.SetFilters(false, new FileBrowser.Filter("", extension));
        FileBrowser.ShowFileOverwriteDialog = true;

        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, "", "", "Save", "Ok");

        //save
        if (FileBrowser.Success)
        {
            try
            {
                SaveFinal(FileBrowser.Result[0]);    //Path, Name and Extension
            }
            catch (Exception e)
            {
                Debug.Log("Exception saving the file: " + e);
            }
        }
    }

    private void SaveFinal(string pathAndNameAndExtension)
    {
        InitializationManager.Instance.MatlabSerializedData = CreateMatlabSerializedData();

        JsonSerializationUtil.Serialize(InitializationManager.Instance.MatlabSerializedData, pathAndNameAndExtension);

        Debug.Log("Saved: " + pathAndNameAndExtension);
        filename.text = Util.GetFilename(pathAndNameAndExtension);
    }

    /// <summary>
    /// create data container and fill it with data from gui
    /// </summary>
    public static MatlabSerializedData CreateMatlabSerializedData()
    {
        MatlabSerializedData matlabSerializedData = new();

        foreach (MatlabMenu matlabMenu in MatlabSettings.Instance.MatlabMenus)
        {
            ISaveable saveable = matlabMenu.GetComponent<ISaveable>();

            if (saveable != null)
                saveable.Save(matlabSerializedData);
        }

        return matlabSerializedData;
    }

    public void Load()      //gui
    {
        StartCoroutine(LoadIEnumerator());
    }

    private IEnumerator LoadIEnumerator()
    {
        //check permissions
        if (FileBrowser.CheckPermission() != FileBrowser.Permission.Granted)
        {
            Debug.Log("Permission failure: " + FileBrowser.CheckPermission() + ", abort load");
            yield break;
        }

        //show load dialog
        FileBrowser.SetFilters(false, new FileBrowser.Filter("", extension));
        FileBrowser.ShowFileOverwriteDialog = true;

        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, "", "", "Load", "Select");

        //load
        if (FileBrowser.Success)
        {
            LoadFinal(FileBrowser.Result[0]);
        }
    }

    private void LoadFinal(string pathAndNameAndExtension)
    {
        InitializationManager.Instance.MatlabSerializedData = JsonSerializationUtil.Deserialize<MatlabSerializedData>(pathAndNameAndExtension);

        foreach (MatlabMenu matlabMenu in MatlabSettings.Instance.MatlabMenus)
        {
            ISaveable saveable = matlabMenu.GetComponent<ISaveable>();

            if (saveable != null)
                saveable.Load(InitializationManager.Instance.MatlabSerializedData);
        }

        Debug.Log("Loaded: " + pathAndNameAndExtension);
        filename.text = Util.GetFilename(pathAndNameAndExtension);
    }

}
