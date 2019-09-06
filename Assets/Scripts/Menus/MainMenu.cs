using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _startButton = null;

    [SerializeField]
    private GameObject _howtoButton = null;

    [SerializeField]
    private GameObject _pcHowTo = null;

    [SerializeField]
    private GameObject _mobileHowTo = null;

    public void OnStartButtonClicked()
    {
        GameManager.Instance.StartGame();
    }

    public void OnHowToButtonClicked()
    {
        _startButton.SetActive(false);
        _howtoButton.SetActive(false);
        if ((Application.platform != RuntimePlatform.Android) && (Application.platform != RuntimePlatform.IPhonePlayer))
        {
            _pcHowTo.SetActive(true);
        }
        else
        {
            _mobileHowTo.SetActive(true);
        }
    }

    public void OnBackButtonClicked()
    {
        _startButton.SetActive(true);
        _howtoButton.SetActive(true);
        _pcHowTo.SetActive(false);
        _mobileHowTo.SetActive(false);
    }
}
