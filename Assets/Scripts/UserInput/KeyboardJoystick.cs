using UnityEngine;
using UnityEngine.Assertions;

public class KeyboardJoystick : MonoBehaviour
{
    #region SerialiseFields

    // Speed of lerp for generated keyboard input vector values
    [SerializeField]
    private float _lerpSpeed = 7f;

    // Treshold for applying lerp, set to float.MaxValue to disable
    [SerializeField]
    private float _lerpTreshold = .2f;

    #endregion

    #region Variables

    // Internal reference to joystick component
    private Joystick _joystick = null;

    // Bool to keep track of when zero input vector was sent. Input sending stops after, so mouse controls are possible again after keyboard is released
    private bool _sentZero = true;
    private Vector2 _lastInputVector = Vector2.zero;

    #endregion

    #region Functions

    private void Start()
    {
        _joystick = GetComponent<Joystick>();
        Assert.IsNotNull(_joystick, "KeyboardJoystick component should be placed on a GameObject with a Joystick component");
    }

    // Update is called once per frame
    void Update()
    {
        // Get keyboard input vector
        Vector2 keyboardVector = new Vector2(UnityEngine.Input.GetAxis("Horizontal"), UnityEngine.Input.GetAxis("Vertical"));

        // When generated keyboard vector is over set distance away from previous value, lerp will be applied to smooth out input
        if (Vector2.Distance(_lastInputVector, keyboardVector) > _lerpTreshold)
        {
            keyboardVector = Vector2.Lerp(_lastInputVector, keyboardVector, Time.deltaTime * _lerpSpeed);
        }
        _lastInputVector = keyboardVector;

        // Check if we have a zero input vector
        bool hasZeroMagnitude = (Vector2.zero == keyboardVector);

        // Only send input if it's not zero, or if we did not send zero yet
        // This allows mouse controls when keyboard is not used
        if (!hasZeroMagnitude || !_sentZero)
        {
            _joystick.HandleInput(keyboardVector);
            _sentZero = hasZeroMagnitude;
        }
    }

    #endregion
}
