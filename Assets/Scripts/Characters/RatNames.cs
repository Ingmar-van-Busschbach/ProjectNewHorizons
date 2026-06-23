using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RatNames", menuName = "ScriptableObjects/RatNames", order = 1)]
public class RatNames : ScriptableObject
{
    public List<string> firstNames;
    public List<string> surNames;

    public string GenerateName()
    {
        int index = Random.Range(0, firstNames.Count);
        string firstName = firstNames[index];
        index = Random.Range(0, surNames.Count);
        string surName = surNames[index];
        return firstName + " " + surName;
    }
}
