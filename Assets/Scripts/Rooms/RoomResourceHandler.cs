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
    public float timeToProduce;
    [Header("Amount to produce per resource")]
    public int nutritionAmount;
    public int woodAmount;
    public int stoneAmount;
    public int metalAmount;
    public int plagueVialAmount;
    public bool canSwitchResourceTypes;
    [Tooltip("The effectiveness the stats have when at minimum and maximum stat values. Should be a number between 0-1 and 1-2, with both being the same value away from 1.")]
    [SerializeField] private Vector2 statEffectiveness;
    

    [HideInInspector] public float currentTime;

    private void Start()
    {
        StartCoroutine(ResourceHandler());
    }
    private IEnumerator ResourceHandler()
    {
        collectButton.gameObject.SetActive(false);
        currentTime = timeToProduce;
        while(currentTime > 0)
        {
            yield return new WaitForSeconds(0.1f);
            float averageStat = 0;
            foreach(Character character in characterIndex.Keys)
            {
                averageStat += character.GetRecourcefulness();
            }
            if(characterIndex.Count > 0)
            {
                averageStat = averageStat / characterIndex.Count;
            }
            else
            {
                averageStat = 1;
            }
            averageStat = RemapFloat(averageStat, new Vector2(0, 2), statEffectiveness);
            currentTime -= 0.1f * characterIndex.Count * averageStat;
        }
        collectButton.gameObject.SetActive(true);
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
            case ERoomType.ResearchRoom:
                ResourceManager.instance.ResourceHandler(EResourceType.PlagueVials, plagueVialAmount);
                break;
        }
        StartCoroutine(ResourceHandler());
    }

    public float RemapFloat(float value, Vector2 rangeA, Vector2 rangeB)
    {
        float t = Mathf.InverseLerp(rangeA.x, rangeA.y, value);
        return Mathf.Lerp(rangeB.x, rangeB.y, t);
    }
}
