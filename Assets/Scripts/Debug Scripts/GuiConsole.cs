using TMPro;
using UnityEngine;

/// <summary>
/// show debug output (at build)
/// </summary>
public class GuiConsole : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI text;
    [SerializeField] protected GameObject content;

    private static bool show;
    private static string myLog = "";
    private static GuiConsole instance;
    private string output;

    public static bool Show 
    { 
        get { return show; }
        set { show = value; instance.content.SetActive(show); }
    }

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    private void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        myLog = output + "\n" + myLog;
        if (myLog.Length > 5000)
        {
            myLog = myLog.Substring(0, 4000);
        }

        text.text = myLog;
    }

}
