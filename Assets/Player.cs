using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //Inspector
    [Header("Parameters")]
    [SerializeField] float _touchZoneRadius = 0.5f;
    [SerializeField] float _maxDragRadius = 1f;

    [Header("References")]
    [SerializeField] Transform _touchIndicator;

    [Header("Inputs")]
    [SerializeField] InputActionReference _inputDown;
    [SerializeField] InputActionReference _inputUp;
    [SerializeField] InputActionReference _inputPosition;

    //Data
    bool _isTouching = false;

    //Unity Calls
    private void Awake()
    {
        _inputDown.action.performed += OnTouch;
        _inputUp.action.performed += OnRelease;
    }
    void Start()
    {
        _touchIndicator.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_isTouching)
            return;

        Vector2 touchPos = Camera.main.ScreenToWorldPoint(_inputPosition.action.ReadValue<Vector2>());
        Vector2 playerPos = transform.position;
        Vector2 dragVector = touchPos - playerPos;

        // test if outside max drag distance and correct
        if (dragVector.sqrMagnitude > Mathf.Pow(_maxDragRadius, 2f))
        {
            dragVector = dragVector.normalized * _maxDragRadius;
            touchPos = playerPos + dragVector;
        }
        _touchIndicator.position = touchPos;
    }

    private void OnDestroy()
    {
        _inputDown.action.performed -= OnTouch;
        _inputUp.action.performed -= OnRelease;
    }

    //Callback Events
    void OnTouch(InputAction.CallbackContext ctx)
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(_inputPosition.action.ReadValue<Vector2>());
        Vector2 playerPos = transform.position;
        if ((pos - playerPos).sqrMagnitude > Mathf.Pow(_touchZoneRadius, 2f)) // Invalid touch
            return;

        _isTouching = true;
        _touchIndicator.gameObject.SetActive(true);
    }

    void OnRelease(InputAction.CallbackContext ctx)
    {
        if (!_isTouching)
            return;

        _touchIndicator.gameObject.SetActive(false);
    }

}
