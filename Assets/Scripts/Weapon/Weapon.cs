using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private float _fireDelay = 2f;

    [SerializeField]
    private float _maxDistance = 50f;

    [SerializeField]
    private LayerMask _layerMask = default;

    [SerializeField]
    private GameObject _projectile = null;

    [SerializeField]
    private Transform _cam = null;

    [SerializeField]
    private float _rotationSlerpRate = 2f;

    private float _fireTimer = 0f;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward) * _maxDistance);
    }

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(_projectile);
    }

    // Update is called once per frame
    void Update()
    {
        bool hasTarget = false;
        {
            RaycastHit hit;

            if (Physics.Raycast(_cam.position, _cam.TransformDirection(Vector3.forward), out hit, _maxDistance, _layerMask))
            {
                if ((null != hit.collider) && (hit.collider.CompareTag("Target")))
                {
                    transform.LookAt(hit.point);
                    hasTarget = true;
                }
            }

            if (!hasTarget)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _cam.rotation, _rotationSlerpRate * Time.deltaTime);
            }
        }

        _fireTimer = Mathf.Max(0f, _fireTimer - Time.deltaTime);

        if ((0f == _fireTimer) && hasTarget)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _maxDistance, _layerMask))
            {
                if ((null != hit.collider) && (hit.collider.CompareTag("Target")))
                {
                    Instantiate(_projectile, transform.position, transform.rotation);
                    _fireTimer = _fireDelay;
                }
            }
        }
    }
}
