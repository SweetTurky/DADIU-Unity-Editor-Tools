using UnityEngine;
using UnityEditor;

public class SnapToPositionEditor
{
    // Menu item with shortcut Shift+R
    [MenuItem("Tools/Snap To Position #r", true)] // Validation function
    private static bool ValidateSnapToPosition()
    {
        // Enable menu only if at least one GameObject is selected
        return Selection.activeGameObject != null;
    }

    [MenuItem("Tools/Snap To Position #r")] // %#R means Shift+Ctrl+R, but we want Shift+R only
    private static void SnapToPosition()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            Undo.RecordObject(go.transform, "Snap Position");

            Vector3 pos = go.transform.position;
            pos.x = Mathf.Round(pos.x * 2f) / 2f;
            pos.y = Mathf.Round(pos.y * 2f) / 2f;
            pos.z = Mathf.Round(pos.z * 2f) / 2f;

            go.transform.position = pos;
        }
    }
}