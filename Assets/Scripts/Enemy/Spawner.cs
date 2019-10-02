using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Private Variables

    [SerializeField]
    private List<GameObject> _enemies = new List<GameObject>();

    private List<Transform> _spawnLocations = null;

    private PlayerControls _player = null;

    [SerializeField]
    private int _maxEnemies = 50;

    private int _currentEnemies = 0;

    [SerializeField]
    private float _spawnTime = 5f;

    [SerializeField]
    private float _spawnSpeedup = 0.9f;

    private float _spawnTimer = 0f;

    #endregion

    #region Public Properties

    public int Enemies
    {
        get { return _currentEnemies; }
        set { _currentEnemies = value; }
    }

    #endregion

    #region Monobehaviour Functions

    // Start is called before the first frame update
    void Start()
    {
        // The way the spawner script is setup, it uses all child transform positions as possible spawn locations
        _spawnLocations = new List<Transform>(GetComponentsInChildren<Transform>());
    }

    // Update is called once per frame
    void Update()
    {
        // Only run if we're in play mode (wipe mode disables spawns as well)
        if (GameManager.GameStates.Game == GameManager.Instance.GameState)
        {
            _spawnTimer = Mathf.Max(0f, _spawnTimer - Time.deltaTime);

            if (0f == _spawnTimer)
            {
                // After every spawn, the spawn time is shortened to increase difficulty over time
                _spawnTimer = _spawnTime;
                _spawnTime = _spawnTime * _spawnSpeedup;

                if (_spawnTime < 0.1f)
                {
                    _spawnTime = 0f;
                }

                if (_currentEnemies < _maxEnemies)
                {
                    SpawnX(2);
                }
            }
        }
    }

    #endregion

    #region Private Functions

    // Helper function to spawn X number of enemies
    private void SpawnX(int x)
    {
        // We create a list containing the index of all spawn points
        // We'll select randomly from this list, and remove indexes that failed to spawn an enemy until we find valid spots or we checked all
        List<int> nums = new List<int>();
        for(int i = 0; i < _spawnLocations.Count; ++i)
        {
            nums.Add(i);
        }

        int idx = Random.Range(0, nums.Count);
        int spawned = 0;
        while (spawned < x)
        {
            // There's an increasing chance to spawn harder version of enemies as time goes on
            // The first half of the enemies list is the easy version, the second half is the harder versions
            float hardModeChance = Mathf.Atan(GameManager.Instance.GameTime / 100f) * Random.Range(0f, 1f);

            if (Spawn(_spawnLocations[nums[idx]], _enemies[Random.Range(0, _enemies.Count / 2) + (hardModeChance > 0.7f ? _enemies.Count / 2 : 0)]))
            {
                // If the spawn is successful, increase spawned count
                ++spawned;
                ++_currentEnemies;
            }

            // Remove the spawn point we just used, and then select a new one
            nums.RemoveAt(idx);
            if (0 == nums.Count)
            {
                break;
            }

            idx = Random.Range(0, nums.Count);
        }
    }

    // Helper function to get the player
    private PlayerControls GetPlayer()
    {
        if (null == _player)
        {
            _player = FindObjectOfType<PlayerControls>();
        }

        return _player;
    }

    // Helper function to check if a specific tag is in range
    // Used to see if a spawn point alreadzy has an enemy on it
    private bool CheckForTagInRange(Vector3 searchPos, string searchTag, float range)
    {
        bool match = false;

        Collider[] colliders = Physics.OverlapSphere(searchPos, range);
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag(searchTag))
            {
                match = true;
                break;
            }
        }

        return match;
    }

    // Helper function to determine if the player is within a given range, or looking within a given range of a point
    private bool PlayerIsNearOrLooking(Vector3 loc, float distance, float angle)
    {
        bool observed = false;

        Transform player = GetPlayer().transform;

        // First check if the player is near
        observed = (Vector3.Distance(loc, player.position) < distance);

        if (!observed)
        {
            // If the player is not near, check if they are looking towards the given point
            observed = (Mathf.Abs(Vector3.Angle(loc - player.position, player.TransformDirection(Vector3.forward))) < angle);
        }

        return observed;
    }

    // Helper function to spawn one enemy, returns if it was successful or not
    private bool Spawn(Transform location, GameObject enemy)
    {
        bool spawned = false;

        if (!CheckForTagInRange(location.position, "Target", 2f) && !PlayerIsNearOrLooking(location.position, 20f, 30f))
        {
            // Only spawns an enemy if there's no enemy already at the spawn location, and the player isn't near, or looking at the spawn location 
            GameObject obj = Instantiate(enemy, location.position, location.rotation);
            obj.GetComponent<EnemyController>().Spawn = this;
            spawned = true;
        }

        return spawned;
    }

    #endregion
}
