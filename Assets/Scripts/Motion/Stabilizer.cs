using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StabilizationAxes
{
    X,
    Y,
    Z,
};

[RequireComponent(typeof(Rigidbody))]
public class Stabilizer : MonoBehaviour
{
    [SerializeField]
    private float _strength = 1f;

    [SerializeField]
    private float _breakAngle = float.PositiveInfinity;

    [SerializeField]
    private Vector3 _stabilizationDirection = new Vector3(0f, 1f, 0f);

    [SerializeField]
    private StabilizationAxes _axis = StabilizationAxes.Y;

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

    public StabilizationAxes Axis
    {
        get { return _axis; }
        set { _axis = value; }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + delta);

        switch (_axis)
        {
            case StabilizationAxes.X:
                Gizmos.color = Color.red;
                break;
            case StabilizationAxes.Y:
                Gizmos.color = Color.green;
                break;
            case StabilizationAxes.Z:
                Gizmos.color = Color.blue;
                break;
        }
        Gizmos.DrawLine(transform.position, transform.position + (transform.TransformDirection(_stabilizationDirection) / 2f));
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        StabilizationDirection = _stabilizationDirection;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Mathf.Abs(Vector3.Angle(transform.TransformDirection(_stabilizationDirection), GetAxis())) > _breakAngle)
        {
            Destroy(this);
        }
        else
        {
            // Torque is a Vector3 where the vector direction is the axis of rotation and the magnitude is the angular acceleration (in radian / sec^2)
            // To get the direction of the torque, we need to get the cross product of the local and world up vectors, as the result will be perpendicular to both
            Vector3 rotationalAxis = Vector3.Cross(transform.TransformDirection(_stabilizationDirection), GetAxis());

            // The acceleration rate should depend on how far we are from the correct rotation, we can get the angle difference from the cross product
            float theta = Mathf.Asin(rotationalAxis.magnitude);

            // The desired change in the angular velocity
            Vector3 angularAcc = rotationalAxis.normalized * theta / Time.fixedDeltaTime;

            // Torque = Inertia tensor * angular acceleration
            // We need to multiply with the inertia tensor, however as it's not aligned, we need to transform into the proper space, multiply, then transform back
            Quaternion q = transform.rotation * _rb.inertiaTensorRotation;
            delta = q * Vector3.Scale(_rb.inertiaTensor, (Quaternion.Inverse(q) * angularAcc));

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
            case StabilizationAxes.X:
                axis = Vector3.right;
                break;
            case StabilizationAxes.Y:
                axis = Vector3.up;
                break;
            case StabilizationAxes.Z:
                axis = Vector3.forward;
                break;
        }

        return axis;
    }
}
