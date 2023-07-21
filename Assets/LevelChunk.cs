using UnityEngine;

public class LevelChunk : MonoBehaviour
{
    [SerializeField] float _height = 10;
    [SerializeField] bool _showDebugLine = true;
    Vector2 _storagePoint;
    CapturePoint[] _capturePoints;

    public float height { get { return _height; } }
    public float halfHeight { get { return _height * 0.5f; } }
    public Vector2 storagePoint { get { return _storagePoint; } }

    private void Awake()
    {
        _storagePoint = transform.position;
    }
    private void Start()
    {
        _capturePoints = GetComponentsInChildren<CapturePoint>();
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!_showDebugLine)
            return;

        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireCube(transform.position, new Vector2(5f, _height));
#endif
    }

    public void ResetPointFlags()
    {
        for (int i = 0; i < _capturePoints.Length; ++i)
        {
            _capturePoints[i].pointSpent = false;
        }
    }
}
