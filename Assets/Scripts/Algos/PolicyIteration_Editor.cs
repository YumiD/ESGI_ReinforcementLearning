using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Algos.PolicyIteration))]
public class PolicyIteration_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //EditorGUILayout.LabelField("Label");
        Algos.PolicyIteration algo = (Algos.PolicyIteration)target;
        if (GUILayout.Button("Run Algorithm"))
        {
            algo.RunAlgorithm();
        }
    }
}
