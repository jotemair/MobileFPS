using UnityEngine;
using UnityEngine.AI;

public class NavMeshHandler : MonoBehaviour
{
    void Start()
    {
        // Regenerate the Navmesh at the start
        // Usefull in case dynamically generated levels are introduced
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
