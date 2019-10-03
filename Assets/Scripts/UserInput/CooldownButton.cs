using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class CooldownButton : MonoBehaviour, IPointerDownHandler
{
    // Event triggered bu the button
    public UnityEvent clicked = null;

    #region Private Variables

    [SerializeField]
    private float _cooldownTime = 5f;

    private float _timer = 0f;

    [SerializeField]
    private Image _cooldownImage = null;

    #endregion

    #region MonoBehaviour Functions

    public void Start()
    {
        Assert.IsNotNull(_cooldownImage, "Cooldown script needs a cooldown image");
    }

    public void Update()
    {
        // Update timer state and button animation
        _timer = Mathf.Max(_timer - Time.deltaTime, 0f);
        _cooldownImage.fillAmount = _timer / _cooldownTime;
    }

    #endregion

    #region Interface Implementation

    public void OnPointerDown(PointerEventData eventData)
    {
        ActivateButton();
    }

    #endregion

    #region Public Functions

    public void ActivateButton()
    {
        // Check if the timer is not running
        if (0f == _timer)
        {
            // If the timer is not running the button is available
            // Start the timer and fire the connected event
            _timer = _cooldownTime;
            _cooldownImage.fillAmount = _timer / _cooldownTime;
            clicked.Invoke();
        }
    }

    #endregion
}
