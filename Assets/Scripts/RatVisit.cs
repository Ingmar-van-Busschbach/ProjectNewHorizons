using System.Collections;
using UnityEngine;

public class RatVisit : MonoBehaviour
{
    [SerializeField] private float babyMakingCooldown;
    [SerializeField] private float ratVisitorCooldown;
    [SerializeField] private GameObject ratPrefab;
    [Tooltip("spawnPosition should be just outside of screen so it seems the rat walks to colony")]
    [SerializeField] private Transform spawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(RatVisitor());
    }

    IEnumerator RatVisitor()
    {
        yield return new WaitForSeconds(ratVisitorCooldown);

        Instantiate(ratPrefab, spawnPosition.position, transform.rotation);
        StartCoroutine(RatVisitor());
    }


}
