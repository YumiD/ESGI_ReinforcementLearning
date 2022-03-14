using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private readonly Dictionary<Vector2, bool> _goalPositionState = new Dictionary<Vector2, bool>();

    public int Height { get; private set; }
    public int Width { get; private set; }

    public Cell[,] GridCoordinate { get; private set; }

    public List<GameObject> EveryTiles { get; } = new List<GameObject>();

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
                if (GridCoordinate[i, j].value == (int)TileType.Void)
                {
                    EveryTiles.Add(null);
                    continue;
                }

                var obj = Instantiate(_listPrefabDictionary[GridCoordinate[i, j].value],
                    new Vector2(j, grid.Width - 1 - i),
                    Quaternion.identity);
                EveryTiles.Add(obj);

                switch (GridCoordinate[i, j].value)
                {
                    case (int)TileType.Player: // Spawn player on ground tile
                        Instantiate(_listPrefabDictionary[(int)TileType.Ground],
                            new Vector2(j, grid.Width - 1 - i),
                            Quaternion.identity);
                        obj.GetComponent<PlayerController>()
                            .Spawn(grid, new Vector2(grid.SpawnPos.y, grid.SpawnPos.x), this, new List<Vector2>());
                        obj.name = "Player";
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
        Height = grid.Height;
        Width = grid.Width;
        GridCoordinate = new Cell[grid.Width, grid.Height];
        for (var i = 0; i < grid.Width; i++)
        {
            for (var j = 0; j < grid.Height; j++)
            {
                var cellOnGrid = listPrefab.Find(c => c.key == grid.State.Grid[i, j]);
                if (cellOnGrid != null)
                {
                    GridCoordinate[i, j] = new Cell(grid.State.Grid[i, j],
                        listPrefab.Find(c => c.key == grid.State.Grid[i, j]).reward);
                }
                else
                {
                    GridCoordinate[i, j] = new Cell(grid.State.Grid[i, j], -1);
                }
            }
        }

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
        (EveryTiles[tileNb], EveryTiles[(int)newPosInList]) = (EveryTiles[(int)newPosInList], EveryTiles[tileNb]);
        EveryTiles[(int)newPosInList].transform.position = newPos;
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

    public Vector2Int GetDeplacementPosition(PossibleMovement movement, Vector2Int pos)
    {
        Vector2Int nextPos = new Vector2Int(pos.x, pos.y);
        switch (movement)
        {
            case PossibleMovement.Up:
                nextPos.y -= 1;
                break;
            case PossibleMovement.Down:
                nextPos.y += 1;
                break;
            case PossibleMovement.Right:
                nextPos.x += 1;
                break;
            case PossibleMovement.Left:
                nextPos.x -= 1;
                break;
            default:
                break;
        }
        if (nextPos.x < 0 || nextPos.x >= grid.Width || nextPos.y < 0 || nextPos.y >= grid.Height)
            return new Vector2Int(int.MinValue, int.MinValue); ;
        return nextPos;
    }
    public bool CanMove(PossibleMovement movement, Vector2Int pos)
    {
        Vector2Int nextPos = GetDeplacementPosition(movement, pos);
        if (nextPos.x == int.MinValue && nextPos.y == int.MinValue)
            return false;
        switch (grid.State.Grid[nextPos.y, nextPos.x])
        {
            case (int)TileType.Ground:
                return true;
            case (int)TileType.Player:
                return true;
            case (int)TileType.Goal:
                return true;
            default:
                return false;
        }
    }

    public void Display()
    {
        for (var i = 0; i < grid.Width; i++)
        {
            string output = "";
            for (var j = 0; j < grid.Height; j++)
            {
                output += " " + grid.State.Grid[i,j];
                //output += " " + GridCoordinate[i, j].value;
            }
            print(output);
        }
        /*print("Height : "+grid.Height);
        print("Width : " + grid.Width);
        print("SpawnPos : " + grid.SpawnPos);
        print(GridCoordinate[(int)grid.SpawnPos.x, (int)grid.SpawnPos.y]);*/
    }

    public Vector2 getSpawnPos()
    {
        return grid.SpawnPos;
    }
    public Vector2 getGoalPos()
    {
        return grid.GoalPos;
    }

    public bool isGoal(Vector2Int pos)
    {
        if (pos.x == getGoalPos().y && pos.y == getGoalPos().x)
            return true;
        return false;
    }

    public void RestartGrid()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}