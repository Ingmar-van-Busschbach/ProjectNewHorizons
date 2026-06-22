using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlagueSpreadRoom : Room
{
    [Header("Leave Handler")]
    [Tooltip("percentage that dies")]
    [SerializeField] private float ratDeathRate = 20;
    [Tooltip("min amount it takes for a rat to return")]
    [SerializeField] private float minReturnTime;
    [Tooltip("max amount it takes for a rat to return")]
    [SerializeField] private float maxReturnTime;
    [Tooltip("minimal amount of plague spread per rat per time")]
    [SerializeField] private int minPlague = 5;
    [Tooltip("max amount of plague spread per rat per time")]
    [SerializeField] private int maxPlague = 10;

    Character ratThatLeaves;


    [SerializeField] private Transform leaveLocation;

    public override Transform AssignCharacter(Character character)
    {
        if (unlockedRoom)
        {
            for (int i = 0; i < characterLocations.Count; i++)
            {
                if (!characterIndex.Values.Contains(i))
                {
                    characterIndex.Add(character, i);
                    character.currentRoom = this;
                    ratThatLeaves = character;
                    StartCoroutine(ratLeave());
                    return characterLocations[i];
                }
            }
        }
        return character.gameObject.transform;
    }
    IEnumerator ratLeave()
    {
        //if rat is infected
        ratThatLeaves.MoveToLocation(leaveLocation);
        int plagueAmount = Random.Range(minPlague, maxPlague);
        float returnTime = Random.Range(minReturnTime, maxReturnTime);
        yield return new WaitForSeconds(returnTime);
        float random = Random.Range(0, 100);
        Debug.Log(random);
        if (random > ratDeathRate)
        {
            ResourceManager.instance.ResourceHandler(EResourceType.Plague, plagueAmount);
            Debug.Log("plague added, amount: " +  plagueAmount);
            for (int i = 0; i < characterLocations.Count; i++)
            {
                if (!characterIndex.Values.Contains(i))
                {
                    ratThatLeaves.MoveToLocation(characterLocations[i]);
                }
            }
        }
        else
        {
            Debug.Log("rat died");
            UnassignCharacter(ratThatLeaves);
            Destroy(ratThatLeaves.gameObject);
            ResourceManager.instance.ResourceHandler(EResourceType.Rats, -1);
        }

    }
}
