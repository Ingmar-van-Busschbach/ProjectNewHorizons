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
    [SerializeField] private int amountToProduce;

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
