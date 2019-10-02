using UnityEngine;

// The Repulsor acts as a hover engine that produces force in a direction if it can push off something in the opposite direction
[RequireComponent(typeof(Rigidbody))]
public class Repulsor : MonoBehaviour
{
    #region Private Variables

    [SerializeField]
    private float _force = 100f;

    [SerializeField]
    private float _maxDistance = 1f;

    [SerializeField]
    private LayerMask _layerMask = ~0;

    [SerializeField]
    private Vector3 _anchor = new Vector3(0f, -0.5f, 0f);

    [SerializeField]
    private Vector3 _axis = new Vector3(0f, -1f, 0f);

    private Rigidbody _rb = null;

    private Vector3 repulsonVector = Vector3.zero;

    #endregion

    #region MonoBehaviour Functions

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _axis = _axis.normalized;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        // If we can hit something in the defined direction within the given range that's part of the allowed layermask, we push off
        if (Physics.Raycast(anchorPosition(), transform.TransformDirection(_axis), out hit, _maxDistance, _layerMask))
        {
            // The repulsion force increases as the hit point gets closer to the anchor

            Vector3 hitVector = anchorPosition() - hit.point;
            repulsonVector = hitVector.normalized * (_maxDistance - hitVector.magnitude);
            Vector3 repulsonForce = repulsonVector * _force;

            _rb.AddForce(repulsonForce);

            // If the object we push off of also has a rigidbody, it will be effected by the repolsor as well
            Rigidbody hitRB = hit.rigidbody;
            if (null != hitRB)
            {
                hitRB.AddForceAtPosition(-repulsonForce, hit.point);
            }
        }
    }

    // Debug draw to show repulsor effect line, and forces
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.25f, 0.25f, 0.5f);
        Gizmos.DrawSphere(anchorPosition(), 0.025f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(anchorPosition(), anchorPosition() + transform.TransformDirection(_axis) * _maxDistance + repulsonVector);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(anchorPosition(), anchorPosition() + repulsonVector);
    }

    #endregion

    #region Private Functions

    // Helper function to get the world space anchor position from local space coordinates
    private Vector3 anchorPosition()
    {
        return (transform.TransformPoint(_anchor));
    }

    #endregion
}
