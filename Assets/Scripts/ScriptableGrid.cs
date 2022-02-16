using System;
using UnityEngine;
using Utilities;

[CreateAssetMenu(fileName = "Grid", menuName = "ESGI/Grid")]
public class ScriptableGrid : ScriptableObject
{
    [SerializeField] private int height, width;

    [SerializeField] private Vector2 spawnPos;

    [SerializeField] private string folderName;
    [SerializeField] private string fileName;

    private int[,] _gridCoordinate;

    public void InitGrid()
    {
        _gridCoordinate = new int[width, height];
        var array = RetrieveFile.GetFile(folderName, fileName);
        for (int i = 0; i < array.Length; i++)
        {
            var words = array[i].Split(' ');
            for (int j = 0; j < width; j++)
            {
                if (!string.IsNullOrEmpty(words[j]))
                {
                    _gridCoordinate[i, j] = int.Parse(words[j]);
                }
            }
        }
    }

    public int Height => height;

    public int Width => width;

    public int[,] GridCoordinate => _gridCoordinate;

    public Vector2 SpawnPos => spawnPos;
}