using System.Collections;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private ResourceManager instance;

    [Tooltip("amount of seconds in one day/night cycle")]
    [SerializeField] private float dayCycleTime = 1;

    [Header("resources")]
    [SerializeField] private int rats = 2;

    [Tooltip("Amount of food and water rats start with")]
    [SerializeField] private int necessityStarter = 100;

    [Tooltip("amount food drained per rat per day")]
    [SerializeField] private int foodDrain;

    [Tooltip("amount water drained per rat per day")]
    [SerializeField] private int waterDrain;


    private int stone;
    private int water;
    private int food;

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
        water = necessityStarter;
        food = necessityStarter;
        StartCoroutine(DayCycle());
    }

    public IEnumerator DayCycle()
    {
        yield return new WaitForSeconds(dayCycleTime);
        food -= foodDrain * rats;
        water -= waterDrain * rats;
        if (food < 0 || water < 0 || rats < 0)
        {
            Debug.Log("oops you failed");
        }

        Debug.Log("ending day...");
        StartCoroutine(DayCycle());
    }
    public void resourceHandler(EResourceType resource, int amount)
    {
        switch (resource)
        {
            case EResourceType.Stone:
                stone += amount; 
                break;
            case EResourceType.Water:
                water += amount;
                break;
            case EResourceType.Food:
                stone += amount;
                break;
            case EResourceType.Rats:
                rats += amount;
                break;
        }
    }

}
