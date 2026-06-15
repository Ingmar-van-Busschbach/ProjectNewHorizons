using System.Collections;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private int rats = 2; 
    [SerializeField] private int food = 100;
    [Tooltip("amount food drained per rat per day")]
    [SerializeField] private int foodDrain;
    [SerializeField] private int water = 100;
    [Tooltip("amount water drained per rat per day")]
    [SerializeField] private int waterDrain;

    private int stone;
    public void EndDay()
    {
        if (food - foodDrain * rats < 0 || water - waterDrain * rats < 0)
        {
            Debug.Log("oops you failed");
        }
        food -= foodDrain * rats;
        water -= waterDrain * rats;
    }
    public void resourceHandler(EResourceType resource, int amount)
    {
        switch (resource)
        {
            case EResourceType.Stone:
                stone += amount; 
                break;
        }
    }

}
