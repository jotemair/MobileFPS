using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MouseJoystick : MonoBehaviour
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

    private bool _lockCursor = false;

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
        if (Input.GetKeyDown(KeyCode.M))
        {
            _lockCursor = !_lockCursor;
            Cursor.lockState = (_lockCursor ? CursorLockMode.Locked : CursorLockMode.None);
        }

        if (_lockCursor)
        {
            // Get mouse input vector
            Vector2 mouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            // When generated keyboard vector is over set distance away from previous value, lerp will be applied to smooth out input
            if (Vector2.Distance(_lastInputVector, mouseVector) > _lerpTreshold)
            {
                mouseVector = Vector2.Lerp(_lastInputVector, mouseVector, Time.deltaTime * _lerpSpeed);
            }
            _lastInputVector = mouseVector;

            // Check if we have a zero input vector
            bool hasZeroMagnitude = (Vector2.zero == mouseVector);

            // Only send input if it's not zero, or if we did not send zero yet
            // This allows mouse controls when keyboard is not used
            if (!hasZeroMagnitude || !_sentZero)
            {
                _joystick.HandleInput(mouseVector);
                _sentZero = hasZeroMagnitude;
            }
        }
    }

    #endregion
}
