using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    #region Private Variables

    [SerializeField]
    private TMP_Text _time = null;

    [SerializeField]
    private TMP_Text _kills = null;

    [SerializeField]
    private GameObject _pauseMenu = null;

    [SerializeField]
    private GameObject _resumeButton = null;

    [SerializeField]
    private TMP_Text _menuTitle = null;

    #endregion

    #region Private Functions

    private void SetupText()
    {
        // Update the text in the Pause Menu with the current information
        int totalSeconds = (int)GameManager.Instance.GameTime;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        _time.text = minutes.ToString() + " : " + seconds.ToString();
        _kills.text = "Kills: " + GameManager.Instance.Kills.ToString();
    }

    #endregion

    #region Public Functions

    public void OpenMenu()
    {
        GameManager.Instance.PauseGame();
        _pauseMenu.SetActive(true);
        SetupText();
        _menuTitle.text = "Paused";
        _resumeButton.SetActive(true);
    }

    public void OpenAsEndMenu()
    {
        // The Game Over menu is actually the pause menu, just with a different title, and the resume button hidden
        _pauseMenu.SetActive(true);
        SetupText();
        _menuTitle.text = "Game Over";
        _resumeButton.SetActive(false);
    }

    public void CloseMenu()
    {
        _pauseMenu.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    public void OnMainMenuButtonPressed()
    {
        _pauseMenu.SetActive(false);
        GameManager.Instance.BackToMain();
    }

    public void ToggleMenu()
    {
        if (GameManager.Instance.GameState == GameManager.GameStates.Pause)
        {
            CloseMenu();
        }
        else if ((GameManager.Instance.GameState == GameManager.GameStates.Game) || (GameManager.Instance.GameState == GameManager.GameStates.Wipe))
        {
            OpenMenu();
        }
    }

    #endregion
}
