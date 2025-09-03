using UnityEngine;
using UnityEditor;

public class ReorganizeHierarchyEditor
{
    // Validation: enable menu only if at least one GameObject is selected
    [MenuItem("Tools/Reorganize Hierarchy #&e", true)] // #&e = Shift + Alt + E
    private static bool ValidateReorganizeHierarchy()
    {
        return Selection.gameObjects.Length > 0;
    }

    // Menu item with shortcut Shift + Alt + E
    [MenuItem("Tools/Reorganize Hierarchy #&e")]
    private static void ReorganizeHierarchy()
    {
        // Create new empty GameObject at origin
        GameObject newParent = new("Grouped Objects");
        Undo.RegisterCreatedObjectUndo(newParent, "Create Group Parent");
        newParent.transform.position = Vector3.zero;

        // Parent all selected GameObjects under the new GameObject
        foreach (GameObject go in Selection.gameObjects)
        {
            Undo.SetTransformParent(go.transform, newParent.transform, "Reparent Selected Objects");
        }

        // Select the new parent GameObject after grouping
        Selection.activeGameObject = newParent;
    }
}