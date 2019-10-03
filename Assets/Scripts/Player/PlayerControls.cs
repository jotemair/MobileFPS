using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControls : MonoBehaviour
{
    #region Private Variables

    // Control is handled through Joystick, PC controls feed to the player control script through the Joystick script

    [SerializeField]
    private Joystick _movementControl = null;

    [SerializeField]
    private Joystick _directionControl = null;

    [SerializeField]
    private float _moveSpeed = 500f;

    [SerializeField]
    private float _rotationSpeed = 100f;

    // Pitch is handled by actually moving the head of the player character, to which the camera is attached

    [SerializeField]
    private Transform _pitchTransform = null;

    [SerializeField]
    private float _pitchSpeed = 100f;

    [SerializeField]
    private float _pitchMin = -100f;

    [SerializeField]
    private float _pitchMax = 100f;

    private Rigidbody _rigidbody = null;

    #endregion

    #region MonoBehaviour Functions

    private void Start()
    {
        // Initial setup and checks
        Assert.IsNotNull(_movementControl, "Player needs movement controls");
        Assert.IsNotNull(_directionControl, "Player needs direction controls");

        Assert.IsNotNull(_pitchTransform, "Set pitch transform for player");

        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Move the player by changing the velocity
        Vector3 movementInput = (new Vector3(_movementControl.Input.x, 0f, _movementControl.Input.y));
        _rigidbody.velocity = transform.TransformDirection(movementInput) * Time.deltaTime * _moveSpeed;

        // Rotate the player based on the rotation input
        transform.Rotate(0f, _directionControl.Input.x * Time.deltaTime * _rotationSpeed, 0f);

        // Calculate the new pitch value, and clamp it between the given range, then apply it to the head/camera
        float currentRotation = _pitchTransform.rotation.eulerAngles.x;
        float newRotation = MathUtils.ClampAngle(currentRotation - _directionControl.Input.y * Time.deltaTime * _pitchSpeed, _pitchMin, _pitchMax);
        _pitchTransform.rotation = Quaternion.Euler(newRotation, _pitchTransform.rotation.eulerAngles.y, 0f);
    }

    #endregion

    #region Public Functions

    public void SetLookSpeed(float speed)
    {
        _pitchSpeed = speed;
        _rotationSpeed = speed;
    }

    #endregion
}
