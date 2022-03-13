using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algos
{
    public class MonteCarlo : MonoBehaviour, IAlgorithm
    {
        [SerializeField] private Grid2D grid;
        [SerializeField] private int _INSTANCES_MAX = 50;
        public void InitGrid()
        {
            grid.RestartGrid(); //TODO Better Restart
        }
        public void InitAlgorithm()
        {
            throw new NotImplementedException();
        }

        public void MonteCarloESAlgorithm()
        {
            throw new NotImplementedException();
        }

        public void PlayGame()
        {
            throw new NotImplementedException();
        }

        public void RunAlgorithm()
        {
            throw new NotImplementedException();
        }
    }
}
