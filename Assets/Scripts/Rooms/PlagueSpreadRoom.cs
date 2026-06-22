using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlagueSpreadRoom : Room
{
    [Header("Leave Handler")]
    [Tooltip("amount the plague goes up per rat send out")]
    [SerializeField] private float plagueSpread;
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
        Debug.Log("character assigned");
        if (unlockedRoom)
        {
            for (int i = 0; i < characterLocations.Count; i++)
            {
                if (!characterIndex.Values.Contains(i))
                {
                    characterIndex.Add(character, i);
                    character.currentRoom = this;
                    StartCoroutine(ratLeave());
                    return characterLocations[i];
                }
            }
        }
        return character.gameObject.transform;
    }
    IEnumerator ratLeave()
    {
        Debug.Log("leave initiated");
        //if rat is infected
        ratThatLeaves.MoveToLocation(leaveLocation);
        int plagueAmount = Random.Range(minPlague, maxPlague);
        float returnTime = Random.Range(minReturnTime, maxReturnTime);
        yield return new WaitForSeconds(returnTime);

        ResourceManager.instance.ResourceHandler(EResourceType.Plague, plagueAmount);

        Debug.Log("rat came back :D");

        for (int i = 0; i < characterLocations.Count; i++)
        {
            if (!characterIndex.Values.Contains(i))
            {
                ratThatLeaves.MoveToLocation(characterLocations[i]);
            }
        }

    }
}
