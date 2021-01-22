using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public IntVariable Score;

    private void Start()
    {
        Score.Value = 0;
    }
    public void IncrementScore()
    {
        Score.Value++;
    }

}
