using UnityEngine;
using UnityEngine.InputSystem;

public class ResetScores : MonoBehaviour
{
    private void Awake()
    {
        Disable();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void ResetScore()
    {
        ScoreManager.instance.ResetAll();
        FindObjectOfType<GameOverScore>().UpdateHighScoreText();
    }
}
