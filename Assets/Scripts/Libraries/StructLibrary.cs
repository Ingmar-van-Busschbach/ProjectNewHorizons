using UnityEngine;

public class StructLibrary : MonoBehaviour
{ 
    [System.Serializable]
    public struct Struct_DialogueEntry
    {
        [Tooltip("Who is speaking?")]
        public string speakerName;
        [Tooltip("What are they saying?")]
        public string dialogue;
        [Tooltip("How long does it take for the each individal leter to print onto the screen?")]
        public float printDuration;
        public AudioClip dialogueVoice;
    }

    [System.Serializable]

    public struct Struct_RandomAudioEntry
    {
        public AudioClip audioClip;
        public float volume;
        public float weight;
    }
}
