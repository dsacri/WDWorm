using UnityEngine;

/// <summary>
/// get references to scriptable objects
/// </summary>
public class ScriptableObjectUtil : MonoBehaviour
{
    [SerializeField] private PathsScriptableObject windowsPathsScriptableObject;
    [SerializeField] protected ColorsScriptableObject colorsScriptableObject;

    public static PathsScriptableObject WindowsPathsScriptableObject { get { return instance.windowsPathsScriptableObject; } }
    public static ColorsScriptableObject ColorsScriptableObject { get { return instance.colorsScriptableObject; } }

    private static ScriptableObjectUtil instance;

    private void Awake()
    {
        instance = this;
    }

}
