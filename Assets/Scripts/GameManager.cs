using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    private bool _isSetup = false;

    private float _gameTime = 0f;

    private bool _hitInProgress = false;

    private int _health = 0;

    private int _kills = 0;

    public enum GameStates
    {
        Main,
        Game,
        Wipe,
        Pause,
        End,
    }

    private Stack<GameStates> _gameState = new Stack<GameStates>();

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

    public void OnDestroy()
    {
        _instance = null;
    }

    public static GameManager Instance
    {
        get
        {
            if (null == _instance)
            {
                var instances = FindObjectsOfType<GameManager>();
                var count = instances.Length;
                if (count > 0)
                {
                    for (var i = 1; i < count; ++i)
                    {
                        Destroy(instances[i]);
                    }

                    _instance = instances[0];
                }
                else
                {
                    _instance = new GameObject("GameManager").AddComponent<GameManager>();
                    _instance.Setup();
                }
            }

            return _instance;
        }
    }

    public void AddKill()
    {
        if (GameState == GameStates.Game)
        {
            ++_kills;
        }
    }

    public void Start()
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

    public void Update()
    {
        if ((GameState == GameStates.Game) || (GameState == GameStates.Wipe))
        {
            _gameTime += Time.deltaTime;
        }
    }

    private void Setup()
    {
        if (!_isSetup)
        {
            _gameState.Push(GameStates.Main);

            // Projectiles should not collide with each other or the player
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Player"));
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Projectile"));

            DontDestroyOnLoad(gameObject);

            _isSetup = true;
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
        if (!_hitInProgress)
        {
            _hitInProgress = true;

            if (_health > 0)
            {
                --_health;
                _gameState.Push(GameStates.Wipe);
                FindObjectOfType<Wipe>().DoWipe();
                Invoke("PlayerHitDone", 5f);
            }
            else
            {
                GameOver();
            }
        }
    }

    private void PlayerHitDone()
    {
        if (GameState == GameStates.Wipe)
        {
            _gameState.Pop();
            _hitInProgress = false;
        }
        else
        {
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
        if ((GameState == GameStates.Game) || (GameState == GameStates.Wipe))
        {
            PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            if (null != pauseMenu)
            {
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
            Time.timeScale = 1f;
            _gameState.Pop();
        }
    }

    public void BackToMain()
    {
        _gameState.Clear();
        _gameState.Push(GameStates.Main);
        Time.timeScale = 1f;

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
