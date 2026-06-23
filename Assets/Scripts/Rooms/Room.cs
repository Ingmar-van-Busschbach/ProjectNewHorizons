using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public AudioSource roomAmbience;
    [SerializeField] private GameObject roomlight;
    [SerializeField] protected List<Transform> characterLocations = new();
    public Dictionary<Character, int> characterIndex = new();
    public bool unlockedRoom;
    public string roomName;
    public int woodToUnlock;
    public int stoneToUnlock;
    public int metalToUnlock;
    public Sprite ratHat;

    private void Update()
    {
        if (characterIndex.Count > 0)
        {
            roomAmbience.gameObject.SetActive(true);
            roomlight.SetActive(true);
        }
        else
        {
            roomAmbience.gameObject.SetActive(false);
            roomlight.SetActive(false);
        }
    }
    public virtual Transform AssignCharacter(Character character)
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
