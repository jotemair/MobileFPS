using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickControls : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GetComponent<CooldownButton>().StartCooldown();
        }
    }
}
