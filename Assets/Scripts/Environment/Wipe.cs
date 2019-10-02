using UnityEngine;

public class Wipe : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _effect = null;

    public void DoWipe()
    {
        // Runs a particle system effect and kills all enemies
        _effect.Play();
        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            enemy.Die();
        }
    }
}
