using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private List<Transform> characterLocations = new();
    [SerializeField] protected Dictionary<Character, int> characterIndex = new();
    public bool unlockedRoom;
    public string roomName;
    public int woodToUnlock;
    public int stoneToUnlock;
    public int metalToUnlock;
    public Sprite ratHat;

    public Transform AssignCharacter(Character character)
    {
        if (unlockedRoom)
        {
            for (int i = 0; i < characterLocations.Count; i++)
            {
                if (!characterIndex.Values.Contains(i))
                {
                    characterIndex.Add(character, i);
                    character.currentRoom = this;
                    return characterLocations[i];
                }
            }
        }
        return character.gameObject.transform;
    }
    public void UnassignCharacter(Character character)
    {
        if (characterIndex.Keys.Contains(character))
        {
            characterIndex.Remove(character);
        }
    }
}
