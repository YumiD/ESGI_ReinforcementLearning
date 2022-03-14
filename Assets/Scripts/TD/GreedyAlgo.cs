using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyAlgo : MonoBehaviour
{
    int _currency = 100;
    int CURRENCY_GOAL = 10000;
    float EPSILON = 0.3f;
    List<Coin> _coins = new List<Coin>();

    void Start()
    {
        _coins.Add(new Coin(0.25f));
        _coins.Add(new Coin(0.5f));
        _coins.Add(new Coin(0.75f));
        runExperiment();
    }

    void runExperiment()
    {
        while (_currency > 0 && _currency < CURRENCY_GOAL)
        {
            //Epsilon Greedy
            int rCoin;
            float p = Random.Range(0.0f, 1.0f);
            if (p < EPSILON)
                rCoin = Random.Range(0, _coins.Count);
            else
                rCoin = getCurrentBestCoin();
            _coins[rCoin].updateCoin();

            float rProba = Random.Range(0.0f, 1.0f);
            bool heads = rProba < _coins[rCoin].getProbability() ? true : false;
            _currency--;
            if (heads)
                _currency += 2;
        }

        if (_currency >= CURRENCY_GOAL)
            print("Vous êtes à présent riche");
        else
            print("mdrrrrrrr perdu");

        displayEstimations();
    }

    int getCurrentBestCoin()
    {
        float currentBest = _coins[0]._estimation;
        int currentBestIndex = 0;

        for(int i = 1; i<_coins.Count; i++)
        {
            if (currentBest < _coins[i]._estimation)
            {
                currentBestIndex = i;
                currentBest = _coins[i]._estimation;
            }
        }

        return currentBestIndex;
    }

    void displayEstimations()
    {
        print("ESTIMATIONS : ");
        foreach (Coin c in _coins)
        {
            print(c._estimation);
        }
    }

    /*MonteCarlo
     * 
     * Format de la policy : lookup table où pour chaque state on va définir la meilleure action possible
     * On initialize tout en une action random
     * 
     * 
     */
}
