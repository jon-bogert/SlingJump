using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSceneManager : MonoBehaviour
{
    public void RestartGame()
    {
        if (ScoreManager.instance.lives > 0)
        {
            ScoreManager.instance.score = 0;
            SceneManager.LoadScene("Game");
        }
    }

    public void MainMenu()
    {
        ScoreManager.instance.score = 0;
        SceneManager.LoadScene("MainMenu");
    }
}
