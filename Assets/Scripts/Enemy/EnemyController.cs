using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GameObject _body = null;

    [SerializeField]
    private GameObject _head = null;

    [SerializeField]
    private float _speed = 0f;

    private Stabilizer _bodyStabilizer = null;

    private Stabilizer _headStabilizer = null;

    private DirectionalJoint _neck = null;

    // Start is called before the first frame update
    void Start()
    {
        _bodyStabilizer = _body.GetComponent<Stabilizer>();
        _headStabilizer = _head.GetComponent<Stabilizer>();
        _neck = _head.GetComponent<DirectionalJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_neck.Equals(null))
        {
            Destroy(_headStabilizer);
            _bodyStabilizer.Strength = -3f;
            _bodyStabilizer.BreakAngle = 30f;
        }
    }

    private void FixedUpdate()
    {
        Rigidbody rb = _body.GetComponent<Rigidbody>();
        //rb.velocity = rb.velocity + Vector3.forward * _speed;
        rb.AddForceAtPosition(Vector3.forward * _speed, rb.transform.position - new Vector3(0f, rb.transform.position.y / 3f, 0f));
        //rb.AddForce(Vector3.forward * _speed);
    }
}
