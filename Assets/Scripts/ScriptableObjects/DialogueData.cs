using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DialogueData", order = 1)]
public class DialogueData : ScriptableObject
{
    public StructLibrary.Struct_DialogueEntry[] dialogue;
    public bool endScene;
}
