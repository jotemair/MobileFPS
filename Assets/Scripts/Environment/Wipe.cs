using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wipe : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _effect = null;

    public void DoWipe()
    {
        _effect.Play();
        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            enemy.Die();
        }
    }
}
