using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("EventManager");
                _instance = obj.AddComponent<EventManager>();
                DontDestroyOnLoad(obj);
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

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _listeners.Clear();
            _instance = null;
        }
    }

    private readonly Dictionary<EventID, Action<object>> _listeners = new();

    public void RegisterListener(EventID eventID, Action<object> callback)
    {
        if (_listeners.ContainsKey(eventID))
            _listeners[eventID] += callback;
        else
        {
            _listeners[eventID] = null;
            _listeners[eventID] += callback;
        }
    }

    public void RemoveListener(EventID eventID, Action<object> callback)
    {
        if (_listeners.ContainsKey(eventID))
            _listeners[eventID] -= callback;
    }

    public void PostEvent(EventID eventID, object param = null)
    {
        if (!_listeners.TryGetValue(eventID, out var callbacks) || callbacks == null) return;
        callbacks(param);
    }
}

public static class EventManagerExtension
{
    public static void RegisterListener(this MonoBehaviour listener, EventID eventID, Action<object> callback)
        => EventManager.Instance.RegisterListener(eventID, callback);

    public static void PostEvent(this MonoBehaviour sender, EventID eventID, object param = null)
        => EventManager.Instance.PostEvent(eventID, param);
}
