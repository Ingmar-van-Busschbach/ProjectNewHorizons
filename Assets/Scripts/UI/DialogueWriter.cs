using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[RequireComponent(typeof(AudioSource))]
public class DialogueWriter : MonoBehaviour
{
    public static DialogueWriter Instance { get; private set; }
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float waitTillAutoNextDialogue = 3;
    [SerializeField] private UnityEvent endScene;
    [SerializeField] private InputActionReference clickInput;
    private InputAction interact;
    private DialogueData currentDialogue;
    private AudioSource audioSource;
    private int currentDialogueIndex;
    private Coroutine routine;
    private bool isPressed;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        audioSource = GetComponent<AudioSource>();
        #if UNITY_WEBGL
            EnhancedTouchSupport.Enable();
        #endif
    }

    private void OnDestroy()
    {
        Instance = null; // This is technically not needed as on scene loading it should automatically delete the Instance reference, but it is a precaution.
    }

    public void InitializeDialogue(DialogueData dialogueData)
    {
        currentDialogue = dialogueData;
        currentDialogueIndex = 0;
        WriteDialogue(currentDialogue.dialogue[currentDialogueIndex]);
    }

    private void Update()
    {
        if (currentDialogue == null)
        {
            return;
        }
    #if UNITY_EDITOR
        if (clickInput.action.WasPressedThisFrame())
        {
            NextDialogue();
        }

    #elif UNITY_WEBGL
        if (!isPressed && Touch.activeFingers.Count == 1)
        {            
            isPressed = true;
            NextDialogue();
        }
        if(Touch.activeFingers.Count == 0)
        { 
            isPressed = false; 
        }
    #endif
    }

    private void WriteDialogue(StructLibrary.Struct_DialogueEntry dialogueEntry)
    {
        nameText.text = dialogueEntry.speakerName;
        audioSource.clip = dialogueEntry.dialogueVoice;
        audioSource.Play();
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(PrintText(dialogueEntry));
    }

    private void NextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex >= currentDialogue.dialogue.Length)
        {
            StopCoroutine(routine);
            nameText.text = "";
            dialogueText.text = "";
            audioSource.Stop();
            if (currentDialogue.endScene)
            {
                endScene?.Invoke();
            }
            return;
        }
        WriteDialogue(currentDialogue.dialogue[currentDialogueIndex]);
    }



    private IEnumerator PrintText(StructLibrary.Struct_DialogueEntry dialogueEntry)
    {
        char[] letters = dialogueEntry.dialogue.ToCharArray();
        string displayText = "";
        foreach(char letter in letters)
        {
            yield return new WaitForSeconds(dialogueEntry.printDuration);
            displayText += letter;
            dialogueText.text = displayText;
        }
        yield return new WaitForSeconds(waitTillAutoNextDialogue);
        NextDialogue();
    }
}
