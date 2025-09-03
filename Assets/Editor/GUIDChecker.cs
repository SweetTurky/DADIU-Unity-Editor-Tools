using UnityEngine;
using UnityEditor;
using System.IO;

public class GUIDChecker : EditorWindow
{
    private string guidToCheck = "";
    private string resultMessage = "";

    [MenuItem("Tools/GUID Checker")]
    public static void ShowWindow()
    {
        GetWindow<GUIDChecker>("GUID Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Check if a GUID exists in the project", EditorStyles.boldLabel);

        guidToCheck = EditorGUILayout.TextField("GUID", guidToCheck);

        if (GUILayout.Button("Check GUID"))
        {
            CheckGUIDExists(guidToCheck);
        }

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(resultMessage, MessageType.Info);
    }

    private void CheckGUIDExists(string guid)
    {
        if (string.IsNullOrEmpty(guid))
        {
            resultMessage = "Please enter a GUID.";
            return;
        }

        // Search for the asset path by GUID
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);

        if (!string.IsNullOrEmpty(assetPath) && File.Exists(assetPath))
        {
            resultMessage = $"GUID exists! Asset path: {assetPath}";
        }
        else
        {
            resultMessage = "GUID does NOT exist in the project.";
        }
    }
}