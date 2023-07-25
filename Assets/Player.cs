using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    //Inspector
    [Header("Parameters")]
    [SerializeField] float _touchZoneRadius = 0.5f;
    [SerializeField] float _maxDragRadius = 1f;
    [SerializeField] float _maxShotForce = 100f;
    [SerializeField] float _worldHalfWidth = 1f;
    [SerializeField] float _velocityLineScale = 1f;
    [SerializeField] float _velocityLineSmoothFactor = 0.5f;

    [Header("References")]
    [SerializeField] Transform _touchIndicator;
    [SerializeField] LineRenderer _touchLineRenderer;
    [SerializeField] LineRenderer _velocityLineRenderer;

    [Header("Inputs")]
    [SerializeField] InputActionReference _inputDown;
    [SerializeField] InputActionReference _inputUp;
    [SerializeField] InputActionReference _inputPosition;

    //Data
    bool _isTouching = false;
    bool _canGrab = true;
    public bool canGrab { get { return _canGrab; } set { _canGrab = value; } }

    Vector2 _dragVector = Vector2.zero;

    //References
    Rigidbody2D _rigidbody;
    ScoreManager _scoreManager;

    public bool PhysicsActive
    {
        get
        {
            return _rigidbody.bodyType == RigidbodyType2D.Dynamic;
        }
        set
        {
            _rigidbody.bodyType = (value) ?
                RigidbodyType2D.Dynamic :
                RigidbodyType2D.Kinematic ;
            if (!value)
                _rigidbody.velocity = Vector2.zero;
        }
    }

    //Unity Calls
    private void Awake()
    {
        Application.targetFrameRate = 60;

        _inputDown.action.performed += OnTouch;
        _inputUp.action.performed += OnRelease;

        if (!_touchLineRenderer)
            Debug.LogError("Player -> Could not find touch LineRenderer");

        if (!_velocityLineRenderer)
            Debug.LogError("Player -> Could not find velocity LineRenderer");

        _rigidbody = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        _touchIndicator.gameObject.SetActive(false);
        _touchLineRenderer.gameObject.SetActive(false);
        _scoreManager = ScoreManager.instance;
    }

    private void Update()
    {
        UpdateVelocityLine();

        if (!_isTouching)
            return;

        Vector2 touchPos = Camera.main.ScreenToWorldPoint(_inputPosition.action.ReadValue<Vector2>());
        Vector2 playerPos = transform.position;
        _dragVector = touchPos - playerPos;

        // test if outside max drag distance and correct
        if (_dragVector.sqrMagnitude > Mathf.Pow(_maxDragRadius, 2f))
        {
            _dragVector = _dragVector.normalized * _maxDragRadius;
            touchPos = playerPos + _dragVector;
        }
        _touchIndicator.position = touchPos;
        UpdateTouchLine();
    }

    private void FixedUpdate()
    {
        if (_rigidbody.position.x < -_worldHalfWidth)
        {
            _rigidbody.position = new Vector2(_rigidbody.position.x + _worldHalfWidth * 2f, _rigidbody.position.y);
            Vector2 trailPos = _velocityLineRenderer.GetPosition(1);
            _velocityLineRenderer.SetPosition(1, trailPos + _worldHalfWidth * 2f * Vector2.right);
        }
        else if (_rigidbody.position.x > _worldHalfWidth)
        {
            _rigidbody.position = new Vector2(_rigidbody.position.x - _worldHalfWidth * 2f, _rigidbody.position.y);
            Vector2 trailPos = _velocityLineRenderer.GetPosition(1);
            _velocityLineRenderer.SetPosition(1, trailPos + _worldHalfWidth * 2f * Vector2.left);
        }
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.cyan;
        Vector2 pos = transform.position;
        UnityEditor.Handles.DrawLine(pos + _worldHalfWidth * Vector2.left, pos + _worldHalfWidth * Vector2.right);
#endif
    }

    private void OnDestroy()
    {
        _inputDown.action.performed -= OnTouch;
        _inputUp.action.performed -= OnRelease;
    }

    //Methods

    void UpdateVelocityLine()
    {
        Vector2 pos = _rigidbody.position;
        Vector2 desiredPos = pos - _rigidbody.velocity * _velocityLineScale;
        Vector2 accel = Vector2.zero; // throwaway var
        Vector2 finalPos = Vector2.SmoothDamp(_velocityLineRenderer.GetPosition(1), desiredPos, ref accel, _velocityLineSmoothFactor);
        _velocityLineRenderer.SetPosition(0, pos);
        _velocityLineRenderer.SetPosition(1, finalPos);
    }

    void UpdateTouchLine()
    {
        _touchLineRenderer.SetPosition(0, transform.position);
        _touchLineRenderer.SetPosition(1, _touchIndicator.position);
    }

    public void GameOver()
    {
        _scoreManager.CheckScore(_scoreManager.score);
        SceneManager.LoadScene("GameOver");
    }

    public void TeleportY(float amt)
    {
        _rigidbody.position = new Vector2(_rigidbody.position.x, _rigidbody.position.y + amt);
        Vector2 trailPos = _velocityLineRenderer.GetPosition(1);
        _velocityLineRenderer.SetPosition(1, trailPos + amt * Vector2.up);
    }

    //Callback Events
    void OnTouch(InputAction.CallbackContext ctx)
    {
        if (!_canGrab) // Check if ball is at grab point
            return;

        Vector2 pos = Camera.main.ScreenToWorldPoint(_inputPosition.action.ReadValue<Vector2>());
        Vector2 playerPos = transform.position;
        if ((pos - playerPos).sqrMagnitude > Mathf.Pow(_touchZoneRadius, 2f)) // Invalid touch
            return;

        _isTouching = true;
        _touchIndicator.gameObject.SetActive(true);
        _touchLineRenderer.gameObject.SetActive(true);
        UpdateTouchLine();

    }

    void OnRelease(InputAction.CallbackContext ctx)
    {
        if (!_isTouching)
            return;

        _touchIndicator.gameObject.SetActive(false);
        _touchLineRenderer.gameObject.SetActive(false);

        Vector2 forceNormal = -_dragVector.normalized;
        float forceMagnitude = (_dragVector.magnitude / _maxDragRadius) * _maxShotForce;
        PhysicsActive = true;
        _rigidbody.AddForce(forceNormal * forceMagnitude);

        _isTouching = false;
        _canGrab = false;
    }

}
