using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CodeMonkeySolution : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _mouseSensitivity = 3f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private int _maxJumps = 2;
    
    private Rigidbody _rb;
    private Camera _cam;
    private float _pitch;
    private float _yaw;
    private int _remainingJumps;
    private bool _skipFrame;
    private bool _skipFixedUpdate;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InitializeRotation();
        ResetJumps();
    }
    
    private void Start()
    {
        Input.ResetInputAxes();
        _skipFrame = true;
    }

    private void Update()
    {
        if (_skipFrame) {_skipFrame = false; return;}
        
        HandleRotationInput();
        HandleJump();
    }
    
    private void FixedUpdate()
    {
        if (_skipFixedUpdate) {_skipFixedUpdate = false; return;}
        
        HandleMovement();
        HandleRotation();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.layer.Equals(LayerMask.NameToLayer("JumpSurface"))) return;
        
        ResetJumps();
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Input.ResetInputAxes();
            _skipFrame = true;
            _skipFixedUpdate = true;
            return;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void HandleMovement()
    {
        float translationX = Input.GetAxisRaw("Horizontal");
        float translationZ = Input.GetAxisRaw("Vertical");

        Vector3 translation = transform.right * translationX + transform.forward * translationZ;
        _rb.MovePosition(transform.position + translation.normalized * (_movementSpeed * Time.fixedDeltaTime));
    }

    private void HandleRotationInput()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * _mouseSensitivity;
        
        _pitch = Mathf.Clamp(_pitch - mouseY, -90f, 90f);
        _yaw += mouseX;
    }

    private void HandleRotation()
    {
        _rb.angularVelocity = Vector3.zero;
        _cam.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        _rb.MoveRotation(Quaternion.Euler(0f, _yaw, 0f));
    }
    
    private void InitializeRotation()
    {
        _yaw = transform.eulerAngles.y;

        float angle = _cam.transform.localEulerAngles.x;
        if (angle > 180f) angle -= 360f;
        _pitch = Mathf.Clamp(angle, -90f, 90f);
    }
    
    private void HandleJump()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || _remainingJumps <= 0) return;
        
        _remainingJumps--;
        _rb.linearVelocity = Vector3.zero;
        Vector3 jumpVector = Vector3.up;
        _rb.AddForce(jumpVector * _jumpForce, ForceMode.Impulse);
    }
    
    private void ResetJumps()
    {
        _remainingJumps = _maxJumps;
    }
}
