using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        if ((Application.platform != RuntimePlatform.Android) && (Application.platform != RuntimePlatform.IPhonePlayer))
        {
            _movementControls.anchoredPosition = new Vector2(-100f, -100f);
            _movementControls.sizeDelta = new Vector2(0, 0);

            _directionControls.anchoredPosition = new Vector2(-100f, -100f);
            _directionControls.sizeDelta = new Vector2(0, 0);

            _pauseControls.anchoredPosition = new Vector2(-100f, -100f);
            _pauseControls.sizeDelta = new Vector2(0, 0);

            FindObjectOfType<PlayerControls>().SetLookSpeed(_pcLookSpeed);

            foreach (var controller in FindObjectsOfType<MasterController>())
            {
                controller.DeadZone = 0f;
            }
        }
    }
}
