using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Repulsor : MonoBehaviour
{
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

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.25f, 0.25f, 0.5f);
        Gizmos.DrawSphere(anchorPosition(), 0.025f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(anchorPosition(), anchorPosition() + transform.TransformDirection(_axis) * _maxDistance + repulsonVector);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(anchorPosition(), anchorPosition() + repulsonVector);
    }

    private Vector3 anchorPosition()
    {
        return (transform.TransformPoint(_anchor));
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _axis = _axis.normalized;
    }

    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(anchorPosition(), transform.TransformDirection(_axis), out hit, _maxDistance, _layerMask))
        {
            Vector3 hitVector = anchorPosition() - hit.point;
            repulsonVector = hitVector.normalized * (_maxDistance - hitVector.magnitude);
            Vector3 repulsonForce = repulsonVector * _force;

            _rb.AddForce(repulsonForce);

            Rigidbody hitRB = hit.rigidbody;
            if (null != hitRB)
            {
                hitRB.AddForceAtPosition(-repulsonForce, hit.point);
            }
        }
    }
}
