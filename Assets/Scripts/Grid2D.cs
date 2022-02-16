using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Grid2D : MonoBehaviour
{
    [SerializeField] private ScriptableGrid grid;
    [SerializeField] private List<KvpIndexGameObject> listPrefab;
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private Camera cam;

    private Dictionary<int, GameObject> _listPrefabDictionary;
    private int[,] _gridCoordinate;

    private void Start()
    {
        cam.orthographicSize = grid.Width * 2f * Screen.height / Screen.width * 0.5f;
        cam.transform.position =
            new Vector3((grid.Width / 2f) - .5f, (grid.Height / 2f) - .5f, cam.transform.position.z);
        InitList();
        var player = Instantiate(playerPrefab, grid.SpawnPos, Quaternion.identity);
        player.Spawn(grid, grid.SpawnPos);
        Spawn();
    }

    private void Spawn()
    {
        for (int i = 0; i < grid.Width; i++)
        {
            for (int j = 0; j < grid.Height; j++)
            {
                Instantiate(_listPrefabDictionary[_gridCoordinate[i, j]], new Vector3(j, (grid.Width - 1) - i),
                    quaternion.identity);
            }
        }
    }

    private void InitList()
    {
        grid.InitGrid();
        _gridCoordinate = grid.GridCoordinate.Clone() as int[,];
        _listPrefabDictionary = new Dictionary<int, GameObject>();
        foreach (var prefab in listPrefab)
        {
            _listPrefabDictionary.Add(prefab.key, prefab.value);
        }
    }
}