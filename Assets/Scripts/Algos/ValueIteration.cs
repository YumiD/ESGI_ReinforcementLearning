using System;
using UnityEngine;

namespace Algos
{
    public class ValueIteration : MonoBehaviour, IAlgorithm
    {
        [SerializeField] private Grid2D grid;

        private float[,] _rewardFunction;
        private float[,] _valueFunction;
        private const float GAMMA = 0.9f;
        private const float THETA = 0.1f;
        public void InitAlgorithm()
        {
            _valueFunction = new float[grid.Width, grid.Height];
            _rewardFunction = new float[grid.Width, grid.Height];

            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    _rewardFunction[i, j] = grid.GridCoordinate[i, j].rewardValue;
                    _valueFunction[i, j] = 0.0f;
                }
            }
        }

        public void RunAlgorithm()
        {
            InitAlgorithm();

            float DELTA = float.MaxValue;

            print(grid.GridCoordinate[0, 0].value);
            while (DELTA > THETA)
            {
                DELTA = 0.0f;
                for (var i = 0; i < grid.Width; i++)
                {
                    for (var j = 0; j < grid.Height; j++)
                    {
                        if(grid.GridCoordinate[i, j].value == (int)TileType.Player || grid.GridCoordinate[i, j].value == (int)TileType.Ground)
                        {
                            float oldValue = _valueFunction[i, j];

                            float maxValue = float.MinValue;
                            Vector2Int currentPosition = new Vector2Int(i, j);
                            foreach (PossibleMovement movement in Enum.GetValues(typeof(PossibleMovement)))
                            {
                                if (grid.CanMove(movement, currentPosition, false))
                                {
                                    Vector2Int newPosition = grid.GetDeplacementPosition(movement, currentPosition);
                                    float value = _rewardFunction[newPosition.x, newPosition.y] + GAMMA * _valueFunction[newPosition.x, newPosition.y]; //TODO comment ça newPosition???
                                    if (value > maxValue)
                                        maxValue = value;
                                }
                            }
                            if (maxValue != float.MinValue)
                                _valueFunction[i, j] = maxValue;
                            DELTA = Mathf.Max(DELTA, Mathf.Abs(oldValue - _valueFunction[i, j]));
                        }
                    }
                }
            }

            DisplayValueGrid();
        }

        public void DisplayValueGrid()
        {
            for (var i = 0; i < grid.Width; i++)
            {
                string output = "";
                for (var j = 0; j < grid.Height; j++)
                {
                    output += " " + _valueFunction[i, j];
                }
                print(output);
            }
        }

    }
}