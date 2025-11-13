using UnityEngine;

public class ArkanoidPlayerControl : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _movementSpeed;
    private float _input;

    private void Update()
    {
        _input = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        Vector3 movement = new(_input, 0f, 0f);
        _rb.MovePosition(_rb.position + movement * (_movementSpeed * Time.fixedDeltaTime));
    }
}
