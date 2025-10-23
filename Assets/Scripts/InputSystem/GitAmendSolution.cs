using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class GitAmendSolution : MonoBehaviour
{
    [Header("Actions")]
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _lookAction;
    [SerializeField] private InputActionReference _jumpAction;
    
    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _mouseSensitivity = 0.1f; // mouse delta (deg/px)
    [SerializeField] private float _gamepadSensitivity = 180f; // max stick angle (deg/s * theta_max)
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private uint _maxJumps = 2;
    
    [Header("Miscellaneous")]
    [SerializeField] private LayerMask _jumpResetLayer;
    
    private InputAction _move;
    private InputAction _look;
    private InputAction _jump;
    private Rigidbody _rb;
    private PlayerInput _playerInput;
    private Camera _cam;
    private Vector2 _moveInput;
    private Vector2 _gamepadLookRate;
    private float _yaw;
    private float _pitch;
    private bool _jumpQueued;
    private uint _remainingJumps;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _cam = GetComponentInChildren<Camera>(includeInactive: true);
        
        InputActionAsset inputActions = _playerInput.actions;
        _move = inputActions.FindAction(_moveAction.action.id);
        _look = inputActions.FindAction(_lookAction.action.id);
        _jump = inputActions.FindAction(_jumpAction.action.id);
        
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        ResetJumps();
    }
    
    private void OnEnable()
    {
        _move.performed += OnMovePerformed;
        _move.canceled += OnMoveCanceled;
        _look.performed += OnLookPerformed;
        _look.canceled += OnLookCanceled;
        _jump.performed += OnJumpPerformed;
    }

    private void Update()
    {
        if (_gamepadLookRate == Vector2.zero) return;
        
        _yaw += _gamepadLookRate.x * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch - _gamepadLookRate.y * Time.deltaTime, -90f, 90f);
    }
    
    private void FixedUpdate()
    {
        _rb.angularVelocity = Vector3.zero;
        _rb.MoveRotation(Quaternion.Euler(0f, _yaw, 0f));
        _cam.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        
        Vector3 translation = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        _rb.MovePosition(_rb.position + translation * (_movementSpeed * Time.fixedDeltaTime));

        if (!_jumpQueued || _remainingJumps <= 0) return;
        
        _jumpQueued = false;
        _remainingJumps--;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if ((_jumpResetLayer.value & (1 << other.gameObject.layer)) == 0) return;
        
        ResetJumps();
    }
    
    private void OnDisable()
    {
        _move.performed -= OnMovePerformed;
        _move.canceled -= OnMoveCanceled;
        _look.performed -= OnLookPerformed;
        _look.canceled -= OnLookCanceled;
        _jump.performed -= OnJumpPerformed;
    }
    
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        _moveInput = value.magnitude <= 1f ? value : value.normalized;
    }
    
    private void OnMoveCanceled(InputAction.CallbackContext context) => _moveInput = Vector2.zero;
    
    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        switch (context.control?.device)
        {
            case Mouse:
                _yaw += value.x * _mouseSensitivity;
                _pitch = Mathf.Clamp(_pitch - value.y * _mouseSensitivity, -90f, 90f);
                break;
            case Gamepad:
                _gamepadLookRate = value * _gamepadSensitivity;
                break;
            default:
                Debug.Log($"Unknown controller device: {context.control?.device}");
                break;
        }
    }
    
    private void OnLookCanceled(InputAction.CallbackContext context) => _gamepadLookRate = Vector2.zero;

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (_remainingJumps <= 0) return;
        
        _jumpQueued = true;
    }
    
    private void ResetJumps()
    {
        _remainingJumps = _maxJumps;
    }
}
