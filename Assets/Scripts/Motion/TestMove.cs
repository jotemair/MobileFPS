using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TestMove : MonoBehaviour
{
    [SerializeField]
    private float _speed = 0.8f;
    
    private Rigidbody _rb = null;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        Vector3 keyboardInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        _rb.velocity += keyboardInput * _speed;
    }
}
