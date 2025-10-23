using UnityEngine;

public class BrackeysSolution : MonoBehaviour
{
    public Rigidbody rb;
    public Camera cam;
    public float movementSpeed = 5f;
    public float mouseSensitivity = 0.1f;
    public float jumpForce = 5f;
    public int maxJumps = 2;
    private Vector2 _mouseInitialPos;
    private float _pitch;
    private int _remainingJumps;
    
    private void Awake()
    {
        Cursor.visible = false;
        ResetMousePos();
        ResetJumps();
    }
    
    private void Update()
    {
        HandleRotation();
        HandleJump();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("JumpSurface")))
        {
            ResetJumps();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ResetMousePos();
        }
    }
    
    private void HandleMovement()
    {
        Vector3 translation = Vector3.zero;
        
        if (Input.GetKey(KeyCode.A))
        {
            translation -= rb.transform.right;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            translation += rb.transform.right;
        }
        
        if (Input.GetKey(KeyCode.W))
        {
            translation += rb.transform.forward;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            translation -= rb.transform.forward;
        }
        
        rb.MovePosition(transform.position + translation.normalized * (movementSpeed * Time.fixedDeltaTime));
    }

    private void HandleRotation()
    {
        rb.angularVelocity = Vector3.zero;
        
        Vector2 currMousePos = Input.mousePosition;
        Vector2 delta = currMousePos - _mouseInitialPos;
        _mouseInitialPos = currMousePos;
        
        float yawDelta = delta.x * mouseSensitivity;
        float pitchDelta = delta.y * mouseSensitivity;
        
        _pitch = Mathf.Clamp(_pitch - pitchDelta, -90f, 90f);
        
        cam.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        rb.transform.Rotate(Vector3.up, yawDelta, Space.World);
    }

    private void ResetMousePos()
    {
        _mouseInitialPos = Input.mousePosition;
    }
    
    private void HandleJump()
    {
        Vector3 jumpVector = rb.transform.up; // Can also be Vector3.up for a global "up" direction.

        if (Input.GetKeyDown(KeyCode.Space) && _remainingJumps > 0)
        {
            _remainingJumps--;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(jumpVector * jumpForce, ForceMode.Impulse);
        }
    }

    private void ResetJumps()
    {
        _remainingJumps = maxJumps;
    }
}
