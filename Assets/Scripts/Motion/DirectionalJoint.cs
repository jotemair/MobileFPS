using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class DirectionalJoint : MonoBehaviour
{
    [SerializeField]
    private GameObject _connectedBody = null;

    [SerializeField]
    private Vector3 _anchor = Vector3.zero;

    [SerializeField]
    private Vector3 _connectedAnchor = Vector3.zero;

    [SerializeField]
    private float _spring = 100f;

    [SerializeField]
    private float _breakForce = float.PositiveInfinity;

    [SerializeField]
    private bool _directional = true;

    [SerializeField]
    private float _minLengthRatio = 1f;

    [SerializeField]
    private float _maxLengthRatio = 1f;

    private Rigidbody _rb = null;

    private Rigidbody _connectedRb = null;

    private Vector3 _initialDistance = Vector3.zero;

    private Vector3 _initialPosition = Vector3.zero;

    private float _initialLength = 0f;

    private float _minLength = 0f;
    private float _maxLength = 0f;

    private Vector3 springDelta = Vector3.zero;

    private Vector3 anchorPosition()
    {
        return (transform.TransformPoint(_anchor));
    }

    private Vector3 connectedAnchorPosition()
    {
        return (null == _connectedBody) ? (_initialPosition) : (_connectedBody.transform.TransformPoint(_connectedAnchor));
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.25f, 0.25f);
        Gizmos.DrawSphere(anchorPosition(), 0.025f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(anchorPosition(), anchorPosition() + springDelta);

        if (null != _connectedBody)
        {
            Gizmos.color = new Color(0.5f, 0.25f, 0.25f);
            Gizmos.DrawSphere(connectedAnchorPosition(), 0.025f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(connectedAnchorPosition(), connectedAnchorPosition() - springDelta);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(anchorPosition(), connectedAnchorPosition());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(_minLengthRatio <= _maxLengthRatio, "Minumum length must be less than maximum length");
        _rb = GetComponent<Rigidbody>();
        _initialPosition = anchorPosition();
        _initialDistance = (connectedAnchorPosition() - anchorPosition());
        _initialLength = _initialDistance.magnitude;
        _minLength = _initialLength * _minLengthRatio;
        _maxLength = _initialLength * _maxLengthRatio;

        if (null !=_connectedBody)
        {
            _connectedRb = _connectedBody.GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_directional)
        {
            springDelta = (connectedAnchorPosition() - anchorPosition()) - _initialDistance;
        }
        else
        {
            Vector3 springVector = (connectedAnchorPosition() - anchorPosition());
            float springStrain = springVector.magnitude;
            springStrain = Mathf.Min(0f, springStrain - _minLength) + Mathf.Max(0f, springStrain - _maxLength);
            springDelta = springVector.normalized * springStrain;
        }

        _rb.AddForceAtPosition(springDelta * _spring, anchorPosition());
        if (null != _connectedRb)
        {
            _connectedRb.AddForceAtPosition(-springDelta * _spring, connectedAnchorPosition());
        }

        if ((springDelta * _spring).magnitude > _breakForce)
        {
            Destroy(this);
        }
    }
}
