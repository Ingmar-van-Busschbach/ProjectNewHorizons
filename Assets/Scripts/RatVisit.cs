using System.Collections;
using UnityEngine;

public class RatVisit : MonoBehaviour
{
    [SerializeField] private float ratVisitorCooldown;
    [SerializeField] private Character ratPrefab;
    [Tooltip("spawnPosition should be just outside of screen so it seems the rat walks to colony")]
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Room room;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(RatVisitor());
    }

    IEnumerator RatVisitor()
    {
        yield return new WaitForSeconds(ratVisitorCooldown);

        Character spawnedRat = Instantiate(ratPrefab, spawnPosition.position, transform.rotation);
        Transform roomLocation = room.AssignCharacter(spawnedRat);
        if (roomLocation == spawnedRat.gameObject.transform) { Destroy(spawnedRat.gameObject); }
        else
        {
            spawnedRat.MoveToLocation(roomLocation);
            ResourceManager.instance.ResourceHandler(EResourceType.Rats, 1);
        } 
        StartCoroutine(RatVisitor());
    }


}
