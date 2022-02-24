using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileType
{
    Ground = 0,
    Obstacle = 1,
    Goal = 2,
    Player = 3,
    Crate = 4,
    Wall = 5,
    Void = 6,
    GoalCrate = 7
}

public class Grid2D : MonoBehaviour
{
    [SerializeField] private ScriptableGrid grid;
    [SerializeField] private List<KvpIndexGameObject> listPrefab;
    [SerializeField] private Camera cam;

    private Dictionary<int, GameObject> _listPrefabDictionary;
    private int[,] _gridCoordinate;
    private List<GameObject> _everyTiles = new List<GameObject>();
    private Dictionary<Vector2, bool> _goalPositionState = new Dictionary<Vector2, bool>();

    private void Start()
    {
        InitList();
        cam.orthographicSize = grid.Width * 2f * Screen.height / Screen.width * 0.5f;
        cam.transform.position =
            new Vector3(grid.Width / 2f - .5f, grid.Height / 2f - .5f, cam.transform.position.z);

        Spawn();
    }

    private void Spawn()
    {
        for (var i = 0; i < grid.Width; i++)
        {
            for (var j = 0; j < grid.Height; j++)
            {
                if (_gridCoordinate[i, j] == (int)TileType.Void)
                {
                    _everyTiles.Add(null);
                    continue;
                }

                var obj = Instantiate(_listPrefabDictionary[_gridCoordinate[i, j]],
                    new Vector2(j, grid.Width - 1 - i),
                    Quaternion.identity);
                _everyTiles.Add(obj);

                switch (_gridCoordinate[i, j])
                {
                    case (int)TileType.Player: // Spawn player on ground tile
                        Instantiate(_listPrefabDictionary[(int)TileType.Ground],
                            new Vector2(j, grid.Width - 1 - i),
                            Quaternion.identity);
                        obj.GetComponent<PlayerController>()
                            .Spawn(grid, new Vector2(grid.SpawnPos.y, grid.SpawnPos.x), this, new List<Vector2>());
                        break;
                    // Spawn under objects
                    case (int)TileType.GoalCrate:
                        _goalPositionState.Add(new Vector2(i, j), false);
                        goto case (int)TileType.Obstacle;
                    case (int)TileType.Obstacle:
                    case (int)TileType.Crate:
                        Instantiate(_listPrefabDictionary[(int)TileType.Ground],
                            new Vector2(j, grid.Width - 1 - i),
                            Quaternion.identity);
                        break;
                }
            }
        }
    }

    private void InitList()
    {
        grid.InitGrid();
        _gridCoordinate = grid.State.Grid.Clone() as int[,];
        _listPrefabDictionary = new Dictionary<int, GameObject>();
        foreach (var prefab in listPrefab)
        {
            _listPrefabDictionary.Add(prefab.key, prefab.value);
        }
    }

    public void MoveTile(int tileNb, Vector2 newPos)
    {
        var newPosInList = newPos.x + (grid.Height - 1 - newPos.y) * grid.Width;
        // Swap gameobject to match the scene grid on array grid
        (_everyTiles[tileNb], _everyTiles[(int)newPosInList]) = (_everyTiles[(int)newPosInList], _everyTiles[tileNb]);
        _everyTiles[(int)newPosInList].transform.position = newPos;
    }

    public bool IsActionPossible(Movement movement, Vector2Int pos, bool pushCrate, bool isHorizontal)
    {
        var multiplier = pushCrate ? 2 : 1;
        if (pos.x + multiplier * (int)movement < 0 || pos.x + (int)movement > grid.Width)
        {
            return false;
        }

        if (pos.y + multiplier * (int)movement < 0 || pos.y + (int)movement > grid.Height)
        {
            return false;
        }

        switch (grid.State.Grid[grid.Height - 1 - pos.y, pos.x])
        {
            case (int)TileType.Wall:
            case (int)TileType.Void:
            case (int)TileType.Obstacle:
                return false;
            case (int)TileType.Crate:
            {
                if (isHorizontal)
                {
                    pos.x += (int)movement;
                }
                else
                {
                    pos.y += (int)movement;
                }

                return grid.State.Grid[grid.Height - 1 - pos.y, pos.x] != (int)TileType.Wall &&
                       grid.State.Grid[grid.Height - 1 - pos.y, pos.x] != (int)TileType.Void &&
                       grid.State.Grid[grid.Height - 1 - pos.y, pos.x] != (int)TileType.Obstacle &&
                       grid.State.Grid[grid.Height - 1 - pos.y, pos.x] != (int)TileType.Crate;
            }
            default:
                return true;
        }
    }

    public void VerifyCrateOnGoal(Vector2 cratePosition, Vector2 playerPosition)
    {
        if (grid.State.Grid[grid.Height - 1 - (int)cratePosition.y, (int)cratePosition.x] == (int)TileType.GoalCrate)
        {
            var crateArrayPos = new Vector2(grid.Height - 1 - cratePosition.y, cratePosition.x);
            if (_goalPositionState.ContainsKey(crateArrayPos))
            {
                _goalPositionState[crateArrayPos] = !_goalPositionState[crateArrayPos];
            }   
        }

        var playerArrayPos = new Vector2(grid.Height - 1 - (int)playerPosition.y, (int)playerPosition.x);
        if (_goalPositionState.ContainsKey(playerArrayPos))
        {
            _goalPositionState[playerArrayPos] = false;
        }

        if (_goalPositionState.Any(kv => !kv.Value))
        {
            return;
        }
        Debug.Log("Win");
    }
}