using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class OpenPersistentDataPathEditor
{
    [MenuItem("Tools/Open Persistent Data Path")]
    private static void OpenPersistentDataPath()
    {
        string path = Application.persistentDataPath;

        if (!Directory.Exists(path))
        {
            UnityEngine.Debug.LogWarning($"Persistent Data Path does not exist: {path}");
            return;
        }

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            EditorUtility.RevealInFinder(path);
        }
        else if (Application.platform == RuntimePlatform.LinuxEditor)
        {
            Process.Start("xdg-open", Path.GetFullPath(path));
        }
        else
        {
            UnityEngine.Debug.LogWarning("Opening folder is not supported on this platform.");
        }
    }
}