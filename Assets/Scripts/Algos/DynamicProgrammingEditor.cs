using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Algos.DynamicProgramming))]
public class DynamicProgrammingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //EditorGUILayout.LabelField("Label");
        Algos.DynamicProgramming algo = (Algos.DynamicProgramming)target;
        //if (GUILayout.Button("Initialize Grid"))
        //    algo.InitGrid();
        if (GUILayout.Button("Run Value Iteration Algorithm"))
            algo.ValueIterationAlgorithm();
        if (GUILayout.Button("Run Policy Iteration Algorithm"))
            algo.PolicyIterationAlgorithm();
        if (GUILayout.Button("Play Game"))
            algo.PlayGame();
    }
}
