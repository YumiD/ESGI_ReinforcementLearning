using UnityEngine;

namespace Algos
{
    public class ValueIteration : MonoBehaviour, IAlgorithm
    {
        [SerializeField] private Grid2D grid;
        private float[,] _rewardFunction;
        private float[,] _valueFunction;
        public void InitAlgorithm()
        {
            _valueFunction = new float[grid.Width, grid.Height];
            _rewardFunction = new float[grid.Width, grid.Height];

            for (var i = 0; i < grid.Width; i++)
            {
                for (var j = 0; j < grid.Height; j++)
                {
                    _valueFunction[i, j] = 0.0f;
                    _rewardFunction[i, j] = grid.GridCoordinate[i, j].rewardValue;
                }
            }
        }

        public void RunAlgorithm()
        {
            throw new System.NotImplementedException();
        }
    }
}