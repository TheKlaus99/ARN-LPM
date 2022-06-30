using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ScaledRect))]
public class ScaledRectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ScaledRect myScript = (ScaledRect)target;
        if(GUILayout.Button("Set Object"))
        {
            myScript.Set();
        }
    }
}