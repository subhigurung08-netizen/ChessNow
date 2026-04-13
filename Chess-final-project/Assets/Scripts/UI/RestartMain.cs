using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartMain : MonoBehaviour
{
    public Button restart;

    // Start is called before the first frame update
    void Start()
    {
        Button b = restart.GetComponent<Button>();
        b.onClick.AddListener(restartGame);
    }

    void restartGame()
    {
        // SceneManager.LoadScene(GetActiveScene().name);
    }
}
