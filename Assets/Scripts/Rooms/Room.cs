using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Rendering.ShadowCascadeGUI;

public class Room : MonoBehaviour
{
    [SerializeField] private List<Transform> characterLocations = new();
    [SerializeField] private Dictionary<Character, int> characterIndex = new();

    [Header("Room Resource Handler")]
    [Tooltip("This doesn't have to be changed")]
    [SerializeField] private Button collectButton;
    public ERoomType roomType;
    [Tooltip("Time in seconds it takes to produce selected resource")]
    [SerializeField] private float timeToProduce;
    [SerializeField] private int amountToProduce;

    private void Start()
    {
        StartCoroutine(RoomResourceHandler());
    }
    public Transform AssignCharacter(Character character)
    {
        for(int i = 0; i < characterLocations.Count; i++)
        {
            if (!characterIndex.Values.Contains(i))
            {
                characterIndex.Add(character, i);
                character.currentRoom = this;
                return characterLocations[i];
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

    private IEnumerator RoomResourceHandler()
    {
        yield return new WaitForSeconds(timeToProduce);
        collectButton.gameObject.SetActive(true);
        StartCoroutine(RoomResourceHandler());
    }

    public void AddResources()
    {
        switch (roomType)
        {
            case ERoomType.ResourceRoomWood:
                ResourceManager.instance.ResourceHandler(EResourceType.Wood, amountToProduce);
                break;
            case ERoomType.ResourceRoomStone:
                ResourceManager.instance.ResourceHandler(EResourceType.Stone, amountToProduce);
                break;
            case ERoomType.ResourceRoomMetal:
                ResourceManager.instance.ResourceHandler(EResourceType.Metal, amountToProduce);
                break;
            case ERoomType.NutritionRoom:
                ResourceManager.instance.ResourceHandler(EResourceType.Nutrition, amountToProduce);
                break;
        }
    }
}
