using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField]
    private Image _hp1 = null;

    [SerializeField]
    private Image _hp2 = null;

    [SerializeField]
    private Image _hp3 = null;

    // Update is called once per frame
    void Update()
    {
        _hp1.enabled = (GameManager.Instance.Health >= 1);
        _hp2.enabled = (GameManager.Instance.Health >= 2);
        _hp3.enabled = (GameManager.Instance.Health >= 3);
    }
}
