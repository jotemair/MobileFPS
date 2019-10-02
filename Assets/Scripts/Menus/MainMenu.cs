using UnityEngine;

public class MainMenu : MonoBehaviour
{
    #region Private Variables

    [SerializeField]
    private GameObject _startButton = null;

    [SerializeField]
    private GameObject _howtoButton = null;

    [SerializeField]
    private GameObject _pcHowTo = null;

    [SerializeField]
    private GameObject _mobileHowTo = null;

    #endregion

    #region Public Functions

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

    #endregion
}
