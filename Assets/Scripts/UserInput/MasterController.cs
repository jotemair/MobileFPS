using UnityEngine;
using UnityEngine.EventSystems;

public class MasterController : Joystick
{
    #region SerialiseFields

    // Treshold before dynamic joystick anchor position moves
    [SerializeField]
    private float _moveTreshold = 1f;

    // Joystick type, with base joystick script providing the Fixed functionality, and master controller allowing for the added Floating and Dynamic behaviors
    [SerializeField]
    private JoystickType _joystickType = JoystickType.Fixed;

    #endregion

    #region Variables

    private Vector2 _fixedPosition = Vector2.zero;

    #endregion

    #region Properties

    // The move threshold of the dynamic joystick
    public float MoveTreshold
    {
        get { return _moveTreshold; }
        set { _moveTreshold = Mathf.Abs(value); }
    }

    // The type of the joystick, for setting, the SetType function should be used
    public JoystickType Type
    {
        get { return _joystickType; }
        set { SetType(value); }
    }

    #endregion

    #region Functions

    private void SetType(JoystickType joystickType)
    {
        this._joystickType = joystickType;

        if ((JoystickType.Fixed == _joystickType) || (JoystickType.LocalFloating == _joystickType))
        {
            _background.anchoredPosition = _fixedPosition;
            _background.gameObject.SetActive(true);
        }
        else
        {
            _background.gameObject.SetActive(false);
        }
    }

    protected override void Start()
    {
        base.Start();
        _fixedPosition = _background.anchoredPosition;
        Type = _joystickType;
        MoveTreshold = _moveTreshold;
    }

    public override void HandleInput(Vector2 rawInput)
    {
        float magnitude = rawInput.magnitude;
        Vector2 radius = _background.sizeDelta / 2f;

        if ((JoystickType.Dynamic == _joystickType) && (magnitude > _moveTreshold))
        {
            Vector2 diff = rawInput.normalized * (magnitude - MoveTreshold) * radius;
            _background.anchoredPosition += diff;
        }
        base.HandleInput(rawInput);
    }

    #endregion

    #region Interface

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (JoystickType.LocalFloating == _joystickType)
        {
            _background.anchoredPosition = _fixedPosition;
            Vector2 rawInput = GetRawInputFromEvent(eventData);
            float magnitude = rawInput.magnitude;

            if (magnitude < _moveTreshold)
            {
                _background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            }
        }
        else if (JoystickType.Fixed != _joystickType)
        {
            _background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            _background.gameObject.SetActive(true);
        }
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (JoystickType.LocalFloating == _joystickType)
        {
            _background.anchoredPosition = _fixedPosition;
        }
        else if (JoystickType.Fixed != _joystickType)
        {
            _background.gameObject.SetActive(false);
        }

        base.OnPointerUp(eventData);
    }

    #endregion
}

public enum JoystickType
{
    Fixed,          // The joystick is always displayed, the base does not move
    Floating,       // The joystick is only displayed on input, the base is fixed to the position of the first input for each new input
    Dynamic,        // The joystick is only displayed on input, thebase starts at the first user input position, but will follow the input location if it gets too far
    LocalFloating   // The joystick is always displayed, the behaviour is the same as for Floating when the user input starts close to the original center position
}
