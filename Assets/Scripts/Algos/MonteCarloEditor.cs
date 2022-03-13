using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Algos.MonteCarlo))]
public class MonteCarloEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //EditorGUILayout.LabelField("Label");
        Algos.MonteCarlo algo = (Algos.MonteCarlo)target;
        if (GUILayout.Button("Initialize Grid"))
        {
            algo.InitGrid();
        }

        if (GUILayout.Button("Run Monte Carlo ES Algorithm"))
        {
            algo.MonteCarloESAlgorithm();
        }

        if (GUILayout.Button("Play Game"))
        {
            algo.PlayGame();
        }
    }
}
