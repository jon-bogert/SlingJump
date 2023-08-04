using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct ScoreData
{
    public ulong score;
    public string date;
}

public class ScoreManager : MonoBehaviour
{
    const ushort SAVE_VER_NUM = 1;

    [SerializeField] bool _isSaving = true;
    [SerializeField] int _resetTimeHours = 2;

    public static ScoreManager instance;
    ScoreManager() { }

    public delegate void ScoreEvent(ulong score);
    public ScoreEvent scoreUpdated;

    ulong _score;
    ushort _lives = 15;
    bool _isConnectedToInternet = true;
    DateTime _lastTimeRefresh = new DateTime();
    DateTime _currentTime = new DateTime();
    ScoreData[] _highScores = new ScoreData[5];
    const string fileName = "/save.dat";

    public ushort lives { get { return _lives; } }
    public bool CanPlay { get { return _lives >= 0; } }
    public string TimeToGo { get { return (_isConnectedToInternet) ? (_lastTimeRefresh.AddHours(_resetTimeHours) - _currentTime).ToString().Substring(0, 8) : "-:--:--"; } }

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
            CheckDateTime();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            CheckDateTime();
        }
    }

    private void Update()
    {
        _currentTime = _currentTime.AddSeconds(Time.unscaledDeltaTime);
        if (_lastTimeRefresh.AddHours(_resetTimeHours) - _currentTime <= TimeSpan.Zero)
        {
            _lastTimeRefresh = _currentTime;
            ResetLives();
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
    /// Checks score against all in the high score array and inse
    /// rts it of appropriate
    /// </summary>
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

        if (insert != -1)
        {
            for (int i = _highScores.Length - 1; i > insert; --i)
            {
                _highScores[i] = _highScores[i - 1];
            }
            ScoreData newData = new ScoreData();
            newData.score = scoreToCheck;
            newData.date = DateTime.Now.ToString("yyyy/MM/dd");
            _highScores[insert] = newData;
        }
        SaveData();
    }

    void LoadData()
    {
        if (!_isSaving)
            return;

        if (!System.IO.File.Exists(Application.persistentDataPath + fileName)) // No Data Exists
            return;

        try
        {
            using (FileStream fstream = new FileStream(Application.persistentDataPath + fileName, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(fstream))
                {
                    if (reader.PeekChar() != '%')
                    {
                        LoadVer0(reader);
                        return;
                    }
                    reader.ReadChar();// throw away '%'

                    ushort saveVersion = reader.ReadUInt16();
                    if (saveVersion == 1)
                    {
                        LoadVer1(reader);
                    }
                    //Add More Save version as become relevent
                    else
                    {
                        Debug.LogError("ScoreManager -> Unrecognized Save Format");
                    }
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    void LoadVer0(BinaryReader reader)
    {
        int index = 0;
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
    void LoadVer1(BinaryReader reader)
    {
        _lastTimeRefresh = DateTime.FromBinary(reader.ReadInt64());
        _lives = reader.ReadUInt16();
        int index = 0;
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

    void SaveData()
    {
        if (!_isSaving)
            return;

        if (!System.IO.File.Exists(Application.persistentDataPath + fileName)) // create file if doesn't Exist
        {
            System.IO.File.Create(Application.persistentDataPath + fileName).Close();
        }
        try
        {
            using (FileStream fstream = new FileStream(Application.persistentDataPath + fileName, FileMode.Truncate))
            {
                using (BinaryWriter writer = new BinaryWriter(fstream))
                {
                    writer.Write('%');
                    writer.Write(SAVE_VER_NUM);
                    writer.Write(_lastTimeRefresh.ToBinary());
                    writer.Write(_lives);

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

    public void ResetAll()
    {
        _highScores = new ScoreData[5];
        SaveData();
    }
    public void ResetLives()
    {
        if (_lives < 15)
        {
            _lives = 15;
            SaveData();
        }
    }

    public void AddLives(ushort amt)
    {
        _lives += amt;
        SaveData();
    }

    public void RemoveLife()
    {
        _lives -= 1;
    }

    void CheckDateTime()
    {
        DateTimeResult result = DateTimeRequest();
        switch (result)
        {
            case DateTimeResult.NoConnection:
                //Toggle No Connection Screen
                _isConnectedToInternet = false;
                break;
            case DateTimeResult.NoReset:
                _isConnectedToInternet = true;
                break;
            case DateTimeResult.Reset:
                _isConnectedToInternet = true;
                ResetLives();
                break;
            default:
                Debug.LogWarning("No Support for DateTimeResult enum");
                break;
        }
    }

    private enum DateTimeResult { Reset, NoReset, NoConnection };
    DateTimeResult DateTimeRequest()
    {
        string url = $"https://worldtimeapi.org/api/timezone/Europe/London";
        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage response = httpClient.GetAsync(url).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                JObject json = JObject.Parse(content);
                _currentTime = DateTime.Parse((string)json["datetime"]);
            }
            else
            {
                return DateTimeResult.NoConnection;
            }
        }
        if (_lastTimeRefresh.AddHours(_resetTimeHours) < _currentTime)
        {
            _lastTimeRefresh = _currentTime;
            return DateTimeResult.Reset;
        }
        return DateTimeResult.NoReset;
    }
}
