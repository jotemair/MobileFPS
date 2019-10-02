using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appendage : MonoBehaviour
{
    #region Private Variables

    [SerializeField]
    private Material _deadMat = null;

    [SerializeField]
    private Object _lifeSource = null;

    private bool _isAlive = true;

    #endregion

    #region MonoBehaviour Functions

    public void Update()
    {
        if (_isAlive && (null == _lifeSource))
        {
            // As soon as the lifesource (usually a joint that connects the appendage to the main body) dies, the appendage dies as well
            Die();
        }
    }

    #endregion

    #region Public Functions

    public void Die()
    {
        if (_isAlive)
        {
            // Change the material to better show the part is 'dead'
            GetComponent<Renderer>().material = _deadMat;

            Rigidbody rigidbody = GetComponent<Rigidbody>();

            if (null != rigidbody)
            {
                // Decrease rigidbody mass to make it easier to push/knock around
                rigidbody.mass /= 3f;
            }

            if (null != _lifeSource)
            {
                // The die function can also be called from outside (usually the main body), in this case destroy the lifesource
                Destroy(_lifeSource);
            }

            _isAlive = false;

            // Destroy this component right now, and despawn the gameobject a bit later
            Destroy(this);
            Destroy(gameObject, 30f);
        }

        return;
    }

    #endregion
}
