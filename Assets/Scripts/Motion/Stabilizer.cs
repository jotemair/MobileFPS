using UnityEngine;
using UnityEngine.Assertions;

// The stabilizer is a component that tries to rotate the gameobject it's attached to in a certain orientation
// Like how a spinning top tries to keep upright, the stabilizer tries to rotate one of the XYZ axis in a given direction
[RequireComponent(typeof(Rigidbody))]
public class Stabilizer : MonoBehaviour
{
    #region Private Variables

    // The strength of the stabilizer
    [SerializeField]
    private float _strength = 1f;

    // The break angle of the stabilizer, if the angle between the stabilization axis and direction is greater, the stabilizer breaks
    // IMPROVEMENT: Add the option to set break torque
    [SerializeField]
    private float _breakAngle = float.PositiveInfinity;

    // The world space direction the stabilizer tries to rotate the stabilization axis to
    [SerializeField]
    private Vector3 _stabilizationDirection = new Vector3(0f, 1f, 0f);

    // The axis we are trying to stabilize
    [SerializeField]
    private Axes _stabilizationAxis = Axes.Y;

    // We can lock to an axis to only allow rotation around that axis
    [SerializeField]
    private Axes _stabilizationAxisLock = Axes.None;

    private Rigidbody _rb = null;

    private Vector3 delta = Vector3.zero;

    #endregion

    #region Public Properties

    public enum Axes
    {
        X,
        Y,
        Z,
        None,
    };

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

    public Axes StabilizationAxis
    {
        get { return _stabilizationAxis; }
        set { _stabilizationAxis = value; }
    }

    public Axes StabilizationAxisLock
    {
        get { return _stabilizationAxisLock; }
        set { _stabilizationAxisLock = value; }
    }

    #endregion

    #region MonoBehaviour Functions

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        StabilizationDirection = _stabilizationDirection;
    }

    private void FixedUpdate()
    {
        if (Axes.None == _stabilizationAxis)
        {
            // If the stabilization axis is set to none, then we're disabled
            return;
        }

        Vector3 axisVector = transform.TransformDirection(GetAxis(_stabilizationAxis));
        float angleDiff = Mathf.Abs(Vector3.Angle(axisVector, _stabilizationDirection));

        // The math breaks down when the angle difference is exactly a multiple of 90 degrees which can happen when something is laying on a surface
        if (Mathf.Approximately(angleDiff, 90f) || Mathf.Approximately(angleDiff, 180f) || Mathf.Approximately(angleDiff, 270f))
        {
            axisVector = transform.TransformDirection(GetAxis(_stabilizationAxis) + new Vector3(0.01f, 0.01f, 0.01f)).normalized;
        }

        // Note that this bit of code is mathematically wrong, BUT it works for what it's used for in-game
        // IMPROVEMENT : Figure out how to correctly do it
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
            // To get the direction of the torque, we need to get the cross product of the 'from' and 'to' vectors, as the result will be perpendicular to both
            Vector3 rotationalAxis = Vector3.Cross(axisVector, _stabilizationDirection);

            // The acceleration rate should depend on how far we are from the correct rotation, we can get the angle difference from the cross product
            float theta = Mathf.Asin(rotationalAxis.magnitude);

            // The desired change in the angular velocity
            Vector3 angularAcc = rotationalAxis.normalized * theta / Time.fixedDeltaTime;

            // Torque = Inertia tensor * angular acceleration
            // We need to multiply with the inertia tensor, however as it's not aligned, we need to transform into the proper space, multiply, then transform back
            Quaternion q = transform.rotation * _rb.inertiaTensorRotation;
            delta = q * Vector3.Scale(_rb.inertiaTensor, (Quaternion.Inverse(q) * angularAcc));

            // The math should not break down, since the edge cases are handled with an added perturbance, but just in case
            if (!float.IsNaN(delta.x) && !float.IsNaN(delta.y) && !float.IsNaN(delta.z))
            {
                _rb.AddTorque(delta * _strength);
            }
        }
    }

    private void OnDrawGizmos()
    {
        switch (_stabilizationAxis)
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

    // Debug draw to show the axis around which we are currently rotating
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (transform.position + delta).normalized * (2f - (1f / (delta.magnitude + 1f))));
    }

    #endregion

    #region Private Functions

    // Helper function to get the axis we are trying to stabilize
    private Vector3 GetAxis(Axes targetAxis)
    {
        Vector3 axis = Vector3.zero;

        switch (targetAxis)
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

    #endregion
}
