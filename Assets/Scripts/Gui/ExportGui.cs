using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// gui to export
/// </summary>
public class ExportGui : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI output;
    [SerializeField] protected GameObject exportBlock;

    private List<string> outputTexts = new();

    private enum ExportTypes
    {
        None,
        CalciumConcentration,
        MembranePotential
    }

    private void Awake()
    {
        output.text = string.Empty;
        exportBlock.SetActive(false);
    }

    public void CalciumConcentration()        //gui
    {
        StartCoroutine(ShowExportDialogIEnumerator(ExportTypes.CalciumConcentration));
    }

    public void MembranePotential()        //gui
    {
        StartCoroutine(ShowExportDialogIEnumerator(ExportTypes.MembranePotential));
    }

    private IEnumerator ShowExportDialogIEnumerator(ExportTypes exportType)
    {
        TimingManager.Instance.IsPaused = true;

        exportBlock.SetActive(true);
        output.text = string.Empty;
        outputTexts.Clear();

        AddOutput("Export output:");

        if (FileBrowser.CheckPermission() != FileBrowser.Permission.Granted)
            AddOutput("- Permission failure: " + FileBrowser.CheckPermission() + ", abort export");

        FileBrowser.SetFilters(false, new FileBrowser.Filter("", ".csv"));
        FileBrowser.ShowFileOverwriteDialog = true;

        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, null, exportType.ToString(), "Export as", "Export");

        if (FileBrowser.Success)
        {
            AddOutput("- Selected folder: " + FileBrowser.Result[0].Replace("\\", "/"));
            AddOutput("- Export started, this may take some time");

            AddOutput("    Please wait...");

            yield return null;

            System.Diagnostics.Stopwatch stopwatch = new();
            stopwatch.Start();

            RunMe runMe = new();

            int length = MatlabUtil.Length(runMe.t);

            for (int k = 0; k < length; k++)
            {
                runMe.RunOneLoop(k);
                ReplaceLastText("    " + (k + 1) + " / " + length);

                yield return null;
            }

            AddOutput("- Simulation completed, writing to file");
            AddOutput("    Please wait...");

            try
            {
                string path = FileBrowser.Result[0];

                if (exportType == ExportTypes.CalciumConcentration)
                    MatlabUtil.WriteMatrix(path, runMe.eta_logging);
                else if (exportType == ExportTypes.MembranePotential)
                    MatlabUtil.WriteMatrix(path, runMe.u_logging);

                ReplaceLastText("- Export completed successfully (" + (int)stopwatch.Elapsed.TotalSeconds + "s)");
            }
            catch
            {
                ReplaceLastText("- Export failed");
            }
        }
        else
        {
            output.text += "- No folder selected, abort export\n";
        }

        exportBlock.SetActive(false);
    }

    private void AddOutput(string text)
    {
        outputTexts.Add(text);
        output.text += text + "\n";
    }

    private void ReplaceLastText(string text)
    {
        outputTexts.RemoveAt(outputTexts.Count - 1);
        outputTexts.Add(text);

        output.text = string.Empty;

        for (int i = 0; i < outputTexts.Count; i++)
        {
            output.text += outputTexts[i] + "\n";
        }
    }

}
