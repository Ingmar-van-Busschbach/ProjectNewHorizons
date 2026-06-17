using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    [SerializeField] private float idleRotationSpeed = 5;
    // Components
    [HideInInspector] public Room currentRoom;
    private NavMeshAgent navMeshAgent;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void MoveToLocation(Transform location)
    {
        navMeshAgent.SetDestination(location.position);
    }

    private void Update()
    {
        if (!navMeshAgent.pathPending)
        {
            // Check if the remaining distance is within the stopping distance
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                // Verify the agent actually has a path and isn't just stopped
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    StartCoroutine(OnArrival());
                }
            }
        }
    }

    private IEnumerator OnArrival()
    {
        Quaternion targetRotation = Vector3.Dot(transform.right, Vector3.right) < 0 ? Quaternion.Euler(0, 90, 0) : Quaternion.Euler(0, -90, 0);
        while (Quaternion.Angle(transform.rotation, targetRotation) > 5)
        {
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, idleRotationSpeed * Time.fixedDeltaTime);
        }
    }
}
