using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(BadGuyStats))]
public class BadGuyCustomInspector : Editor
{
    private bool showWeapon;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUI.BeginChangeCheck();
        showWeapon = EditorGUILayout.Toggle("Show Weapon", showWeapon);
        if(EditorGUI.EndChangeCheck())
        {
           ((BadGuyStats)target).ShowWeapon(showWeapon);
        }
    }
}