using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Private Variables

    [SerializeField]
    private float _maxDistance = 50f;

    [SerializeField]
    private LayerMask _layerMask = default;

    // The projectiles are handled by a particle system
    // It's a burst system with no emission, so the time of the particle effect works as the reload time
    // The particles are set to collide and to have impact force on collision
    // The enemies are set up to die if the joint that connects their head to their body is broken, so they don't have traditional health, rather it's physics based
    [SerializeField]
    private ParticleSystem _projectile = null;

    [SerializeField]
    private GameObject _secondary = null;

    [SerializeField]
    private Transform _cam = null;

    [SerializeField]
    private float _rotationSlerpRate = 2f;

    #endregion

    #region MonoBehaviour Functions

    void Update()
    {
        bool hasTarget = false;

        // We send out a ray from the center of the camera
        RaycastHit hit;
        if (Physics.Raycast(_cam.position, _cam.TransformDirection(Vector3.forward), out hit, _maxDistance, _layerMask))
        {
            // We check if it hit a target
            if ((null != hit.collider) && (hit.collider.CompareTag("Target")))
            {
                // We get the target position from the ray hit
                Vector3 targetPos = hit.point;

                // We check if the target has a rigidbody or not
                Rigidbody hitRb = hit.collider.GetComponent<Rigidbody>();
                if (null != hitRb)
                {
                    // If the target has a rigidbody, we automatically lead our shots based on it's speed
                    // Since the player can't shoot manually, and must aim directly at a target to shoot, we need to lead the shots in code
                    // Get the velocity, and multiply it by how much time the projectile will need to travel the distance
                    targetPos += hitRb.velocity * (Vector3.Distance(hit.point, transform.position) / _projectile.main.startSpeed.constant);
                }

                // We make the weapon immediately face the target, so the projectile will shoot in the right direction
                transform.LookAt(targetPos);
                hasTarget = true;

                // If the projectile particle system isn't playing, the start it up
                if (!_projectile.isPlaying)
                {
                    _projectile.Play();
                }
            }
        }

        if (!hasTarget)
        {
            // If we don't have a target, we gradually move facing the same direction as the player head/camera
            transform.rotation = Quaternion.Slerp(transform.rotation, _cam.rotation, _rotationSlerpRate * Time.deltaTime);
        }
    }

    // Debug draw to show weapon target line
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward) * _maxDistance);
    }

    #endregion

    #region Public Functions

    // Function to spawn secondary attack
    public void FireSecondary()
    {
        if (null != _secondary)
        {
            Instantiate(_secondary, transform.position, transform.rotation);
        }
    }

    #endregion
}
