using System;
using UnityEngine;

[System.Serializable]
public struct ScoreData
{
    public ulong score;
    public string date;
}

public class ScoreManager : MonoBehaviour
{
    public delegate void ScoreEvent(ulong score);

    public ScoreEvent scoreUpdated;

    ulong _score;
    ScoreData[] _highScores = new ScoreData[5];

    private void Awake()
    {
        //Unity-style Singleton
        if (FindObjectsOfType<ScoreManager>().Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(this);
    }

    public ulong score
    {
        get { return _score; }
        set
        {
            _score = value;
            scoreUpdated?.Invoke(_score);
        }
    }

    public ScoreData[] highScores { get { return _highScores; } }
    public ulong highScore { get { return _highScores[0].score; } }

    /// <summary>
    /// Checks score against all in the high score array and inserts it of appropriate
    /// </summary>
    /// <param name="scoreToCheck"></param>
    public void CheckScore(ulong scoreToCheck)
    {
        int insert = -1;
        for (int i = 0; i < _highScores.Length; i++)
        {
            if (scoreToCheck > _highScores[i].score)
            {
                insert = i;
                break;
            }
        }

        if (insert == -1) return;

        for (int i = _highScores.Length - 1; i > insert; --i)
        {
            _highScores[i] = _highScores[i - 1];
        }
        ScoreData newData = new ScoreData();
        newData.score = scoreToCheck;
        newData.date = DateTime.Now.ToString("yyyy/MM/dd");
        _highScores[insert] = newData;
    }
}
