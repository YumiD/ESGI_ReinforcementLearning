using System;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

[CreateAssetMenu(fileName = "Grid", menuName = "ESGI/Grid")]
public class ScriptableGrid : ScriptableObject
{
    [SerializeField] private string folderName;
    [SerializeField] private string fileName;

    public void InitGrid()
    {
        var array = RetrieveFile.GetFile(folderName, fileName);
        Width = array.Length;
        Height = array[0].Replace(" ", string.Empty).Length;
        foreach (var line in array)
        {
            if (line.Replace(" ", string.Empty).Length > Height)
            {
                Height = line.Replace(" ", string.Empty).Length;
            }
        }

        State.Grid = new int[Width, Height];
        for (var i = Width - 1; i >= 0; i--)
        {
            var words = array[i].Split(' ');
            for (var j = 0; j < Height; j++)
            {
                if (int.Parse(words[j]) == (int)TileType.Player)
                {
                    SpawnPos = new Vector2(Width - 1 -i, j);
                }
                State.Grid[i, j] = int.Parse(words[j]);
            }
        }
    }

    public int Height { get; private set; }

    public int Width { get; private set; }

    public State State { get; } = new State();

    public Vector2 SpawnPos { get; private set; }
}