using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class DialogueBoxController : MonoBehaviour
{
    public static DialogueBoxController instance {  get; private set; }

    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] CanvasGroup dialogueBox;

    public static event Action OnDialogueStarted;
    public static event Action OnDialogueEnded;
    bool skipLineTriggered;

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

    public void StartDialogue(DialogueAsset dialogueAsset, int startPosition)
    {
        if (dialogueAsset == null){ return; }
        dialogueBox.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(RunDialogue(dialogueAsset, startPosition));
    }

    IEnumerator RunDialogue(DialogueAsset dialogueAsset, int startPosition)
    {
        skipLineTriggered = false;
        OnDialogueStarted?.Invoke();

        for(int i = startPosition; i < dialogueAsset.dialogue.Length; i++)
        {
            DialogueSegment dialogueSegment = dialogueAsset.dialogue[i];
            nameText.text = dialogueSegment.speakerName;
            StartCoroutine(TypeTextUncapped(dialogueSegment.dialogueText, dialogueSegment.dialogueSpeed));
            while (skipLineTriggered == false)
            {
                yield return null;
            }
            skipLineTriggered = false;
        }

        OnDialogueEnded?.Invoke();
        dialogueBox.gameObject.SetActive(false);
    }

    IEnumerator TypeTextUncapped(string line, float charsPerSecond)
    {
        float timer = 0f;
        float interval = 1 / charsPerSecond;
        string textBuffer = null;
        char[] chars = line.ToCharArray();
        int i = 0;

        while (i < chars.Length && skipLineTriggered == false)
        {
            if (timer < Time.deltaTime)
            {
                textBuffer += chars[i];
                dialogueText.text = textBuffer;
                timer += interval;
                i++;
            }
            else
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
    }

    public void SkipLine()
    {
        skipLineTriggered = true;
    }
}
