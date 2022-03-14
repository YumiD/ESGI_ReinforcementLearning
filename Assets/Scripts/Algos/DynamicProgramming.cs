using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algos
{
    public class DynamicProgramming : MonoBehaviour, IAlgorithm
    {
        [SerializeField] private Grid2D grid;
        [SerializeField] private int _INSTANCES_MAX = 50;

        private float[,] _rewardFunction;
        private float[,] _valueFunction;
        private PossibleMovement[,] _policyFunction;

        private const float GAMMA = 0.9f;

        public enum DynamicAlgos
        {
            Value,
            Policy
        };

        private DynamicAlgos currentDynamicAlgo;

        public void InitGrid()
        {
            grid.RestartGrid(); //TODO Better Restart
        }

        public void InitAlgorithm()
        {
            _rewardFunction = new float[grid.Width, grid.Height];
            _valueFunction = new float[grid.Width, grid.Height];
            _policyFunction = new PossibleMovement[grid.Width, grid.Height];

            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    _rewardFunction[i, j] = grid.GridCoordinate[j, i].rewardValue;
                    _valueFunction[i, j] = 0.0f;
                    _policyFunction[i, j] = (PossibleMovement)UnityEngine.Random.Range(0, 4);
                }
            }
        }

        public void ValueIterationAlgorithm()
        {
            InitAlgorithm();
            currentDynamicAlgo = DynamicAlgos.Value;

            int instance = 0;
            while (instance < _INSTANCES_MAX)
            {
                for (var i = 0; i < grid.Width; i++)
                {
                    for (var j = 0; j < grid.Height; j++)
                    {
                        if (grid.GridCoordinate[j, i].value == (int)TileType.Player ||
                            grid.GridCoordinate[j, i].value == (int)TileType.Ground)
                        {
                            float oldValue = _valueFunction[i, j];

                            float maxActionValue = float.MinValue;
                            Vector2Int currentPosition = new Vector2Int(i, j);
                            foreach (PossibleMovement movement in Enum.GetValues(typeof(PossibleMovement)))
                            {
                                if (grid.CanMove(movement, currentPosition))
                                {
                                    Vector2Int newPosition = grid.GetDeplacementPosition(movement, currentPosition);
                                    float value = _rewardFunction[newPosition.x, newPosition.y] +
                                                  GAMMA * _valueFunction[newPosition.x, newPosition.y];
                                    if (value > maxActionValue)
                                        maxActionValue = value;
                                }
                            }
                            if (maxActionValue != float.MinValue)
                                _valueFunction[i, j] = maxActionValue;
                        }
                    }
                }

                instance++;
            }
            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    if (grid.GridCoordinate[j, i].value == (int)TileType.Goal ||
                        grid.GridCoordinate[j, i].value == (int)TileType.Obstacle)
                    {
                       _valueFunction[i, j] = _rewardFunction[i, j];
                    }
                }
            }

            DisplayValueGrid();
        }

        public void PolicyIterationAlgorithm()
        {
            InitAlgorithm();
            currentDynamicAlgo = DynamicAlgos.Policy;
            ValueIterationAlgorithm();
            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    _policyFunction[i, j] = FindBestDirection(new Vector2Int(i, j));
                }
            }

            DisplayPolicyGrid();
        }

        public void PlayGame()
        {
            if (currentDynamicAlgo == DynamicAlgos.Value)
                StartCoroutine(StepValue());
            if (currentDynamicAlgo == DynamicAlgos.Policy)
                StartCoroutine(StepPolicy());
        }

        private IEnumerator StepValue()
        {
            PlayerController PC = GameObject.Find("Player").GetComponent<PlayerController>();
            Vector2Int currentPos =
                new Vector2Int((int)grid.getSpawnPos().y, grid.Width - 1 - (int)grid.getSpawnPos().x);
            bool victory = false;
            while (!victory)
            {
                yield return new WaitForSeconds(0.5f);
                switch (FindBestDirection(currentPos))
                {
                    case PossibleMovement.Up:
                        currentPos.y -= 1;
                        PC.Move(new Vector2(0, 1));
                        break;
                    case PossibleMovement.Down:
                        currentPos.y += 1;
                        PC.Move(new Vector2(0, -1));
                        break;
                    case PossibleMovement.Right:
                        currentPos.x += 1;
                        PC.Move(new Vector2(1, 0));
                        break;
                    case PossibleMovement.Left:
                        currentPos.x -= 1;
                        PC.Move(new Vector2(-1, 0));
                        break;
                    default:
                        break;
                }
                if (grid.isGoal(currentPos))
                {
                    victory = true;
                }
            }
        }

        private IEnumerator StepPolicy()
        {
            PlayerController PC = GameObject.Find("Player").GetComponent<PlayerController>();
            Vector2Int currentPos =
                new Vector2Int((int)grid.getSpawnPos().x, grid.Height - 1 - (int)grid.getSpawnPos().y);
            bool victory = false;
            while (!victory)
            {
                yield return new WaitForSeconds(0.5f);
                switch (_policyFunction[currentPos.x, currentPos.y])
                {
                    case PossibleMovement.Up:
                        currentPos.y -= 1;
                        PC.Move(new Vector2(0, 1));
                        break;
                    case PossibleMovement.Down:
                        currentPos.y += 1;
                        PC.Move(new Vector2(0, -1));
                        break;
                    case PossibleMovement.Right:
                        currentPos.x += 1;
                        PC.Move(new Vector2(1, 0));
                        break;
                    case PossibleMovement.Left:
                        currentPos.x -= 1;
                        PC.Move(new Vector2(-1, 0));
                        break;
                    default:
                        break;
                }
                if (grid.isGoal(currentPos))
                {
                    victory = true;
                }
            }
        }

        public PossibleMovement FindBestDirection(Vector2Int currentPos)
        {
            float bestValue = float.MinValue;
            PossibleMovement bestDirection = (PossibleMovement)UnityEngine.Random.Range(0, 4);

            if (grid.CanMove(PossibleMovement.Up, currentPos))
            {
                if (bestValue < _valueFunction[currentPos.x, currentPos.y - 1])
                {
                    bestValue = _valueFunction[currentPos.x, currentPos.y - 1];
                    bestDirection = PossibleMovement.Up;
                }
            }

            if (grid.CanMove(PossibleMovement.Down, currentPos))
            {
                if (bestValue < _valueFunction[currentPos.x, currentPos.y + 1])
                {
                    bestValue = _valueFunction[currentPos.x, currentPos.y + 1];
                    bestDirection = PossibleMovement.Down;
                }
            }

            if (grid.CanMove(PossibleMovement.Right, currentPos))
            {
                if (bestValue < _valueFunction[currentPos.x + 1, currentPos.y])
                {
                    bestValue = _valueFunction[currentPos.x + 1, currentPos.y];
                    bestDirection = PossibleMovement.Right;
                }
            }

            if (grid.CanMove(PossibleMovement.Left, currentPos))
            {
                if (bestValue < _valueFunction[currentPos.x - 1, currentPos.y])
                {
                    bestValue = _valueFunction[currentPos.x - 1, currentPos.y];
                    bestDirection = PossibleMovement.Left;
                }
            }

            return bestDirection;
        }

        public void DisplayValueGrid()
        {
            int k = 0;
            //print("VALUES");
            for (var y = 0; y < grid.Height; y++)
            {
                string output = "";
                for (var x = 0; x < grid.Width; x++)
                {
                    output += " " + _valueFunction[x, y].ToString("F2");
                    if (grid.EveryTiles[k].TryGetComponent(out CellDisplay cell))
                    {
                        cell.DisplayValue(_valueFunction[x, y].ToString("F2"));
                    }

                    k++;
                }

                //print(output);
            }
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
                    output += " " + _policyFunction[x, y];
                    if (grid.EveryTiles[k].TryGetComponent(out CellDisplay cell))
                    {
                        cell.DisplayValue(_policyFunction[x, y].ToString());
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
    }
}