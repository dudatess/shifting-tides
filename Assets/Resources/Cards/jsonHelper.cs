using UnityEngine;
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string fixedJson = "{\"array\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(fixedJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}