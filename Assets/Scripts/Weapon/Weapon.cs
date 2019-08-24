using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private float _maxDistance = 50f;

    [SerializeField]
    private LayerMask _layerMask = default;

    [SerializeField]
    private ParticleSystem _projectile = null;

    [SerializeField]
    private Transform _cam = null;

    [SerializeField]
    private float _rotationSlerpRate = 2f;

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
                    Vector3 targetPos = hit.point;
                    Rigidbody hitRb = hit.collider.GetComponent<Rigidbody>();
                    if (null != hitRb)
                    {
                        targetPos += hitRb.velocity * (Vector3.Distance(hit.point, transform.position) / _projectile.main.startSpeed.constant);
                    }
                    transform.LookAt(targetPos);
                    hasTarget = true;
                }
            }

            if (!hasTarget)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _cam.rotation, _rotationSlerpRate * Time.deltaTime);
            }
        }

        if (hasTarget)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _maxDistance, _layerMask))
            {
                if ((null != hit.collider) && (hit.collider.CompareTag("Target")))
                {
                    if (!_projectile.isPlaying)
                    {
                        _projectile.Play();
                    }
                }
            }
        }
    }
}
