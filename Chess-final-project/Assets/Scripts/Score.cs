using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public static Score instan;
    private TMP_Text scoreText;
    
    private void Awake()
    {
        scoreText = GetComponent<TMP_Text>();
        scoreText.text = "Score: 0";
        instan = GetComponent<Score>();
    }

    // public void ScoreUpdate(ScoreContoller scoreController)
    public void ScoreUpdate(float score)
    {
        float playerScore = score * -1;
        scoreText.text = $"Score: {playerScore}";
    }
}
