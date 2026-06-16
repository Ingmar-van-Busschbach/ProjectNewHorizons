using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    public Room currentRoom;
    private NavMeshAgent navMeshAgent;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void MoveToLocation(Transform location)
    {
        navMeshAgent.SetDestination(location.position);
    }
}
