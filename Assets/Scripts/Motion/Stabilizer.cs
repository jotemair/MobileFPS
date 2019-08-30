using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class Stabilizer : MonoBehaviour
{
    public enum Axes
    {
        X,
        Y,
        Z,
        None,
    };

    [SerializeField]
    private float _strength = 1f;

    [SerializeField]
    private float _breakAngle = float.PositiveInfinity;

    [SerializeField]
    private Vector3 _stabilizationDirection = new Vector3(0f, 1f, 0f);

    [SerializeField]
    private Axes _axis = Axes.Y;

    [SerializeField]
    private Axes _stabilizationAxisLock = Axes.None;

    private Rigidbody _rb = null;

    private Vector3 delta = Vector3.zero;

    public float Strength
    {
        get { return _strength; }
        set { _strength = value; }
    }

    public float BreakAngle
    {
        get { return _breakAngle; }
        set { _breakAngle = value; }
    }

    public Vector3 StabilizationDirection
    {
        get { return _stabilizationDirection; }
        set { _stabilizationDirection = value.normalized; }
    }

    public Axes Axis
    {
        get { return _axis; }
        set
        {
            Assert.IsTrue(Axes.None != _axis, "Stabilization axis cannot be NONE");
            _axis = value;
        }
    }

    public Axes StabilizationAxisLock
    {
        get { return _stabilizationAxisLock; }
        set { _stabilizationAxisLock = value; }
    }

    public void OnDrawGizmos()
    {
        switch (_axis)
        {
            case Axes.X:
                Gizmos.color = Color.red;
                break;
            case Axes.Y:
                Gizmos.color = Color.green;
                break;
            case Axes.Z:
                Gizmos.color = Color.blue;
                break;
        }
        Gizmos.DrawLine(transform.position, transform.position + (_stabilizationDirection / 2f));
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, transform.position + delta);
    }

    // Start is called before the first frame update
    void Start()
    {
        Axis = _axis;
        _rb = GetComponent<Rigidbody>();
        StabilizationDirection = _stabilizationDirection;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 axisVector = transform.TransformDirection(GetAxis());
        float angleDiff = Mathf.Abs(Vector3.Angle(axisVector, _stabilizationDirection));

        // The math breaks down when the angle difference is exactly 90 degrees which can happen when something is laying on a surface
        if (Mathf.Approximately(angleDiff, 90f) || Mathf.Approximately(angleDiff, 180f) || Mathf.Approximately(angleDiff, 270f))
        {
            axisVector = transform.TransformDirection(GetAxis() + new Vector3(0.01f, 0.01f, 0.01f)).normalized;
        }

        switch (_stabilizationAxisLock)
        {
            case (Axes.X):
                axisVector = new Vector3(0f, axisVector.y, axisVector.z).normalized;
                break;
            case (Axes.Y):
                axisVector = new Vector3(axisVector.x, 0f, axisVector.z).normalized;
                break;
            case (Axes.Z):
                axisVector = new Vector3(axisVector.x, axisVector.y, 0f).normalized;
                break;
            default:
                break;
        }

        if (angleDiff > _breakAngle)
        {
            Destroy(this);
        }
        else
        {
            // Torque is a Vector3 where the vector direction is the axis of rotation and the magnitude is the angular acceleration (in radian / sec^2)
            // To get the direction of the torque, we need to get the cross product of the local and world up vectors, as the result will be perpendicular to both
            Vector3 rotationalAxis = Vector3.Cross(axisVector, _stabilizationDirection);

            // The acceleration rate should depend on how far we are from the correct rotation, we can get the angle difference from the cross product
            float theta = Mathf.Asin(rotationalAxis.magnitude);

            // The desired change in the angular velocity
            Vector3 angularAcc = rotationalAxis.normalized * theta / Time.fixedDeltaTime;

            // Torque = Inertia tensor * angular acceleration
            // We need to multiply with the inertia tensor, however as it's not aligned, we need to transform into the proper space, multiply, then transform back
            Quaternion q = transform.rotation * _rb.inertiaTensorRotation;
            delta = q * Vector3.Scale(_rb.inertiaTensor, (Quaternion.Inverse(q) * angularAcc));

            // The math should not break down, since the edge cases ate handled with an added perturbance, but just in case
            if (!float.IsNaN(delta.x) && !float.IsNaN(delta.y) && !float.IsNaN(delta.z))
            {
                _rb.AddTorque(delta * _strength);
            }
        }
    }

    private Vector3 GetAxis()
    {
        Vector3 axis = Vector3.zero;

        switch (_axis)
        {
            case Axes.X:
                axis = Vector3.right;
                break;
            case Axes.Y:
                axis = Vector3.up;
                break;
            case Axes.Z:
                axis = Vector3.forward;
                break;
        }

        return axis;
    }
}
