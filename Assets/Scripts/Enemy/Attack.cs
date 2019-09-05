using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private bool _canAttack = true;

    // Update is called once per frame
    void Update()
    {
        if (null == GetComponent<Appendage>())
        {
            enabled = false;
            _canAttack = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_canAttack && (collision.collider.CompareTag("Player")))
        {
            GameManager.Instance.PlayerHit();
        }
    }
}
