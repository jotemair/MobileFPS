using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _maxLife = 5f;

    private Rigidbody _rigid = null;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        Assert.IsNotNull(_rigid, "Add Rigidbody to projectile");

        _rigid.velocity = transform.TransformDirection(Vector3.forward) * _speed;
    }

    // Update is called once per frame
    void Update()
    {
        _maxLife -= Time.deltaTime;

        if (_maxLife < 0f)
        {
            Destroy(gameObject);
        }
    }
}
