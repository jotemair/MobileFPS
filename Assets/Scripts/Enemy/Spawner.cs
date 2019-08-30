using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
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

    public int Enemies
    {
        get { return _currentEnemies; }
        set { _currentEnemies = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _spawnLocations = new List<Transform>(GetComponentsInChildren<Transform>());
        SpawnX(10);
    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer = Mathf.Max(0f, _spawnTimer - Time.deltaTime);

        if (0f == _spawnTimer)
        {
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

    private void SpawnX(int x)
    {
        List<int> nums = new List<int>();
        for(int i = 0; i < _spawnLocations.Count; ++i)
        {
            nums.Add(i);
        }

        int idx = Random.Range(0, nums.Count);
        int spawned = 0;
        while (spawned < x)
        {
            if (Spawn(_spawnLocations[nums[idx]], _enemies[Random.Range(0, _enemies.Count)]))
            {
                ++spawned;
                ++_currentEnemies;
            }

            nums.RemoveAt(idx);
            if (0 == nums.Count)
            {
                break;
            }

            idx = Random.Range(0, nums.Count);
        }
    }

    private PlayerControls GetPlayer()
    {
        if (null == _player)
        {
            _player = FindObjectOfType<PlayerControls>();
        }

        return _player;
    }

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

    private bool PlayerIsNearOrLooking(Vector3 loc, float distance, float angle)
    {
        bool observed = false;

        Transform player = GetPlayer().transform;

        observed = (Vector3.Distance(loc, player.position) < distance);

        if (!observed)
        {
            observed = (Mathf.Abs(Vector3.Angle(loc - player.position, player.TransformDirection(Vector3.forward))) < angle);
        }

        return observed;
    }

    private bool Spawn(Transform location, GameObject enemy)
    {
        bool spawned = false;

        if (!CheckForTagInRange(location.position, "Target", 2f) && !PlayerIsNearOrLooking(location.position, 20f, 30f))
        {
            GameObject obj = Instantiate(enemy, location.position, location.rotation);
            obj.GetComponent<EnemyController>().Spawn = this;
            spawned = true;
        }

        return spawned;
    }
}
