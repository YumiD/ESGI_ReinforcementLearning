using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algos
{
    public class Q
    {
        private Vector2Int _pos;
        private PossibleMovement _action;

        public Q(Vector2Int pos, PossibleMovement action)
        {
            _pos = pos;
            _action = action;
        }
    }

    public class MonteCarlo : MonoBehaviour, IAlgorithm
    {
        [SerializeField] private Grid2D grid;
        [SerializeField] private int _INSTANCES_MAX = 100;

        private float[,] _rewardGrid;
        private PossibleMovement[,] _policyGrid;
        private Dictionary<Q, int> _QGrid;

        private const float GAMMA = 0.9f;

        public void InitAlgorithm()
        {
            _rewardGrid = new float[grid.Width, grid.Height];
            _policyGrid = new PossibleMovement[grid.Width, grid.Height];
            _QGrid = new Dictionary<Q, int>();

            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    _rewardGrid[i, j] = grid.GridCoordinate[j, i].rewardValue;
                    _policyGrid[i, j] = (PossibleMovement)UnityEngine.Random.Range(0, 4);
                    Vector2Int pos = new Vector2Int(i, j);
                    foreach (PossibleMovement action in grid.getPossibleActions(pos))
                        _QGrid.Add(new Q(pos, action), 0);
                }
            }
        }

        public void MonteCarloESAlgorithm()
        {
            InitAlgorithm();
            DisplayPolicyGrid();
        }

        public void PlayGame()
        {
            throw new NotImplementedException();
        }

        public void DisplayPolicyGrid()
        {
            int k = 0;
            //print("POLICY");
            for (var y = 0; y < grid.Height; y++)
            {
                string output = "";
                for (var x = 0; x < grid.Width; x++)
                {
                    output += " " + _policyGrid[x, y];
                    if (grid.EveryTiles[k].TryGetComponent(out CellDisplay cell))
                    {
                        cell.DisplayValue(_policyGrid[x, y].ToString());
                    }

                    k++;
                }

                //print(output);
            }
        }

        public void RunAlgorithm()
        {
            throw new NotImplementedException();
        }
        public void InitGrid()
        {
            grid.RestartGrid(); //TODO Better Restart
        }
    }
}
