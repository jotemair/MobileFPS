using UnityEngine;
using UnityEngine.Events;

// Class to connect a mouse click to an event for PC controls
public class MouseClickControls : MonoBehaviour
{
    public enum MouseButtons
    {
        Primary = 0,
        Secondary = 1,
        Middle = 2,
    };

    [SerializeField]
    private MouseButtons _button = MouseButtons.Primary;

    public UnityEvent clicked = null;

    void Update()
    {
        if (Input.GetMouseButtonDown((int)_button))
        {
            clicked.Invoke();
        }
    }
}
