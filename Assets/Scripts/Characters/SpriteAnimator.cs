using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class SpriteAnimator : MonoBehaviour
{
    Animator animator;
    NavMeshAgent navMeshAgent;
    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = transform.parent.GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);   
    }
}
