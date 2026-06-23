using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    [SerializeField] private float idleRotationSpeed = 5;
    public StatPlugs statPlugs;
    public Stat stats;
    public RatNames names;

    // Components
    [HideInInspector] public Room currentRoom;
    private NavMeshAgent navMeshAgent;

    public void MoveToLocation(Transform location)
    {
        navMeshAgent.SetDestination(location.position);
    }

    public float GetRecourcefulness()
    {
        return (float)stats.recourcefulness / (statPlugs.MaxStats().recourcefulness / 2);
    }

    public float GetAthletics()
    {
        return (float)stats.athletics / (statPlugs.MaxStats().athletics / 2);
    }

    public float GetTempo()
    {
        return (float)stats.tempo / (statPlugs.MaxStats().tempo / 2);
    }

    public float GetSmarts()
    {
        return (float)stats.smarts / (statPlugs.MaxStats().smarts / 2);
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        stats = statPlugs.GenerateStats();
        name = names.GenerateName();
        navMeshAgent.speed *= GetTempo();
    }

    private void Update()
    {
        if (navMeshAgent == null)
        {
            return;
        }
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
