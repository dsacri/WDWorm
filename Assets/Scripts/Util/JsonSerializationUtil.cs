using System.IO;
using UnityEngine;

public static class JsonSerializationUtil
{
    public static void Serialize(object item, string pathAndNameAndExtension)
    {
        File.WriteAllText(pathAndNameAndExtension, JsonUtility.ToJson(item, false));    //change to true to make json readable
    }

    public static T Deserialize<T>(string pathAndNameAndExtension)
    {
        return JsonUtility.FromJson<T>(File.ReadAllText(pathAndNameAndExtension));
    }
}
