using System.IO;
using UnityEngine;

public class FileSaveService : MonoBehaviour, ISaveService, IInitializable
{
    private string RootPath => Application.persistentDataPath;

    public void Initialize() { }

    public T Load<T>(string key) where T : new()
    {
        string path = GetPath(key);
        if (!File.Exists(path)) return new T();
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }

    public void Save<T>(string key, T data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(key), json);
    }

    public void Delete(string key)
    {
        string path = GetPath(key);
        if (File.Exists(path)) File.Delete(path);
    }

    public bool HasKey(string key) => File.Exists(GetPath(key));

    private string GetPath(string key) => Path.Combine(RootPath, key + ".json");
}
