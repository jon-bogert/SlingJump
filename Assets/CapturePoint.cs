using UnityEngine;
using UnityEngine.Rendering;

public class CapturePoint : MonoBehaviour
{
    [SerializeField] float _captureTime = 0.1f;

    Player _capturedPlayer = null;
    ScoreManager _scoreManager;

    bool _isLerping = false;
    float _lerpTimer = 0f;
    float _lerpTimerMultiplier = 1f;
    Vector2 _lerpStart = Vector2.zero;

    public bool pointSpent { get; set; } = false;

    private void Start()
    {
        _scoreManager = ScoreManager.instance;
        _lerpTimerMultiplier = 1f / _captureTime;
    }

    private void Update()
    {
        if (!_isLerping)
            return;

        if (_lerpTimer < _captureTime)
        {
            _capturedPlayer.transform.position = Vector2.Lerp(_lerpStart, transform.position, _lerpTimer * _lerpTimerMultiplier);
            _lerpTimer += Time.deltaTime;
        }
        else
        {
            _isLerping = false;
            _capturedPlayer.transform.position = transform.position;
            _capturedPlayer.canGrab = true;
            //_capturedPlayer.PhysicsActive = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (!player) // This is not a player
            return;

        _capturedPlayer = player;
        _capturedPlayer.PhysicsActive = false;
        _isLerping = true;
        _lerpStart = _capturedPlayer.transform.position;
        _lerpTimer = 0f;

        if (!pointSpent)
        {
            _scoreManager.score++;
            pointSpent = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == _capturedPlayer)
            _capturedPlayer = null;
    }
}
