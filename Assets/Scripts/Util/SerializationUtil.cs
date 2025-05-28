using System.IO;
using UnityEngine;

public class SerializationUtil
{
    public enum Paths
    {
        NeuronNames,
        NeuronPositions,
        NeuronTypes,

        MatlabACh_weights,
        Matlabelectrical_weights,
        MatlabGABA_weights,
        MatlabGlu_weights,

        A_mon,
        A_np,
    }

    public static StringReader GetStringReader(Paths path)
    {
        return new StringReader(GetString(path));
    }

    public static string GetString(Paths path)
    {
        string[] pathString = GetFilePath(path);

        return Resources.Load<TextAsset>(Path.Combine(pathString[0], pathString[1])).text;
    }

    private static string[] GetFilePath(Paths path)
    {
        if (path == Paths.NeuronNames)
            return GetPathsScriptableObject().NeuronNames;
        else if (path == Paths.NeuronPositions)
            return GetPathsScriptableObject().NeuronPositions;
        else if (path == Paths.NeuronTypes)
            return GetPathsScriptableObject().NeuronTypes;

        else if (path == Paths.MatlabACh_weights)
            return GetPathsScriptableObject().MatlabACh_weights;
        else if (path == Paths.Matlabelectrical_weights)
            return GetPathsScriptableObject().Matlabelectrical_weights;
        else if (path == Paths.MatlabGABA_weights)
            return GetPathsScriptableObject().MatlabGABA_weights;
        else if (path == Paths.MatlabGlu_weights)
            return GetPathsScriptableObject().MatlabGlu_weights;

        else if (path == Paths.A_mon)
            return GetPathsScriptableObject().A_mon;
        else if (path == Paths.A_np)
            return GetPathsScriptableObject().A_np;

        return null;
    }

    private static PathsScriptableObject GetPathsScriptableObject()
    {
        return ScriptableObjectUtil.WindowsPathsScriptableObject;
    }

}
