using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    public void Awake()
    {
        Assert.IsNull(_instance, "There should only be one GameManager instance, but it already exists");
        _instance = this;
    }

    public void OnDestroy()
    {
        _instance = null;
    }

    public static GameManager Instance
    {
        get { return _instance; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Projectiles should not collide with each other or the player
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Player"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Projectile"));
    }
}
