using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
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

    public void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * _firingForce, ForceMode.Impulse);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (null == transform.parent)
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<SphereCollider>());
            Vector3 scale = transform.localScale;
            Vector3 pos = transform.position;
            transform.parent = collision.transform;
            Vector3 parentScale = collision.transform.lossyScale;
            transform.localScale = Vector3.Scale(new Vector3(1f / parentScale.x, 1f / parentScale.y, 1f / parentScale.z), scale);
            transform.localRotation = Quaternion.identity;
            transform.position = collision.contacts[0].point;
        }
    }

    void Explode()
    {
        if (!_hasExploded)
        {

            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, _blastRadius);
            foreach (Collider hit in colliders)
            {
                if (hit.CompareTag("Target"))
                {
                    Rigidbody rb = hit.GetComponent<Rigidbody>();

                    if (null != rb)
                    {
                        rb.AddExplosionForce(_power, explosionPos, _blastRadius, 0.0f, ForceMode.Impulse);
                    }
                }
            }

            _hasExploded = true;
            if (null != _effect)
            {
                _effect.Play();
            }
            GetComponent<Renderer>().enabled = false;
            Destroy(gameObject, 1f);
        }
    }

    public void Update()
    {
        if (!_hasExploded)
        {
            List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(transform.position, _blastRadius));
            Collider hit = colliders.Find(x => (x.CompareTag("Target") && (null != x.transform.parent.GetComponent<EnemyController>())));
            if (null != hit)
            {
                Invoke("Explode", _timer);
            }
        }
    }
}
