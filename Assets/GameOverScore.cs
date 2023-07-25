using UnityEngine;
using TMPro;

public class GameOverScore : MonoBehaviour
{
    [SerializeField] TMP_Text _currentScore;
    [SerializeField] TMP_Text _highScores;

    ScoreManager _scoreManager;
    // Start is called before the first frame update
    void Start()
    {
        _scoreManager = ScoreManager.instance;

        if (_currentScore)
            _currentScore.text = "You Got: " + _scoreManager.score.ToString();

        _highScores.text =
            "1.\t" + HighScoreStr(0) + "\n" +
            "2.\t" + HighScoreStr(1) + "\n" +
            "3.\t" + HighScoreStr(2) + "\n" +
            "4.\t" + HighScoreStr(3) + "\n" +
            "5.\t" + HighScoreStr(4);
    }

    string HighScoreStr(int index)
    {
        if (_scoreManager.highScores[index].score == 0)
            return "---";

        return _scoreManager.highScores[index].score.ToString() + "\t" + _scoreManager.highScores[index].date;
    }
}
