using System;
using System.Collections;
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
                    _rewardFunction[i, j] = grid.GridCoordinate[j, i].rewardValue;
                    _valueFunction[i, j] = 0.0f;
                }
            }
        }

        public void RunAlgorithm()
        {
            InitAlgorithm();

            float DELTA = float.MaxValue;

            while (DELTA > THETA)
            {
                DELTA = 0.0f;
                for (var i = 0; i < grid.Width; i++)
                {
                    for (var j = 0; j < grid.Height; j++)
                    {
                        if(grid.GridCoordinate[j, i].value == (int)TileType.Player || grid.GridCoordinate[j, i].value == (int)TileType.Ground)
                        {
                            float oldValue = _valueFunction[i, j];

                            float maxValue = float.MinValue;
                            Vector2Int currentPosition = new Vector2Int(i, j);
                            foreach (PossibleMovement movement in Enum.GetValues(typeof(PossibleMovement)))
                            {
                                if (grid.CanMove(movement, currentPosition))
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

        public void PlayGame()
        {
            StartCoroutine(Step());
        }
        private IEnumerator Step()
        {
            PlayerController PC = GameObject.Find("Player").GetComponent<PlayerController>();
            Vector2Int currentPos = new Vector2Int((int)grid.getSpawnPos().x, grid.Height - 1 - (int)grid.getSpawnPos().y);
            bool victory = false;
            while (!victory)
            {
                yield return new WaitForSeconds(0.5f);
                if (grid.isGoalNear(currentPos))
                {
                    PC.Move(grid.goToGoal(currentPos));
                    victory = true;
                }
                else
                {
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
                }
            }
        }

        public PossibleMovement FindBestDirection(Vector2Int currentPos)
        {
            float bestValue = float.MinValue;
            PossibleMovement bestDirection = PossibleMovement.Up; //TODO Gérer Exception où on ne renvoie rien
            if (grid.CanMove(PossibleMovement.Up, currentPos))
            {
                if(bestValue< _valueFunction[currentPos.x, currentPos.y - 1])
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
            print("VALUES");
            for (var y = 0; y < grid.Height; y++)
            {
                string output = "";
                for (var x = 0; x < grid.Width; x++)
                {
                    output += " " + _valueFunction[x, y].ToString("F2");
                }
                print(output);
            }
        }


    }
}