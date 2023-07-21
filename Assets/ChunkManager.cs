using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk References")]
    [SerializeField] LevelChunk _startChunk;
    [SerializeField] LevelChunk[] _chunkLibrary;

    [Header("Debug")]
    [SerializeField] bool _spawnSpecific = false;
    [SerializeField] int _debugChunkIndex = 0;

    LevelChunk _activeChunk = null;
    LevelChunk _nextChunk = null;
    LevelChunk _prevChunk = null;

    Player _player;

    int[] _chunkIndexHistory = new int[3];
    Vector2[] _storagePositions;
    Vector2 _startStoragePoint = Vector2.left * 10f;

    private void Awake()
    {
        _storagePositions = new Vector2[_chunkLibrary.Length];

        for (int i = 0; i < _chunkIndexHistory.Length; ++i)
        {
            _chunkIndexHistory[i] = -1;
        }
    }

    private void Start()
    {
        _player = FindObjectOfType<Player>();

        _activeChunk = _startChunk;

        if (_spawnSpecific)
        {
            _nextChunk = _chunkLibrary[_debugChunkIndex];
            PushIndex(_debugChunkIndex);
        }
        else
        {
            NextChunk();
        }
    }

    private void Update()
    {
        if (_player.transform.position.y > _nextChunk.transform.position.y - _nextChunk.halfHeight)
        {
            if (_prevChunk == _startChunk)
                _startChunk.transform.position = _startStoragePoint;
            else if (_prevChunk != null)
            {
                _prevChunk.transform.position = _prevChunk.storagePoint;
                _prevChunk.ResetPointFlags();
            }

            _prevChunk = _activeChunk;
            _activeChunk = _nextChunk;

            NextChunk();
            
        }
    }

    void NextChunk()
    {
        int next = 0;
        bool isUsed = true;
        while (isUsed)
        {
            isUsed = false;
            next = Random.Range(0, _chunkLibrary.Length);
            foreach (int i in _chunkIndexHistory)
                if (next == i)
                    isUsed = true;
        }
        PushIndex(next);

        _nextChunk = _chunkLibrary[next];

        _nextChunk.transform.position = _activeChunk.transform.position + (_activeChunk.halfHeight + _nextChunk.halfHeight) * Vector3.up;
    }

    void PushIndex(int index)
    {
        for (int i = 0; i < _chunkIndexHistory.Length - 1; ++i)
        {
            _chunkIndexHistory[i] = _chunkIndexHistory[i + 1];
        }

        _chunkIndexHistory[_chunkIndexHistory.Length - 1] = index;
    }
}
