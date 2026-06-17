using System.Collections;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    [Tooltip("amount of seconds in one day/night cycle")]
    [SerializeField] private float dayCycleTime = 1;

    [Header("resources")]
    [SerializeField] private int rats = 2;

    [Tooltip("Amount of nutrition rats start with")]
    [SerializeField] private int NutritionStarter = 100;

    [Tooltip("amount nutrition drained per rat per day")]
    [SerializeField] private int nutritionDrain;

    private int nutrition;
    private int stone;
    private int wood;
    private int metal;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        nutrition = NutritionStarter;
        StartCoroutine(DayCycle());
    }

    public IEnumerator DayCycle()
    {
        yield return new WaitForSeconds(dayCycleTime);
        nutrition = nutritionDrain * rats;
        if (nutrition < 0 || rats < 0)
        {
            Debug.Log("oops you failed");
        }

        Debug.Log("ending day...");
        StartCoroutine(DayCycle());
    }
    public void ResourceHandler(EResourceType resource, int amount)
    {
        switch (resource)
        {
            case EResourceType.Rats:
                rats += amount;
                break;
            case EResourceType.Nutrition:
                nutrition += amount;
                break;
            case EResourceType.Wood: 
                wood += amount;
                break;
            case EResourceType.Stone:
                stone += amount; 
                break;
            case EResourceType.Metal:
                stone += amount;
                break;
        }
    }

}
