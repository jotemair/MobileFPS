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
    private float _force = 0f;

    [SerializeField]
    private float _pushPointRatio = 0f;

    private Stabilizer _bodyStabilizer = null;
    private Stabilizer _bodyLookStabilizer = null;

    private Stabilizer _headStabilizer = null;
    private Stabilizer _headLookStabilizer = null;

    private DirectionalJoint _neck = null;

    [SerializeField]
    private GameObject _target = null;

    private Vector3 _directionVector = Vector3.zero;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(_body.transform.TransformPoint(new Vector3(0f, _pushPointRatio, 0f)), _body.transform.TransformPoint(new Vector3(0f, _pushPointRatio, 0f)) + _directionVector);
    }

    // Start is called before the first frame update
    void Start()
    {
        var bodyStabilizers = _body.GetComponents<Stabilizer>();
        _bodyStabilizer = ((StabilizationAxes.Z == bodyStabilizers[0].Axis) ? bodyStabilizers[1] : bodyStabilizers[0]);
        _bodyLookStabilizer = ((StabilizationAxes.Z == bodyStabilizers[0].Axis) ? bodyStabilizers[0] : bodyStabilizers[1]);

        var headStabilizers = _head.GetComponents<Stabilizer>();
        _headStabilizer = ((StabilizationAxes.Z == headStabilizers[0].Axis) ? headStabilizers[1] : headStabilizers[0]);
        _headLookStabilizer = ((StabilizationAxes.Z == headStabilizers[0].Axis) ? headStabilizers[0] : headStabilizers[1]);

        _neck = _head.GetComponent<DirectionalJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (null == _neck)
        {
            Destroy(_headStabilizer);
            Destroy(_headLookStabilizer);
            Destroy(_bodyLookStabilizer);
            _bodyStabilizer.Strength = -3f;
            _bodyStabilizer.BreakAngle = 25f;
        }
        else if (null != _target)
        {
            _directionVector = (_target.transform.position - _body.transform.position);
            _directionVector = new Vector3(_directionVector.x, 0f, _directionVector.z).normalized;
            _bodyLookStabilizer.StabilizationDirection = _directionVector;
            _headLookStabilizer.StabilizationDirection = _directionVector;
        }
    }

    private void FixedUpdate()
    {
        if (null != _neck)
        {
            if (Mathf.Abs(Vector3.Angle(_body.transform.TransformDirection(Vector3.forward), _directionVector)) < 30f)
            {
                Rigidbody rb = _body.GetComponent<Rigidbody>();
                rb.AddForceAtPosition(_directionVector * _force, rb.transform.TransformPoint(new Vector3(0f, _pushPointRatio, 0f)));
            }
        }
    }
}
