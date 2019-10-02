using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    #region Singleton

    // Instance for singleton behaviour
    private static GameManager _instance = null;

    public static GameManager Instance
    {
        get
        {
            // We want to allow access to the GameManager even if it wasn't placed in the scene, in this case we create a new instance
            if (null == _instance)
            {
                // Depending on how things are setup, there might be a GameManager already in the scene that didn't yet have the time to set itself as the instance
                var instances = FindObjectsOfType<GameManager>();
                var count = instances.Length;
                if (count > 0)
                {
                    // If for some reason there are multiple GameManagers, we destroy all but the first one, and set that as the static instance
                    for (var i = 1; i < count; ++i)
                    {
                        Destroy(instances[i]);
                    }

                    _instance = instances[0];
                }
                else
                {
                    // If there aren't any GameManagers, then we create a new one
                    _instance = new GameObject("GameManager").AddComponent<GameManager>();
                }

                // Since the Instance property was accessed when it was still null,
                //   we should make sure that the Setup function of the now set static instance runs before some other script uses it
                // It's fine to call the setup function multiple times, since it has a built in check to make sure it only runs once
                _instance.Setup();
            }

            return _instance;
        }
    }

    #endregion

    #region Private Variables

    private bool _isSetup = false;

    private float _gameTime = 0f;

    private bool _hitInProgress = false;

    private int _health = 0;

    private int _kills = 0;

    // The gamestates are stored in a stack, this helps with knowing what state to go back to when a menu is closed etc
    private Stack<GameStates> _gameState = new Stack<GameStates>();

    #endregion

    #region Public Properties

    public enum GameStates
    {
        Main,
        Game,
        Wipe,
        Pause,
        End,
    }

    public GameStates GameState
    {
        get { return _gameState.Peek(); }
    }

    public int Kills
    {
        get { return _kills; }
    }

    public int Health
    {
        get { return _health; }
    }

    public float GameTime
    {
        get { return _gameTime; }
    }

    #endregion

    #region Monobehaviour Functions

    private void Start()
    {
        if (this == Instance)
        {
            Setup();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if ((GameState == GameStates.Game) || (GameState == GameStates.Wipe))
        {
            // The GameManager keeps track of total gametime spent in actual play mode, and not in the pause menu
            _gameTime += Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        // If the static instance is destroyed, set the static instance reference to null
        if (this == Instance)
        {
            _instance = null;
        }
    }

    #endregion

    #region Private Functions

    private void Setup()
    {
        // The setup function should only run once
        if (!_isSetup)
        {
            // The game starts in the Main(menu) state
            _gameState.Push(GameStates.Main);

            // Projectiles should not collide with each other or the player
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Player"));
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Projectile"));

            // Keep instance alive through scene changes
            DontDestroyOnLoad(gameObject);

            _isSetup = true;
        }
    }

    #endregion

    #region Public Functions

    public void AddKill()
    {
        if (GameState == GameStates.Game)
        {
            ++_kills;
        }
    }

    public void StartGame()
    {
        _gameState.Push(GameStates.Game);
        _gameTime = 0f;
        _health = 3;
        _kills = 0;
        _hitInProgress = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Level");
    }

    public void PlayerHit()
    {
        // Only hit the player, if a hit isn't already in progress
        if (!_hitInProgress)
        {
            _hitInProgress = true;

            if (_health > 0)
            {
                // If the player still has health, decrease it, and activate the wipe effect
                --_health;
                _gameState.Push(GameStates.Wipe);
                FindObjectOfType<Wipe>().DoWipe();
                Invoke("PlayerHitDone", 5f);
            }
            else
            {
                // If the player has no health remaining, then game over
                GameOver();
            }
        }
    }

    private void PlayerHitDone()
    {
        if (GameState == GameStates.Wipe)
        {
            // If we're in the Wipe state, we can stop
            _gameState.Pop();
            _hitInProgress = false;
        }
        else
        {
            // We might be in the paused state, so we should try again in a bit
            Invoke("PlayerHitDone", 1f);
        }
    }

    private void GameOver()
    {
        _gameState.Push(GameStates.End);

        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (null != pauseMenu)
        {
            Time.timeScale = 0f;
            pauseMenu.OpenAsEndMenu();
        }
    }

    public void PauseGame()
    {
        // Only open the pasue menu, if we're in play mode
        if ((GameState == GameStates.Game) || (GameState == GameStates.Wipe))
        {
            PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            if (null != pauseMenu)
            {
                // Zero the timescale and open the pause menu
                Time.timeScale = 0f;
                _gameState.Push(GameStates.Pause);
                pauseMenu.OpenMenu();
            }
        }
    }

    public void ResumeGame()
    {
        if (GameState == GameStates.Pause)
        {
            // Restore the timescale and go back to the previous gamestate
            Time.timeScale = 1f;
            _gameState.Pop();
        }
    }

    public void BackToMain()
    {
        // Reset the gamestate back to the main and restore the timescale in case we were paused
        _gameState.Clear();
        _gameState.Push(GameStates.Main);
        Time.timeScale = 1f;

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    #endregion
}
