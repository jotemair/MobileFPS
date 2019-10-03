using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// This class allows mouse input to be passed to a joystick
public class MouseJoystick : MonoBehaviour
{
    #region Private Variables

    // Speed of lerp for generated keyboard input vector values
    [SerializeField]
    private float _lerpSpeed = 7f;

    // Treshold for applying lerp, set to float.MaxValue to disable
    [SerializeField]
    private float _lerpTreshold = .2f;

    // Internal reference to joystick component
    private Joystick _joystick = null;

    // Bool to keep track of when zero input vector was sent. Input sending stops after, so mouse controls are possible again after keyboard is released
    private bool _sentZero = true;
    private Vector2 _lastInputVector = Vector2.zero;

    private bool _lockCursor = false;

    private bool _isOnPC = false;

    #endregion

    #region MonoBehaviour Functions

    private void Start()
    {
        // This might be excessive, but this script messes up controls on mobile, so I wanted to be absolutely sure it only runs on PC
        if ((Application.platform != RuntimePlatform.Android) && (Application.platform != RuntimePlatform.IPhonePlayer))
        {
            _isOnPC = true;
            _joystick = GetComponent<Joystick>();
            Assert.IsNotNull(_joystick, "KeyboardJoystick component should be placed on a GameObject with a Joystick component");
        }
        else
        {
            enabled = false;
            Destroy(this);
        }
    }

    private void Update()
    {
        // Again, making absolutely usre it only runs on PC
        if (_isOnPC)
        {
            // Lock the cursor and hide it if we are in 'game' mode
            _lockCursor = ((GameManager.Instance.GameState == GameManager.GameStates.Game) || (GameManager.Instance.GameState == GameManager.GameStates.Wipe));
            Cursor.lockState = (_lockCursor ? CursorLockMode.Locked : CursorLockMode.None);
            Cursor.visible = !_lockCursor;

            // If the cursor is locked
            if (_lockCursor)
            {
                // Get mouse input vector
                Vector2 mouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                // When generated mouse vector is over set distance away from previous value, lerp will be applied to smooth out input
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
    }

    #endregion
}
