using System;

[Serializable]
public class Cell
{
    public int value;
    public float rewardValue;

    public Cell(int value, float reward)
    {
        this.value = value;
        this.rewardValue = reward;
    }
}