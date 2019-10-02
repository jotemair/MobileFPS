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

    void Update()
    {
        // Has three Image components, and turns the on or off depending on how many hits the player has left
        _hp1.enabled = (GameManager.Instance.Health >= 1);
        _hp2.enabled = (GameManager.Instance.Health >= 2);
        _hp3.enabled = (GameManager.Instance.Health >= 3);
    }
}
