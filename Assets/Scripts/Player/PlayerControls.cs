using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerControls : MonoBehaviour
{
    #region SerializeFields

    [SerializeField]
    private Joystick _movementControl = null;

    [SerializeField]
    private Joystick _directionControl = null;

    [SerializeField]
    private float _moveSpeed = 500f;

    [SerializeField]
    private float _rotationSpeed = 100f;

    [SerializeField]
    private Transform _pitchTransform = null;

    [SerializeField]
    private float _pitchSpeed = 100f;

    [SerializeField]
    private float _pitchMin = -100f;

    [SerializeField]
    private float _pitchMax = 100f;

    #endregion

    #region Variables

    private Rigidbody _rigidbody = null;

    #endregion

    #region Functions

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(_movementControl, "Player needs movement controls");
        Assert.IsNotNull(_directionControl, "Player needs direction controls");

        Assert.IsNotNull(_pitchTransform, "Set pitch transform for player");

        _rigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(_rigidbody, "Attach Rigidbody component to player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FixedUpdate()
    {
        Vector3 movementInput = (new Vector3(_movementControl.Input.x, 0f, _movementControl.Input.y));
        _rigidbody.velocity = transform.TransformDirection(movementInput) * Time.deltaTime * _moveSpeed;

        transform.Rotate(0f, _directionControl.Input.x * Time.deltaTime * _rotationSpeed, 0f);

        float currentRotation = _pitchTransform.rotation.eulerAngles.x;
        _pitchTransform.rotation = Quaternion.Euler(MathUtils.ClampAngle(currentRotation - _directionControl.Input.y * Time.deltaTime * _pitchSpeed, _pitchMin, _pitchMax), _pitchTransform.rotation.eulerAngles.y, 0f);
        
    }

    #endregion
}
