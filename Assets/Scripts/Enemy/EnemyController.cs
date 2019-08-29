using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    [SerializeField]
    private Material _deadMat = null;

    private Stabilizer _bodyStabilizer = null;
    private Stabilizer _bodyLookStabilizer = null;

    private Stabilizer _headStabilizer = null;
    private Stabilizer _headLookStabilizer = null;

    private DirectionalJoint _neck = null;

    [SerializeField]
    private GameObject _target = null;

    [SerializeField]
    private NavMeshAgent _agent = null;

    [SerializeField]
    private float _avoidTreshold = 0.5f;

    private Vector3 _directionVector = Vector3.zero;

    private bool _isAlive = true;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(_body.transform.TransformPoint(new Vector3(0f, _pushPointRatio, 0f)), _body.transform.TransformPoint(new Vector3(0f, _pushPointRatio, 0f)) + _directionVector);
    }

    public void OnDrawGizmosSelected()
    {
        if (null != _agent)
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < _agent.path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(_agent.path.corners[i], _agent.path.corners[i + 1]);
            }
        }
    }

    private void Die()
    {
        if (_isAlive)
        {
            Destroy(_agent.gameObject);
            Destroy(_headStabilizer);
            Destroy(_headLookStabilizer);
            Destroy(_bodyLookStabilizer);
            _bodyStabilizer.Strength = -3f;
            _bodyStabilizer.BreakAngle = 25f;

            _body.GetComponent<Rigidbody>().mass = _body.GetComponent<Rigidbody>().mass / 3f;
            _head.GetComponent<Rigidbody>().mass = _head.GetComponent<Rigidbody>().mass / 3f;

            _body.GetComponent<Renderer>().material = _deadMat;
            _head.GetComponent<Renderer>().material = _deadMat;

            _isAlive = false;
        }

        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        var bodyStabilizers = _body.GetComponents<Stabilizer>();
        _bodyStabilizer = ((Stabilizer.Axes.Z == bodyStabilizers[0].Axis) ? bodyStabilizers[1] : bodyStabilizers[0]);
        _bodyLookStabilizer = ((Stabilizer.Axes.Z == bodyStabilizers[0].Axis) ? bodyStabilizers[0] : bodyStabilizers[1]);

        var headStabilizers = _head.GetComponents<Stabilizer>();
        _headStabilizer = ((Stabilizer.Axes.Z == headStabilizers[0].Axis) ? headStabilizers[1] : headStabilizers[0]);
        _headLookStabilizer = ((Stabilizer.Axes.Z == headStabilizers[0].Axis) ? headStabilizers[0] : headStabilizers[1]);

        _neck = _head.GetComponent<DirectionalJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (null == _neck)
        {
            Die();
        }
        else if (null != _target)
        {
            _agent.SetDestination(_target.transform.position);

            float avoidDist = Vector3.Scale(_agent.transform.localPosition, Vector3.right + Vector3.forward).magnitude;

            if ((avoidDist < _avoidTreshold) && (_agent.path.corners.Length > 1))
            {
                _directionVector = Vector3.Scale(_agent.path.corners[1] - _body.transform.position, Vector3.right + Vector3.forward).normalized;
                _bodyLookStabilizer.StabilizationDirection = _directionVector;
                _headLookStabilizer.StabilizationDirection = _directionVector;
            }
            else
            {
                Vector3 toTarget = _target.transform.position - _body.transform.position;
                _directionVector = Vector3.Scale(toTarget, Vector3.right + Vector3.forward).normalized;
                _bodyLookStabilizer.StabilizationDirection = _directionVector;
                _headLookStabilizer.StabilizationDirection = _directionVector;

                RaycastHit hit;
                if ((toTarget.magnitude < 1f) || (Physics.Raycast(_body.transform.position, _body.transform.TransformDirection(Vector3.forward), out hit, 0.5f, LayerMask.GetMask("Enemy"))))
                {
                    _directionVector = Vector3.zero;
                }

                _agent.transform.localPosition = Vector3.zero;
            }
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
