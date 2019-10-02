using UnityEngine;
using UnityEngine.Assertions;

// The DirectionalJoint is similar to the Unity SpringJoint
// However it's able to keep a specific direction, instead of being directionless
[RequireComponent(typeof(Rigidbody))]
public class DirectionalJoint : MonoBehaviour
{
    #region Private Variables

    // The other end of the joint. If it has a rigidbody, mirrored forces will apply to it. However it doesn't need to have a rigidbody
    // The connection can be left blank, in that case the other end will be rooted in worldspace
    [SerializeField]
    private GameObject _connectedBody = null;

    // The local position of the anchor connecting to the object this component is on
    [SerializeField]
    private Vector3 _anchor = Vector3.zero;

    // The local position of the anchor at the other end of the joint (local, relative to the other object the joint connects to)
    [SerializeField]
    private Vector3 _connectedAnchor = Vector3.zero;

    // The spring constant that determines the strength of the spring
    [SerializeField]
    private float _spring = 100f;

    // Break treshold, if the force produced by the spring exceeds this value, it will break
    [SerializeField]
    private float _breakForce = float.PositiveInfinity;

    // By default the joint has a directional property, but it can be turned off to have it behave like a regular spring
    [SerializeField]
    private bool _directional = true;

    // When in regular spring mode, a min and max ration can be defined, and the pring will only produce force, when it's length is outside that range
    // The ratio is relative to the initial length of the spring
    // IMPROVEMENT : Use a default length ratio to define the default base length, and a tolerance value to indicate how much the length can change before force is applied
    //                 This approach could aslo be applied when the joint is in directional mode
    [SerializeField]
    private float _minLengthRatio = 1f;

    [SerializeField]
    private float _maxLengthRatio = 1f;

    // Set if rotation of the local object changes the direction the joint tries to point in
    [SerializeField]
    private bool _anchorRotateX = false;

    [SerializeField]
    private bool _anchorRotateY = false;

    [SerializeField]
    private bool _anchorRotateZ = false;

    // Set if rotation of the connected object changes the direction the joint tries to point in
    [SerializeField]
    private bool _connectedAnchorRotateX = false;

    [SerializeField]
    private bool _connectedAnchorRotateY = false;

    [SerializeField]
    private bool _connectedAnchorRotateZ = false;

    // Spring dampening to eventually slow the spring down
    // IMPROVEMENT : Still unstable in certain situations, needs further refinement
    [SerializeField]
    private float _damping = 0f;

    private Rigidbody _rb = null;

    private Rigidbody _connectedRb = null;

    private Vector3 _initialAlignement = Vector3.zero;

    private Vector3 _initialPosition = Vector3.zero;

    private float _initialLength = 0f;

    private Quaternion _initialAnchorRotation = Quaternion.identity;
    private Quaternion _initialConnectedAnchorRotation = Quaternion.identity;

    private float _minLength = 0f;
    private float _maxLength = 0f;

    // Stored as a variable for debug purposes
    private Vector3 springDelta = Vector3.zero;

    #endregion

    #region Private Functions

    // Helper function to get the world position of the anchor
    private Vector3 anchorPosition()
    {
        return (transform.TransformPoint(_anchor));
    }

    // Helper function to get the world position of the connected anchor, the other end of the joint
    private Vector3 connectedAnchorPosition()
    {
        return (null == _connectedBody) ? (_initialPosition) : (_connectedBody.transform.TransformPoint(_connectedAnchor));
    }

    // Helper function to get the relative velocity of one end of the joint compared to the other
    // Spring dampening depends on this velocity
    private Vector3 GetRelativeSpringVelocity(Vector3 direction)
    {
        Vector3 relativeVelocity = Vector3.zero;

        if (Vector3.zero != direction)
        {
            // IMPROVEMENT : Try and calculate the velocity of the anchor point, instead of using the center of the rigidbody
            Vector3 anchorVelocity = MathUtils.GetVectorProjection(_rb.velocity, direction);
            Vector3 connectedAnchorVelocity = MathUtils.GetVectorProjection(((null == _connectedRb) ? Vector3.zero : _connectedRb.velocity), direction);

            relativeVelocity = anchorVelocity - connectedAnchorVelocity;
        }

        return relativeVelocity;
    }

    // Helper function to get the harmonic mass of the two bodies
    // Used in spring dampening, experimentation showed this to be the most practical
    private float GetHarmonicMass()
    {
        float anchorMass = _rb.mass;
        float connectedAnchorMass = ((null == _connectedRb) ? float.PositiveInfinity : _connectedRb.mass);

        return ( 2f / ((1f / anchorMass) + (1f / connectedAnchorMass)) );
    }

    #endregion

    #region MonoBehaviour Functions

    void Start()
    {
        // Initialize starting values
        Assert.IsTrue(_minLengthRatio <= _maxLengthRatio, "Minumum length must be less than maximum length");
        _rb = GetComponent<Rigidbody>();
        _initialPosition = anchorPosition();
        _initialAlignement = (connectedAnchorPosition() - anchorPosition());
        _initialLength = _initialAlignement.magnitude;
        _initialAnchorRotation = transform.rotation;
        _minLength = _initialLength * _minLengthRatio;
        _maxLength = _initialLength * _maxLengthRatio;

        // It's not required to have a connection to another object.
        // In this case, the other end of the joint will connect to the world position it starts in
        if (null !=_connectedBody)
        {
            _connectedRb = _connectedBody.GetComponent<Rigidbody>();
            _initialConnectedAnchorRotation = _connectedBody.transform.rotation;
        }
    }

    void FixedUpdate()
    {
        if (_directional)
        {
            // When using the directional property of the joint, the spring force points in a way that tries to move the connected objects in their original alignement

            // We might need to rotate the direction of the compared to the initial, if anchor rotation is enabled

            Quaternion connectedRot = _connectedBody.transform.rotation * Quaternion.Inverse(_initialConnectedAnchorRotation);
            connectedRot = Quaternion.Euler(Vector3.Scale(connectedRot.eulerAngles, new Vector3( (_connectedAnchorRotateX ? 1f : 0f), (_connectedAnchorRotateY ? 1f : 0f), (_connectedAnchorRotateZ ? 1f : 0f))));

            Quaternion rot = transform.rotation * Quaternion.Inverse(_initialAnchorRotation);
            rot = Quaternion.Euler(Vector3.Scale(rot.eulerAngles, new Vector3((_anchorRotateX ? 1f : 0f), (_anchorRotateY ? 1f : 0f), (_anchorRotateZ ? 1f : 0f))));

            // We get the current alignement of the objects and compare it to the original (rotated by the anchors if enabled)
            // This gives us the spring force magnitude and direction
            springDelta = (connectedAnchorPosition() - anchorPosition()) - (rot * connectedRot * _initialAlignement);
        }
        else
        {
            // If directional mode is off, the joint behaves much like a spring
            // The strain on the spring depends on how much we are outside the min and max range for the spring length
            Vector3 springVector = (connectedAnchorPosition() - anchorPosition());
            float springStrain = springVector.magnitude;

            // At least one of these components will always be zero depending on whether the length is less than min or greated than max, or both if the value is in between
            springStrain = Mathf.Min(0f, springStrain - _minLength) + Mathf.Max(0f, springStrain - _maxLength);
            springDelta = springVector.normalized * springStrain;
        }

        // The spring force is the springDelta multiplied by the spring strength constant
        Vector3 springForce = springDelta * _spring;

        // The dampening force applies along the same direction as the spring force, but it counters the relative movement of the connected objects, and increases with speed
        // IMPROVEMENT : The dampening force is still unstable in certain situations
        Vector3 dampenForce = -GetRelativeSpringVelocity(springDelta) * 2f * Mathf.Sqrt(_spring * GetHarmonicMass());
        Vector3 summedForce = springForce + dampenForce * ((-2f / (_damping + 1f)) + 2f);

        _rb.AddForceAtPosition(summedForce, anchorPosition());
        if (null != _connectedRb)
        {
            _connectedRb.AddForceAtPosition(-summedForce, connectedAnchorPosition());
        }

        if (springForce.magnitude > _breakForce)
        {
            // If the spring force is greater then the break treshold, the joint breaks
            // However only after applying counter force, so the breaking force is still affected for one frame as the spring breaks
            Destroy(this);
        }
    }

    // Debug draw to help visualise certain properties
    private void OnDrawGizmos()
    {
        // Draw the local anchor
        Gizmos.color = new Color(0.5f, 0.25f, 0.25f);
        Gizmos.DrawSphere(anchorPosition(), 0.025f);

        // Draw the spring force affecting the local anchor
        Gizmos.color = Color.red;
        Gizmos.DrawLine(anchorPosition(), anchorPosition() + springDelta);

        if (null != _connectedBody)
        {
            // Draw the connected anchor
            Gizmos.color = new Color(0.5f, 0.25f, 0.25f);
            Gizmos.DrawSphere(connectedAnchorPosition(), 0.025f);

            // Draw the spring force affecting the connected anchor
            Gizmos.color = Color.red;
            Gizmos.DrawLine(connectedAnchorPosition(), connectedAnchorPosition() - springDelta);

            // Draw the joint between the two anchors
            Gizmos.color = Color.green;
            Gizmos.DrawLine(anchorPosition(), connectedAnchorPosition());
        }
    }

    #endregion
}
