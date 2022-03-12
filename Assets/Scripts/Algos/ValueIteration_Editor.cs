using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Algos.ValueIteration))]
public class ValueIteration_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //EditorGUILayout.LabelField("Label");
        Algos.ValueIteration algo = (Algos.ValueIteration)target;
        if(GUILayout.Button("Run Algorithm"))
        {
            algo.RunAlgorithm();
        }
    }
}
