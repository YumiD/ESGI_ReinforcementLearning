using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin
{
    public float _probability;
    public float _estimation = 0.0f;
    public float _numberThrows = 0.0f;

    public Coin(float probability)
    {
        _probability = probability;
    }

    public float getProbability()
    {
        return _probability;
    }

    public void updateCoin()
    {
        //TODO REPARER CETTE MERDE
        _numberThrows += 1.0f;
        _estimation = (1.0f - 1.0f / _numberThrows)*_estimation +1.0f / _numberThrows * _probability; //TODO wtf c'est quoi ce (1.0f - 1.0f / _numberThrows) qui résout tous les problèmes?
    }
}
