using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    #region Private Variables

    [SerializeField]
    private GameObject _body = null;

    [SerializeField]
    private GameObject _head = null;

    [SerializeField]
    private float _force = 0f;

    [SerializeField]
    private float _pushPointRatio = 0f;

    [SerializeField]
    private Material _deadMat = null;

    private Stabilizer _bodyStabilizer = null;
    private Stabilizer _bodyLookStabilizer = null;

    private Stabilizer _headStabilizer = null;
    private Stabilizer _headLookStabilizer = null;

    private DirectionalJoint _neck = null;

    [SerializeField]
    private GameObject _target = null;

    [SerializeField]
    private NavMeshAgent _agent = null;

    private Vector3 _directionVector = Vector3.zero;

    private bool _isAlive = true;

    [SerializeField]
    private List<Appendage> _appendages = new List<Appendage>();

    private Spawner _spawn = null;

    #endregion

    #region Public Properties

    public Spawner Spawn
    {
        get { return _spawn; }
        set { _spawn = value; }
    }

    #endregion

    #region MonoBehaviour Functions

    // Start is called before the first frame update
    void Start()
    {
        // Setup references to important components

        var bodyStabilizers = _body.GetComponents<Stabilizer>();
        _bodyStabilizer = ((Stabilizer.Axes.Z == bodyStabilizers[0].StabilizationAxis) ? bodyStabilizers[1] : bodyStabilizers[0]);
        _bodyLookStabilizer = ((Stabilizer.Axes.Z == bodyStabilizers[0].StabilizationAxis) ? bodyStabilizers[0] : bodyStabilizers[1]);

        var headStabilizers = _head.GetComponents<Stabilizer>();
        _headStabilizer = ((Stabilizer.Axes.Z == headStabilizers[0].StabilizationAxis) ? headStabilizers[1] : headStabilizers[0]);
        _headLookStabilizer = ((Stabilizer.Axes.Z == headStabilizers[0].StabilizationAxis) ? headStabilizers[0] : headStabilizers[1]);

        _neck = _head.GetComponent<DirectionalJoint>();

        // For testing purposes, any GameObject can be set as target, but by default it will be set to the player
        if (null == _target)
        {
            _target = FindObjectOfType<PlayerControls>().gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (null == _neck)
        {
            // The neck joint keeps the enemy alive, if it breaks, the enemy dies
            Die();
        }
        else if (null != _target)
        {
            // Each enemy has a NavMeshAgent with zero speed on a child object to calculate the shortest path to the player
            // The agent doesn't control the enemy directly, as that would interfere with the physics based behaviour

            // We update the target for the agent
            _agent.SetDestination(_target.transform.position);

            if (_agent.path.corners.Length > 1)
            {
                // If we have multiple points in the path, we can't go in a straight line, go towards the next corner
                _directionVector = Vector3.Scale(_agent.path.corners[1] - _body.transform.position, Vector3.right + Vector3.forward).normalized;
            }
            else
            {
                // We can go in a straight line, go towards the player
                Vector3 toTarget = _target.transform.position - _body.transform.position;
                _directionVector = Vector3.Scale(toTarget, Vector3.right + Vector3.forward).normalized;
            }

            // Set the desired facing direction of the head and the body
            _bodyLookStabilizer.StabilizationDirection = _directionVector;
            _headLookStabilizer.StabilizationDirection = _directionVector;

            // Update agent position
            _agent.transform.position = _body.transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (null != _neck)
        {
            // If we still have our neck
            if (Mathf.Abs(Vector3.Angle(_body.transform.TransformDirection(Vector3.forward), _directionVector)) < 30f)
            {
                // And we're facing roughly in the direction we'd like to move in, then add movement force
                Rigidbody rb = _body.GetComponent<Rigidbody>();
                rb.AddForceAtPosition(_directionVector * _force, rb.transform.TransformPoint(new Vector3(0f, _pushPointRatio, 0f)));
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Debug to show direction enemy wants to move in
        Gizmos.color = Color.white;
        Gizmos.DrawLine(_body.transform.TransformPoint(new Vector3(0f, _pushPointRatio, 0f)), _body.transform.TransformPoint(new Vector3(0f, _pushPointRatio, 0f)) + _directionVector);
    }

    private void OnDrawGizmosSelected()
    {
        // Debug to show path calculated by agent
        if (null != _agent)
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < _agent.path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(_agent.path.corners[i], _agent.path.corners[i + 1]);
            }
        }
    }

    #endregion

    #region Public Functions

    public void Die()
    {
        if (_isAlive)
        {
            // Destroy NavMeshAgent and active physics components
            Destroy(_agent.gameObject);
            Destroy(_headStabilizer);
            Destroy(_headLookStabilizer);
            Destroy(_bodyLookStabilizer);

            // The body _bodyStabilizer is kept alive, but with adjusted values to make it easier for the body to topple
            // The stabilizer will break once the body starts to fall over
            _bodyStabilizer.Strength = -3f;
            _bodyStabilizer.BreakAngle = 25f;

            // Decrease the mass to make pushing around easier
            _body.GetComponent<Rigidbody>().mass = _body.GetComponent<Rigidbody>().mass / 3f;
            _head.GetComponent<Rigidbody>().mass = _head.GetComponent<Rigidbody>().mass / 3f;

            // Change the material to show the enemy died
            _body.GetComponent<Renderer>().material = _deadMat;
            _head.GetComponent<Renderer>().material = _deadMat;

            _isAlive = false;

            // Make sure the appendages die as well
            foreach (var appendage in _appendages)
            {
                appendage.Die();
            }

            // Destroy the controller, and despawn based on the elapsed gametime
            // As the game progresses, bodies despawn faster so they can't be used as much to block other enemies
            Destroy(this);
            Destroy(gameObject, (3000f / (100f + GameManager.Instance.GameTime)));

            if (null !=_spawn)
            {
                // Notify the spawner that there's one less enemy around
                _spawn.Enemies = _spawn.Enemies - 1;
            }

            // Increase the kill counter
            GameManager.Instance.AddKill();
        }

        return;
    }

    #endregion
}
