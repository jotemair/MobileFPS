using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class CooldownButton : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent clicked;

    [SerializeField]
    private float _cooldownTime = 5f;

    private float _timer = 0f;

    [SerializeField]
    private Image _cooldownImage = null;

    public void Start()
    {
        Assert.IsNotNull(_cooldownImage, "Cooldown script needs a cooldown image");
    }

    // Update is called once per frame
    public void Update()
    {
        _timer = Mathf.Max(_timer - Time.deltaTime, 0f);
        _cooldownImage.fillAmount = _timer / _cooldownTime;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCooldown();
    }

    public void StartCooldown()
    {
        if (0f == _timer)
        {
            _timer = _cooldownTime;
            _cooldownImage.fillAmount = _timer / _cooldownTime;
            clicked.Invoke();
        }
    }
}
