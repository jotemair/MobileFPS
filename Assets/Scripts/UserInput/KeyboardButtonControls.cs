using UnityEngine;
using UnityEngine.Events;

// Class to connect a keyboard button press to an event
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
