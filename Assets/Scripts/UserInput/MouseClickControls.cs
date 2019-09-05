using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public UnityEvent clicked;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown((int)_button))
        {
            clicked.Invoke();
        }
    }
}
