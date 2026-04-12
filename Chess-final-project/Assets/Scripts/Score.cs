using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    private TMP_Text scoreText;
    private void Awake()
    {
        scoreText = GetComponent<TMP_Text>();
        scoreText.text = "Score: 0";
    }

    public void ScoreUpdate(ScoreContoller scoreController)
    {
        scoreText.text = $"Score: {scoreController.Score}";
    }
}
