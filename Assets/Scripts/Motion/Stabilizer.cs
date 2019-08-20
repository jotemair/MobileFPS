using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Stabilizer : MonoBehaviour
{
    [SerializeField]
    private float _strength = 1f;

    private Rigidbody _rb = null;

    private Vector3 delta = Vector3.zero;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + delta);
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Torque is a Vector3 where the vector direction is the axis of rotation and the magnitude is the angular acceleration (in radian / sec^2)
        // To get the direction of the torque, we need to get the cross product of the local and world up vectors, as the result will be perpendicular to both
        Vector3 rotationalAxis = Vector3.Cross(transform.up, Vector3.up);

        // The acceleration rate should depend on how far we are from the correct rotation, we can get the angle difference from the cross product
        float theta = Mathf.Asin(rotationalAxis.magnitude);

        // The desired change in the angular velocity
        Vector3 angularAcc = rotationalAxis.normalized * theta / Time.fixedDeltaTime;
        
        // Torque = Inertia tensor * angular acceleration
        // We need to multiply with the inertia tensor, however as it's not aligned, we need to transform into the proper space, multiply, then transform back
        Quaternion q = transform.rotation * _rb.inertiaTensorRotation;
        delta = q * Vector3.Scale(_rb.inertiaTensor, (Quaternion.Inverse(q) * angularAcc));

        if( !float.IsNaN(delta.x) && !float.IsNaN(delta.y) && !float.IsNaN(delta.z))
        {
            _rb.AddRelativeTorque(delta * _strength);
        }
    }
}
