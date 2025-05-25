using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Secret
{
    public string secret;
    public string device_id;
}

public static class JsonUtilityWrapper
{
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }

    public static List<T> FromJsonList<T>(string json)
    {
        string wrapped = "{ \"Items\": " + json + "}";
        return JsonUtility.FromJson<Wrapper<T>>(wrapped).Items;
    }
}