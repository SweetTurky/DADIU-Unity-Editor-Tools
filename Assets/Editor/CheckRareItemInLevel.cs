using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckRareItemInLevel : EditorWindow
{
    private SceneAsset sceneToCheck;

    [MenuItem("Tools/Check Rare Item In Level")]
    public static void ShowWindow()
    {
        GetWindow<CheckRareItemInLevel>("Check Rare Item");
    }

    private void OnGUI()
    {
        GUILayout.Label("Rare Item Checker", EditorStyles.boldLabel);

        sceneToCheck = (SceneAsset)EditorGUILayout.ObjectField("Scene to Check", sceneToCheck, typeof(SceneAsset), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Check Level"))
        {
            if (sceneToCheck != null)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneToCheck));
                    CheckRareItemsInCurrentScene();
                }
            }
            else
            {
                CheckRareItemsInCurrentScene();
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Check All Scenes in Project"))
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                CheckAllScenes();
            }
        }
    }

    private void CheckRareItemsInCurrentScene()
    {
        RareItem[] rareItems = Object.FindObjectsByType<RareItem>(FindObjectsSortMode.None);

        if (rareItems.Length == 1)
        {
            EditorUtility.DisplayDialog("Rare Item Check", "Exactly one Rare Item found. All good!", "OK");
        }
        else if (rareItems.Length == 0)
        {
            GameObject newRareItem = new GameObject("RareItem");
            Undo.RegisterCreatedObjectUndo(newRareItem, "Create RareItem");
            newRareItem.AddComponent<RareItem>();
            EditorUtility.DisplayDialog("Rare Item Check", "No Rare Item found. Added one to the scene.", "OK");
        }
        else
        {
            Undo.RecordObjects(rareItems, "Remove extra RareItems");
            for (int i = 1; i < rareItems.Length; i++)
            {
                Undo.DestroyObjectImmediate(rareItems[i].gameObject);
            }
            EditorUtility.DisplayDialog("Rare Item Check", $"Found {rareItems.Length} Rare Items. Removed extras, kept the first one.", "OK");
        }
    }



    private void CheckAllScenes()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        List<string> failedScenes = new List<string>();
        List<string> skippedScenes = new List<string>();

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);

            // Skip scenes outside the Assets folder (e.g., Packages)
            if (!scenePath.StartsWith("Assets/"))
            {
                skippedScenes.Add(scenePath);
                continue;
            }

            Scene scene = EditorSceneManager.OpenScene(scenePath);

            RareItem[] rareItems = Object.FindObjectsByType<RareItem>(FindObjectsSortMode.None);

            if (rareItems.Length == 0)
            {
                GameObject newRareItem = new GameObject("RareItem");
                Undo.RegisterCreatedObjectUndo(newRareItem, "Create RareItem");
                newRareItem.AddComponent<RareItem>();
                EditorSceneManager.MarkSceneDirty(scene);
            }
            else if (rareItems.Length > 1)
            {
                Undo.RecordObjects(rareItems, "Remove extra RareItems");
                for (int i = 1; i < rareItems.Length; i++)
                {
                    Undo.DestroyObjectImmediate(rareItems[i].gameObject);
                }
                EditorSceneManager.MarkSceneDirty(scene);
            }

            if (scene.isDirty)
            {
                if (!EditorSceneManager.SaveScene(scene))
                {
                    failedScenes.Add(scenePath);
                }
            }
        }

        string message = "";

        if (failedScenes.Count == 0)
        {
            message += "All scenes checked and updated successfully.\n";
        }
        else
        {
            message += $"Failed to save some scenes:\n{string.Join("\n", failedScenes)}\n";
        }

        if (skippedScenes.Count > 0)
        {
            message += $"\nSkipped scenes outside Assets folder:\n{string.Join("\n", skippedScenes)}";
        }

        EditorUtility.DisplayDialog("Check All Scenes", message, "OK");
    }
}