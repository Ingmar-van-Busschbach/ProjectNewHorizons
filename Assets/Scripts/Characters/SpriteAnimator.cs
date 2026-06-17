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
        if(transform.parent != null)
        {
            navMeshAgent = transform.parent.gameObject.GetComponent<NavMeshAgent>();
        }
    }
    void Update()
    {
        if (navMeshAgent != null)
        {
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        }
    }
}
