using UnityEngine;

public class ArkanoidBallControl : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Rigidbody _rb;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.linearVelocity = new Vector3(Random.value * _speed, Random.value * _speed, 0f);
    }
}
