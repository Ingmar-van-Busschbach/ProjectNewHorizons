using UnityEngine;

[System.Serializable]
public class Stat
{
    public int recourcefulness;
    public int athletics;
    public int tempo;
    public int smarts;

    public void Add(Stat stat)
    {
        recourcefulness += stat.recourcefulness;
        athletics += stat.athletics;
        tempo += stat.tempo;
        smarts += stat.smarts;
    }

    public void Debug()
    {
        UnityEngine.Debug.Log("Stats: " + recourcefulness + ", " + athletics + ", " + tempo + ", " + smarts);
    }
}
