using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algos
{
    public class MonteCarlo : MonoBehaviour, IAlgorithm
    {
        [SerializeField] private Grid2D grid;
        [SerializeField] private int _ITERATIONS_MAX = 100;

        private float[,] _rewardGrid;
        private PossibleMovement[,] _policyGrid;
        private Dictionary<(Vector2Int, PossibleMovement), float> _QGrid;
        private Dictionary<(Vector2Int, PossibleMovement), List<float>> _ReturnsGrid;

        private const float GAMMA = 0.9f;
        private const float EPSILON = 0.2f;

        public void InitAlgorithm()
        {
            _rewardGrid = new float[grid.Width, grid.Height];
            _policyGrid = new PossibleMovement[grid.Width, grid.Height];
            _QGrid = new Dictionary<(Vector2Int, PossibleMovement), float>();
            _ReturnsGrid = new Dictionary<(Vector2Int, PossibleMovement), List<float>>();

            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    _rewardGrid[i, j] = grid.GridCoordinate[j, i].rewardValue;
                    Vector2Int pos = new Vector2Int(i, j);
                    foreach (PossibleMovement action in grid.getPossibleActions(pos))
                    {
                        _QGrid.Add((pos, action), 0);
                        _ReturnsGrid.Add((pos, action), new List<float>());
                    }
                    _policyGrid[i, j] = (PossibleMovement)UnityEngine.Random.Range(0, 4);
                }
            }
        }

        public void MonteCarloESAlgorithm()
        {
            InitAlgorithm();

            for(int iteration = 0; iteration<_ITERATIONS_MAX; iteration++)
            {
                //On génère un épisode
                List<(Vector2Int, PossibleMovement, float)>  stateActionsReturns = GenerateEpisode();

                //On calcule Q(s,a)
                List<(Vector2Int, PossibleMovement)> seenStateActionPairs = new List<(Vector2Int, PossibleMovement)>();
                foreach((Vector2Int, PossibleMovement, float) stateActionReturn in stateActionsReturns)
                {
                    (Vector2Int, PossibleMovement) stateAction = (stateActionReturn.Item1, stateActionReturn.Item2);
                    if (!seenStateActionPairs.Contains(stateAction)) //On vérifie si on a pas déjà vu ce stateAction
                    {
                        //Ajouter G dans _ReturnsGrid[stateAction]
                        //Q[s][a] = Moyenne des G de _ReturnsGrid[stateAction]
                        seenStateActionPairs.Add(stateAction);
                    }
                }

                //Remplir _policyGrid en fonction de la meilleure action par state
                for (var i = 0; i < grid.Width; i++)
                {
                    for (var j = 0; j < grid.Height; j++)
                    {
                        _policyGrid[i, j] = FindBestPolicy(new Vector2Int(i, j));
                    }
                }
            }


            //Afficher Policy
            DisplayPolicyGrid();
        }

        public List<(Vector2Int, PossibleMovement, float)> GenerateEpisode()
        {
            //Sélectionner point de départ

            //while(True) // On va jouer jusqu'à trouver la fin

            return new List<(Vector2Int, PossibleMovement, float)>();
        }

        public PossibleMovement EpsilonGreedy(Vector2Int pos)
        {
            PossibleMovement movement = (PossibleMovement)UnityEngine.Random.Range(0, 4);
            float p = UnityEngine.Random.Range(0.0f, 1.0f);
            if (p < EPSILON)
            {
                while (!grid.CanMove(movement, pos))
                    movement = (PossibleMovement)UnityEngine.Random.Range(0, 4);
                return movement;
            }
            movement = _policyGrid[pos.x, pos.y];
            return movement;
        }

        public PossibleMovement FindBestPolicy(Vector2Int currentPos)
        {
            float bestValue = float.MinValue;
            PossibleMovement bestDirection = (PossibleMovement)UnityEngine.Random.Range(0, 4);

            foreach(PossibleMovement movement in Enum.GetValues(typeof(PossibleMovement)))
            {
                if(_QGrid.ContainsKey((currentPos, movement)))
                {
                    if (bestValue < _QGrid[(currentPos, movement)] )
                    {
                        bestValue = _QGrid[(currentPos, movement)];
                        bestDirection = movement;
                    }
                }
            }

            return bestDirection;
        }

        public void PlayGame()
        {
            throw new NotImplementedException();
        }

        public void DisplayPolicyGrid()
        {
            int k = 0;
            //
            //("POLICY");
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
