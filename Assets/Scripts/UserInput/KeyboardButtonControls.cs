using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyboardButtonControls : MonoBehaviour
{
    [SerializeField]
    private KeyCode _key = KeyCode.None;

    public UnityEvent pressed;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_key))
        {
            pressed.Invoke();
        }
    }
}
