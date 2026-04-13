using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Exit : MonoBehaviour
{
    public Button exit;

    // Start is called before the first frame update
    void Start()
    {
        Button b = exit.GetComponent<Button>();
        b.onClick.AddListener(exitGame);
    }

    void exitGame()
    {
        SceneManager.LoadScene("Start");
    }
}
