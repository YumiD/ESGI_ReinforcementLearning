using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellDisplay : MonoBehaviour
{
    [SerializeField] private Text value;
    [SerializeField] private Canvas canvas;

    public void DisplayValue(string val)
    {
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
        value.text = val;
    }
}