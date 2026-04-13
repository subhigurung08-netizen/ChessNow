using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchScenes : MonoBehaviour
{
    public Button start;

    // Start is called before the first frame update
    void Start()
    {
        Button b = start.GetComponent<Button>();
        b.onClick.AddListener(startGame);
    }

    void startGame()
    {
        SceneManager.LoadScene("Main");
    }
}