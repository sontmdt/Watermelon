using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private static ServiceLocator _instance;
    private readonly Dictionary<Type, IService> _services = new();

    public static ServiceLocator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ServiceLocator>();
                if (_instance == null)
                {
                    var go = new GameObject("ServiceLocator");
                    _instance = go.AddComponent<ServiceLocator>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterService<T>(T service) where T : IService
    {
        Type type = typeof(T);
        if (_services.TryGetValue(type, out var old))
        {
            CleanupService(old);
            _services.Remove(type);
        }
        _services.Add(type, service);
        if (service is MonoBehaviour mono)
            mono.transform.SetParent(transform);
        if (service is IInitializable init)
            init.Initialize();
    }

    public T GetService<T>() where T : class, IService
    {
        if (_services.TryGetValue(typeof(T), out var service))
            return service as T;
        Debug.LogError($"[ServiceLocator] Service {typeof(T)} not registered!");
        return null;
    }

    public void RemoveService<T>() where T : IService
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            CleanupService(service);
            _services.Remove(typeof(T));
        }
    }

    public void RemoveAllServices()
    {
        foreach (var s in _services.Values) CleanupService(s);
        _services.Clear();
    }

    private void CleanupService(IService service)
    {
        if (service is MonoBehaviour mono)
            Destroy(mono.gameObject);
        else if (service is IDisposable disposable)
            disposable.Dispose();
    }
}
