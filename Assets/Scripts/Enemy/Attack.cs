using UnityEngine;

public class Attack : MonoBehaviour
{
    private bool _canAttack = true;

    #region MonoBehaviour Functions

    // Update is called once per frame
    void Update()
    {
        if (null == GetComponent<Appendage>())
        {
            // Can't attack anymore if the Appendage component is gone
            enabled = false;
            _canAttack = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Attack happens simply by colliding with the player
        if (_canAttack && (collision.collider.CompareTag("Player")))
        {
            GameManager.Instance.PlayerHit();
        }
    }

    #endregion
}
