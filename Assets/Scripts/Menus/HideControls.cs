using UnityEngine;

public class HideControls : MonoBehaviour
{
    [SerializeField]
    private RectTransform _movementControls = null;

    [SerializeField]
    private RectTransform _directionControls = null;

    [SerializeField]
    private RectTransform _pauseControls = null;

    [SerializeField]
    private float _pcLookSpeed = 60f;

    void Start()
    {
        if ((Application.platform != RuntimePlatform.Android) && (Application.platform != RuntimePlatform.IPhonePlayer))
        {
            // If we're not on a PC, hide the joystick controls off screen
            // We don't remove these controls, since the keyboard and mouse controls actually feed into the joystick controls
            // The reason it's set up this way, is that it allows to test the joystick controls on PC with a keyboard and a mouse
            _movementControls.anchoredPosition = new Vector2(-100f, -100f);
            _movementControls.sizeDelta = new Vector2(0, 0);

            _directionControls.anchoredPosition = new Vector2(-100f, -100f);
            _directionControls.sizeDelta = new Vector2(0, 0);

            _pauseControls.anchoredPosition = new Vector2(-100f, -100f);
            _pauseControls.sizeDelta = new Vector2(0, 0);

            // Tweek the mouse look speed to work better for PC
            FindObjectOfType<PlayerControls>().SetLookSpeed(_pcLookSpeed);

            foreach (var controller in FindObjectsOfType<MasterController>())
            {
                controller.DeadZone = 0f;
            }
        }
    }
}
