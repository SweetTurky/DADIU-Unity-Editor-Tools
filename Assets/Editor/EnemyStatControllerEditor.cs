using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class EnemyStatControllerEditor : EditorWindow
{
    // Change enemyType to enum type
    private class EnemyPrefabInfo
    {
        public EnemyType enemyType;
        public GameObject prefab;
        public Enemy enemyComponent;
    }

    private List<EnemyPrefabInfo> enemyPrefabs = new List<EnemyPrefabInfo>();
    private List<EnemyStat> allEnemyStats = new List<EnemyStat>();

    [MenuItem("Tools/Enemy Stat Controller")]
    public static void ShowWindow()
    {
        GetWindow<EnemyStatControllerEditor>("Enemy Stat Controller");
    }

    private void OnEnable()
    {
        RefreshData();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Refresh"))
        {
            RefreshData();
        }

        if (enemyPrefabs.Count == 0)
        {
            EditorGUILayout.HelpBox("No Enemy prefabs found in the project.", MessageType.Warning);
            return;
        }

        if (allEnemyStats.Count == 0)
        {
            EditorGUILayout.HelpBox("No EnemyStat ScriptableObjects found in the project.", MessageType.Warning);
            return;
        }

        // Draw UI for each enemy prefab
        foreach (var enemyInfo in enemyPrefabs)
        {
            EditorGUILayout.LabelField(enemyInfo.enemyType.ToString(), EditorStyles.boldLabel);

            // Filter stats matching this enemy type (enum comparison)
            var matchingStats = allEnemyStats.Where(stat => stat.enemyType == enemyInfo.enemyType).ToList();

            if (matchingStats.Count == 0)
            {
                EditorGUILayout.LabelField("No stats found for this enemy type.");
                continue;
            }

            EditorGUILayout.BeginHorizontal();

            foreach (var stat in matchingStats)
            {
                if (GUILayout.Button(stat.name))
                {
                    AssignStatToPrefab(enemyInfo, stat);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
    }

    private void RefreshData()
    {
        enemyPrefabs.Clear();
        allEnemyStats.Clear();

        // Find all EnemyStat ScriptableObjects in the project
        string[] statGuids = AssetDatabase.FindAssets("t:EnemyStat");
        foreach (string guid in statGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EnemyStat stat = AssetDatabase.LoadAssetAtPath<EnemyStat>(path);
            if (stat != null)
                allEnemyStats.Add(stat);
        }

        // Find all prefabs with Enemy component in the project
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
                continue;

            Enemy enemyComp = prefab.GetComponent<Enemy>();
            if (enemyComp != null)
            {
                EnemyType enemyType;

                // Try to get enemyType from assigned EnemyStat if available
                if (enemyComp.enemyStat != null)
                {
                    enemyType = enemyComp.enemyStat.enemyType;
                }
                else
                {
                    // Fallback: try parse prefab name to EnemyType enum
                    if (!System.Enum.TryParse(prefab.name, out enemyType))
                    {
                        // If parsing fails, assign a default or skip
                        Debug.LogWarning($"Prefab name '{prefab.name}' does not match any EnemyType enum. Assigning default Goblin.");
                        enemyType = EnemyType.Goblin;
                    }
                }

                enemyPrefabs.Add(new EnemyPrefabInfo
                {
                    enemyType = enemyType,
                    prefab = prefab,
                    enemyComponent = enemyComp
                });
            }
        }
    }

    private void AssignStatToPrefab(EnemyPrefabInfo enemyInfo, EnemyStat stat)
    {
        // Load the prefab asset as editable
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(enemyInfo.prefab));
        Enemy enemyComp = prefabRoot.GetComponent<Enemy>();

        if (enemyComp == null)
        {
            Debug.LogError($"Prefab {enemyInfo.prefab.name} does not have an Enemy component.");
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            return;
        }

        Undo.RecordObject(enemyComp, "Assign EnemyStat");
        enemyComp.enemyStat = stat;

        EditorUtility.SetDirty(enemyComp);

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, AssetDatabase.GetAssetPath(enemyInfo.prefab));
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        Debug.Log($"Assigned {stat.name} to prefab {enemyInfo.prefab.name}");
    }
}