using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    TMP_Text _textField;
    ScoreManager _scoreManager;

    void OnScoreUpdate(ulong newScore)
    {
        string highScore = (_scoreManager.highScore == 0) ? "-" : _scoreManager.highScore.ToString(); 
        _textField.text = "Score: " + newScore.ToString() + "\nHigh Score: " + highScore;
    }

    private void Awake()
    {
        _textField = GetComponent<TMP_Text>();
        if (!_textField)
            Debug.LogError("ScoreText -> Add Text Component");
    }

    private void Start()
    {
        _scoreManager = ScoreManager.instance;
        _scoreManager.scoreUpdated += OnScoreUpdate;
        OnScoreUpdate(0);
    }

    private void OnDestroy()
    {
        _scoreManager.scoreUpdated -= OnScoreUpdate;
    }
}