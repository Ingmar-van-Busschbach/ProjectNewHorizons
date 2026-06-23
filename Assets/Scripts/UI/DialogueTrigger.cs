using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData dialogue;

    public void TriggerDialogue()
    {
        DialogueWriter.Instance.InitializeDialogue(dialogue);
    }
}
