using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatStatDisplay : MonoBehaviour
{
    [SerializeField] private Slider resourcefulness;
    [SerializeField] private Slider athletics;
    [SerializeField] private Slider tempo;
    [SerializeField] private Slider smarts;
    [SerializeField] private TMP_Text ratNameText;

    public void DisplayStats(Stat ratStats, StatPlugs statPlugs, string ratName)
    {
        resourcefulness.value = ratStats.recourcefulness;
        athletics.value = ratStats.athletics;
        tempo.value = ratStats.tempo;
        smarts.value = ratStats.smarts;
        Stat maxStats = statPlugs.MaxStats();
        resourcefulness.maxValue = maxStats.recourcefulness;
        athletics.maxValue = maxStats.athletics;
        tempo.maxValue = maxStats.tempo;
        smarts.maxValue = maxStats.smarts;

        ratNameText.text = ratName;
    }
}
