using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    //Inspector
    [Header("Parameters")]
    [SerializeField] float _touchZoneRadius = 0.5f;
    [SerializeField] float _maxDragRadius = 1f;
    [SerializeField] float _maxShotForce = 100f;

    [Header("References")]
    [SerializeField] Transform _touchIndicator;

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
    LineRenderer _lineRenderer;
    Rigidbody2D _rigidbody;

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
        _inputDown.action.performed += OnTouch;
        _inputUp.action.performed += OnRelease;

        _lineRenderer = GetComponentInChildren<LineRenderer>();
        if (!_lineRenderer)
            Debug.LogError("Player -> Could not find LineRenderer in Children");

        _rigidbody = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        _touchIndicator.gameObject.SetActive(false);
        _lineRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
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
        UpdateLine();
    }

    private void OnDestroy()
    {
        _inputDown.action.performed -= OnTouch;
        _inputUp.action.performed -= OnRelease;
    }

    //Methods

    void UpdateLine()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _touchIndicator.position);
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
        _lineRenderer.gameObject.SetActive(true);
        UpdateLine();

    }

    void OnRelease(InputAction.CallbackContext ctx)
    {
        if (!_isTouching)
            return;

        _touchIndicator.gameObject.SetActive(false);
        _lineRenderer.gameObject.SetActive(false);

        Vector2 forceNormal = -_dragVector.normalized;
        float forceMagnitude = (_dragVector.magnitude / _maxDragRadius) * _maxShotForce;

        _rigidbody.AddForce(forceNormal * forceMagnitude);

        _isTouching = false;
        _canGrab = false;
    }

}
