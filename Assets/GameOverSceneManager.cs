using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSceneManager : MonoBehaviour
{
    public void RestartGame()
    {
        ScoreManager.instance.score = 0;
        SceneManager.LoadScene("Game");
    }

    public void MainMenu()
    {
        ScoreManager.instance.score = 0;
        SceneManager.LoadScene("MainMenu");
    }
}
