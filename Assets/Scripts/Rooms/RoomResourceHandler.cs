using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomResourceHandler : Room
{
    [Header("Room Resource Handler")]
    [Tooltip("This doesn't have to be changed")]
    [SerializeField] private Button collectButton;
    public ERoomType roomType;
    [Tooltip("Time in seconds it takes to produce selected resource")]
    [SerializeField] private float timeToProduce;
    [Header("Amount to produce per resource")]
    [SerializeField] private int nutritionAmount;
    [SerializeField] private int woodAmount;
    [SerializeField] private int stoneAmount;
    [SerializeField] private int metalAmount;

    private void Start()
    {
        StartCoroutine(ResourceHandler());
    }
    private IEnumerator ResourceHandler()
    {
        yield return new WaitForSeconds(timeToProduce);
        collectButton.gameObject.SetActive(true);
        StartCoroutine(ResourceHandler());
    }

    public void AddResources()
    {
        switch (roomType)
        {
            case ERoomType.NutritionRoom:
                ResourceManager.instance.ResourceHandler(EResourceType.Nutrition, nutritionAmount);
                break;
            case ERoomType.ResourceRoomWood:
                ResourceManager.instance.ResourceHandler(EResourceType.Wood, woodAmount);
                break;
            case ERoomType.ResourceRoomStone:
                ResourceManager.instance.ResourceHandler(EResourceType.Stone, stoneAmount);
                break;
            case ERoomType.ResourceRoomMetal:
                ResourceManager.instance.ResourceHandler(EResourceType.Metal, metalAmount);
                break; 
        }
    }


}
