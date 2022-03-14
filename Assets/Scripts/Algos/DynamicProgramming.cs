using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algos
{
    public class DynamicProgramming : MonoBehaviour, IAlgorithm
    {
        [SerializeField] private Grid2D grid;
        [SerializeField] private int _ITERATIONS_MAX = 50;

        private float[,] _rewardGrid;
        private float[,] _valueGrid;
        private PossibleMovement[,] _policyGrid;

        private const float GAMMA = 0.9f;

        public enum DynamicAlgos
        {
            Value,
            Policy
        };

        private DynamicAlgos currentDynamicAlgo;


        public void InitAlgorithm()
        {
            _rewardGrid = new float[grid.Width, grid.Height];
            _valueGrid = new float[grid.Width, grid.Height];
            _policyGrid = new PossibleMovement[grid.Width, grid.Height];

            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    _rewardGrid[i, j] = grid.GridCoordinate[j, i].rewardValue;
                    _valueGrid[i, j] = 0.0f;
                    _policyGrid[i, j] = (PossibleMovement)UnityEngine.Random.Range(0, 4);
                }
            }
        }

        public void ValueIterationAlgorithm()
        {
            InitAlgorithm();
            currentDynamicAlgo = DynamicAlgos.Value;

            int iteration = 0;
            while (iteration < _ITERATIONS_MAX)
            {
                for (var i = 0; i < grid.Width; i++)
                {
                    for (var j = 0; j < grid.Height; j++)
                    {
                        if(grid.CanStateAct(i, j))
                        {
                            float maxActionValue = float.MinValue;
                            Vector2Int currentPosition = new Vector2Int(i, j);
                            foreach (PossibleMovement movement in Enum.GetValues(typeof(PossibleMovement)))
                            {
                                if (grid.CanMove(movement, currentPosition))
                                {
                                    Vector2Int newPosition = grid.GetDeplacementPosition(movement, currentPosition);
                                    float value = _rewardGrid[newPosition.x, newPosition.y] +
                                                  GAMMA * _valueGrid[newPosition.x, newPosition.y];
                                    if (value > maxActionValue)
                                        maxActionValue = value;
                                }
                            }
                            if (maxActionValue != float.MinValue)
                                _valueGrid[i, j] = maxActionValue;
                        }
                    }
                }

                iteration++;
            }
            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    if (grid.isStateEnd(i, j))
                    {
                       _valueGrid[i, j] = _rewardGrid[i, j];
                    }
                }
            }

            DisplayValueGrid();
        }

        public void PolicyIterationAlgorithm()
        {
            InitAlgorithm();
            currentDynamicAlgo = DynamicAlgos.Policy;

            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    if (grid.CanStateAct(i, j))
                    {
                        float maxActionValue = float.MinValue;
                        Vector2Int currentPosition = new Vector2Int(i, j);
                        if (grid.CanMove(_policyGrid[i, j], currentPosition))
                        {
                            Vector2Int newPosition = grid.GetDeplacementPosition(_policyGrid[i, j], currentPosition);
                            float value = _rewardGrid[newPosition.x, newPosition.y] +
                                            GAMMA * _valueGrid[newPosition.x, newPosition.y];
                            if (value > maxActionValue)
                                maxActionValue = value;
                        }
                        if (maxActionValue != float.MinValue)
                            _valueGrid[i, j] = maxActionValue;
                    }
                }
            }
            PolicyImprovement();

            DisplayPolicyGrid();
        }

        public void PolicyImprovement()
        {
            bool stable = true;
            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    if (grid.CanStateAct(i, j))
                    {
                        PossibleMovement tempPolicy = _policyGrid[i, j];
                        float maxActionValue = float.MinValue;
                        Vector2Int currentPosition = new Vector2Int(i, j);
                        foreach (PossibleMovement movement in Enum.GetValues(typeof(PossibleMovement)))
                        {
                            if (grid.CanMove(movement, currentPosition))
                            {
                                Vector2Int newPosition = grid.GetDeplacementPosition(movement, currentPosition);
                                float value = _rewardGrid[newPosition.x, newPosition.y] +
                                                GAMMA * _valueGrid[newPosition.x, newPosition.y];
                                if (value > maxActionValue)
                                {
                                    maxActionValue = value;
                                    _policyGrid[i, j] = movement;
                                }
                            }
                        }
                        if (maxActionValue != float.MinValue)
                            _valueGrid[i, j] = maxActionValue;

                        if (_policyGrid[i, j] != tempPolicy)
                            stable = false;
                    }
                }
            }

            if (!stable)
                PolicyImprovement();
        }

        public void PlayGame()
        {
            print(currentDynamicAlgo);
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
                new Vector2Int((int)grid.getSpawnPos().y, grid.Width - 1 - (int)grid.getSpawnPos().x);
            bool victory = false;
            while (!victory)
            {
                print(_policyGrid[currentPos.x, currentPos.y]);
                yield return new WaitForSeconds(0.5f);
                switch (_policyGrid[currentPos.x, currentPos.y])
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
                if (bestValue < _valueGrid[currentPos.x, currentPos.y - 1])
                {
                    bestValue = _valueGrid[currentPos.x, currentPos.y - 1];
                    bestDirection = PossibleMovement.Up;
                }
            }

            if (grid.CanMove(PossibleMovement.Down, currentPos))
            {
                if (bestValue < _valueGrid[currentPos.x, currentPos.y + 1])
                {
                    bestValue = _valueGrid[currentPos.x, currentPos.y + 1];
                    bestDirection = PossibleMovement.Down;
                }
            }

            if (grid.CanMove(PossibleMovement.Right, currentPos))
            {
                if (bestValue < _valueGrid[currentPos.x + 1, currentPos.y])
                {
                    bestValue = _valueGrid[currentPos.x + 1, currentPos.y];
                    bestDirection = PossibleMovement.Right;
                }
            }

            if (grid.CanMove(PossibleMovement.Left, currentPos))
            {
                if (bestValue < _valueGrid[currentPos.x - 1, currentPos.y])
                {
                    bestValue = _valueGrid[currentPos.x - 1, currentPos.y];
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
                    output += " " + _valueGrid[x, y].ToString("F2");
                    if (grid.EveryTiles[k].TryGetComponent(out CellDisplay cell))
                    {
                        cell.DisplayValue(_valueGrid[x, y].ToString("F2"));
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
            grid.RestartGrid();
        }
    }
}