using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StatPlugs", menuName = "ScriptableObjects/StatPlugs", order = 1)]
public class StatPlugs : ScriptableObject
{
    public List<Stat> plugA;
    public List<Stat> plugB;
    public int layers = 2;
    public Stat maxStats;

    public Stat GenerateStats()
    {
        Stat stat = new Stat();
        for(int i  = 0; i < layers; i++)
        {
            int plugASelection = Random.Range(0, plugA.Count);
            int plugBSelection = Random.Range(0, plugB.Count);
            stat.Add(plugA[plugASelection]);
            stat.Add(plugB[plugBSelection]);
        }
        return stat;
    }

    public Stat MaxStats()
    {
        Stat stat = new Stat();
        for (int i = 0; i < layers; i++)
        {
            stat.Add(maxStats);
        }
        return stat;
    }
}
