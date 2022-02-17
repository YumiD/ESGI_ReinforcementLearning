using System;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

[CreateAssetMenu(fileName = "Grid", menuName = "ESGI/Grid")]
public class ScriptableGrid : ScriptableObject
{
    private int _height, _width;

    [SerializeField] private Vector2 spawnPos;

    [SerializeField] private string folderName;
    [SerializeField] private string fileName;
    
    private State _state = new State();

    public void InitGrid()
    {
        var array = RetrieveFile.GetFile(folderName, fileName);
        _width = array.Length;
        _height = array[0].Replace(" ", string.Empty).Length;
        foreach (var line in array)
        {
            if (line.Replace(" ", string.Empty).Length > _height)
            {
                _height = line.Replace(" ", string.Empty).Length;
            }
        }
        _state.Grid = new int[_width, _height];
        for (int i = _width - 1; i >= 0; i--)
        {
            var words = array[i].Split(' ');
            for (int j = 0; j < _height; j++)
            {
                if (i == (int)spawnPos.x && j == (int)spawnPos.y)
                {
                    _state.Grid[_width - 1 - i, j] = (int)TileType.Player;
                }
                else if (!string.IsNullOrEmpty(words[j]))
                {
                    _state.Grid[i, j] = int.Parse(words[j]);
                }
            }
        }
    }

    public int Height => _height;

    public int Width => _width;

    public State State => _state;

    public Vector2 SpawnPos => spawnPos;
}