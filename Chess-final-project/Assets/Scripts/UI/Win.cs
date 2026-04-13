using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Win : MonoBehaviour
{
    public static Win instWin;
    private TMP_Text winText;
    
    private void Awake()
    {
        winText = GetComponent<TMP_Text>();
        winText.text = "";
        instWin = GetComponent<Win>();
    }

    // public void ScoreUpdate(ScoreContoller scoreController)
    public void ShowWinner(string winner)
    {
        winText.text = winner + " WINS";
    }
}
