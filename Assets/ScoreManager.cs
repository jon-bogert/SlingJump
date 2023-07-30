using System;
using System.ComponentModel.Design;
using System.IO;
using UnityEngine;

[System.Serializable]
public struct ScoreData
{
    public ulong score;
    public string date;
}

public class ScoreManager : MonoBehaviour
{
    [SerializeField] bool _isSaving = true;

    public static ScoreManager instance;

    ScoreManager() { }

    public delegate void ScoreEvent(ulong score);

    public ScoreEvent scoreUpdated;

    ulong _score;
    ScoreData[] _highScores = new ScoreData[5];
    const string fileName = "/save.dat";

    private void Awake()
    {
        //Unity-style Singleton
        if (instance)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            LoadData();
        }
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
            else if (scoreToCheck == _highScores[i].score) // Don't insert equal scores
            {
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
        SaveData();
    }

    void LoadData()
    {
        if (!_isSaving)
            return;

        if (!File.Exists(Application.persistentDataPath + fileName)) // No Data Exists
            return;

        try
        {
            int index = 0;
            using (FileStream fstream = new FileStream(Application.persistentDataPath + fileName, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(fstream))
                {
                    while (reader.PeekChar() != -1)
                    {
                        ScoreData data = new ScoreData();

                        data.score = reader.ReadUInt64();
                        ushort year = reader.ReadUInt16();
                        ushort month = reader.ReadUInt16();
                        ushort day = reader.ReadUInt16();
                        data.date = year.ToString("D4") + "-" + month.ToString("D2") + "-" + day.ToString("D2");

                        _highScores[index++] = data;
                    }
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    void SaveData()
    {
        if (!_isSaving)
            return;

        if (!File.Exists(Application.persistentDataPath + fileName)) // create file if doesn't Exist
        {
            File.Create(Application.persistentDataPath + fileName).Close();
        }
        try
        {
            using (FileStream fstream = new FileStream(Application.persistentDataPath + fileName, FileMode.Truncate))
            {
                using (BinaryWriter writer = new BinaryWriter(fstream))
                {
                    foreach (ScoreData data in _highScores)
                    {
                        if (data.score == 0) // don't write if no score
                            continue;

                        writer.Write(data.score);
                        writer.Write(ushort.Parse(data.date.Substring(0, 4)));
                        writer.Write(ushort.Parse(data.date.Substring(5, 2)));
                        writer.Write(ushort.Parse(data.date.Substring(8, 2)));
                    }
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

}
