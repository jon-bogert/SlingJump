using UnityEngine;

public class MovingSpike : MonoBehaviour
{
    [SerializeField] Transform[] _movePoints;
    [SerializeField] float _moveTime = 1f;
    [SerializeField] float _rotationSpeed = 1f;
    int _startIndex = 0;
    int _destIndex = 1;
    float _timer = 0f;

    float _timeFactor = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            player.GameOver();
        }
    }

    private void Awake()
    {
        _destIndex = (_startIndex + 1) % _movePoints.Length;
        _timeFactor = 1f/_moveTime;

        for (int i = 0; i < _movePoints.Length; i++)
        {
            _movePoints[i].parent = transform.parent;
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);

        if (_timer > _moveTime)
        {
            transform.position = _movePoints[_destIndex].position;
            _startIndex = _destIndex;
            _destIndex = (_startIndex + 1) % _movePoints.Length;
            _timer = 0f;
            return;
        }

        transform.position = Vector2.Lerp(_movePoints[_startIndex].position, _movePoints[_destIndex].position, _timer * _timeFactor);
        _timer += Time.deltaTime;
    }
}
