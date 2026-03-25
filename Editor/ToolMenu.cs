using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;
public class ToolMenu
{
    private const string pathResources = "Assets/Resources";

    private const string menuTitle = "MyTool/";

    [MenuItem(menuTitle + "Create Game Setting", false, 1)]
    static void CreateGameData()
    {
        GameSettings asset = ScriptableObject.CreateInstance<GameSettings>();

        AssetDatabase.CreateAsset(asset, pathResources + "/GameSettings.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
    [MenuItem(menuTitle + "Game Settings", false, 2)]
    static void OpenGameData()
    {
        GameSettings asset = Resources.Load<GameSettings>("gamesettings");

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

}
