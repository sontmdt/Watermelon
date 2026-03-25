using System;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static Helper Instance { get; private set; }

    public UIGameStarted uiGameStarted;
    public UIGameOver uiGameOver;

    private readonly Dictionary<EventID, MenuBase> _menuDict = new();
    private readonly Dictionary<EventID, Action<object>> _actions = new();
    private MenuBase _currentMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    internal void Setup()
    {
        // Remove old listeners before re-registering
        foreach (var kvp in _actions)
            EventManager.Instance.RemoveListener(kvp.Key, kvp.Value);
        _actions.Clear();
        _menuDict.Clear();

        var menus = GetComponentsInChildren<MenuBase>(true);
        foreach (var menu in menus)
        {
            menu.Setup();
            if (_menuDict.ContainsKey(menu.eventID)) continue;

            _menuDict[menu.eventID] = menu;
            var id = menu.eventID;
            _actions[id] = _ => ShowMenu(id);
            this.RegisterListener(id, _actions[id]);

            if (menu is UIGameStarted gs) uiGameStarted = gs;
            if (menu is UIGameOver   go) uiGameOver   = go;
        }
    }

    private void ShowMenu(EventID eventID)
    {
        if (!_menuDict.TryGetValue(eventID, out var next)) return;
        if (_currentMenu == next) return;
        _currentMenu?.Hide();
        next.Show();
        _currentMenu = next;
    }

    private void OnDestroy() => _actions.Clear();
}
