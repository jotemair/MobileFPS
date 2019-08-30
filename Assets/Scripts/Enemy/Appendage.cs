using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appendage : MonoBehaviour
{
    [SerializeField]
    private Material _deadMat = null;

    [SerializeField]
    private Object _lifeSource = null;

    private bool _isAlive = true;

    public void Die()
    {
        if (_isAlive)
        {
            GetComponent<Renderer>().material = _deadMat;

            Rigidbody rigidbody = GetComponent<Rigidbody>();

            if (null != rigidbody)
            {
                rigidbody.mass /= 3f;
            }

            if (null != _lifeSource)
            {
                Destroy(_lifeSource);
            }

            _isAlive = false;

            Destroy(this);
        }

        return;
    }

    public void Update()
    {
        if (_isAlive && (null == _lifeSource))
        {
            Die();
        }
    }
}
