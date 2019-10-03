using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    #region Private Variables

    [SerializeField]
    private float _blastRadius = 5.0F;

    [SerializeField]
    private float _power = 10.0F;

    [SerializeField]
    private float _firingForce = 50f;

    [SerializeField]
    private float _timer = 1f;

    [SerializeField]
    private ParticleSystem _effect = null;

    private bool _hasExploded = false;

    #endregion

    #region MonoBehaviour Functions

    private void Start()
    {
        // After spawning, immediately shoot forward
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * _firingForce, ForceMode.Impulse);
    }

    private void Update()
    {
        // If the bomb hasn't exploded yet
        if (!_hasExploded)
        {
            // Check for objects with the Target tag in the given radius, that also have an EnemyController component
            List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(transform.position, _blastRadius));
            Collider hit = colliders.Find(x => (x.CompareTag("Target") && (null != x.transform.parent.GetComponent<EnemyController>())));

            // If a valid target is found, start the delayed explosion
            if (null != hit)
            {
                Invoke("Explode", _timer);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // The first time the bomb hits something, stick to it, by setting that object as parent
        if (null == transform.parent)
        {
            // Destroy the rigidbody and the collider, from now on movement is based only on the parent
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<SphereCollider>());

            // Attach to the object we collided with, and update scale so that it negates any scaling from the new parent
            Vector3 scale = transform.localScale;
            transform.parent = collision.transform;
            Vector3 parentScale = collision.transform.lossyScale;
            transform.localScale = Vector3.Scale(new Vector3(1f / parentScale.x, 1f / parentScale.y, 1f / parentScale.z), scale);
            transform.localRotation = Quaternion.identity;
            transform.position = collision.contacts[0].point;
        }
    }

    #endregion

    #region Private Functions

    private void Explode()
    {
        // If the bomb hasn't exploded yet
        if (!_hasExploded)
        {
            // Find all objects within the blast radius
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, _blastRadius);
            foreach (Collider hit in colliders)
            {
                // Check if they have the Target tag
                if (hit.CompareTag("Target"))
                {
                    Rigidbody rb = hit.GetComponent<Rigidbody>();

                    // If they have a rigidbody, apply explosion force
                    if (null != rb)
                    {
                        rb.AddExplosionForce(_power, explosionPos, _blastRadius, 0.0f, ForceMode.Impulse);
                    }
                }
            }

            // If an effect is attached, play it
            _hasExploded = true;
            if (null != _effect)
            {
                _effect.Play();
            }

            // Disable the renderer, and delayed destroy the gameObject
            // This makes the bomb disappear immediately when it explodes, but gives time for the effect to play
            GetComponent<Renderer>().enabled = false;
            Destroy(gameObject, 1f);
        }
    }

    #endregion
}
