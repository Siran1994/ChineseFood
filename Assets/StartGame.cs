using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    void Start()
    {
        Invoke("Startgame",0.2f);
    }

    void Startgame()
    {
        SceneManager.LoadScene(1);
    }
}
