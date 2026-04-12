using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreContoller : MonoBehaviour
{
    public int Score {get; private set; }
    public UnityEvent scoreChange;

    public void AddScore(int val)
    {
        Score += val;
        scoreChange.Invoke();
    }
}
