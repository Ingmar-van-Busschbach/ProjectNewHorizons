using System.Collections;
using TMPro;
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

    [Header("UI")]
    [SerializeField] TMP_Text nutritionCounter;
    [SerializeField] TMP_Text woodCounter;
    [SerializeField] TMP_Text stoneCounter;
    [SerializeField] TMP_Text metalCounter;

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
        nutritionCounter.text = nutrition.ToString();
        StartCoroutine(DayCycle());
    }

    public IEnumerator DayCycle()
    {
        yield return new WaitForSeconds(dayCycleTime);
        nutrition -= nutritionDrain * rats;
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
                nutritionCounter.text = nutrition.ToString();
                break;
            case EResourceType.Wood: 
                wood += amount;
                woodCounter.text = wood.ToString();
                break;
            case EResourceType.Stone:
                stone += amount;
                stoneCounter.text = stone.ToString();
                break;
            case EResourceType.Metal:
                metal += amount;
                metalCounter.text = metal.ToString();
                break;
        }
    }

}
