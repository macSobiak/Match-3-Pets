using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreVisualizer : MonoBehaviour
{
    public Text Text;
    public IntVariable ScoreCounter;
    private int previousCount = -1;

    private void OnEnable()
    {
        UpdateText();
    }

    private void Update()
    {
        if (previousCount != ScoreCounter.Value)
        {
            UpdateText();
            previousCount = ScoreCounter.Value;
        }
    }

    public void UpdateText()
    {
        Text.text = "Score: " + ScoreCounter.Value;
    }
}
