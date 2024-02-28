using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class DialogueBoxController : MonoBehaviour
{
    public static DialogueBoxController instance {  get; private set; }

    [Header("Dialogue UI Elements")]
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] CanvasGroup dialogueBox;

    public static event Action OnDialogueStarted;
    public static event Action OnDialogueEnded;
    bool skipLineTriggered;

    [Header("Fade To Black")]
    [SerializeField] Image blackoutImage;
    [SerializeField] float fadeDuration = 3f;
    public static event Action OnBlackoutComplete;
    private bool bFadeToBlack = false;
    private float fadeTimer;
    private Color m_blackoutImageOriginalColor;

    [Header("Music")]
    public MusicManager musicManager = MusicManager.Instance;

  
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

        for (int i = startPosition; i < dialogueAsset.dialogue.Length; i++)
        {
            DialogueSegment dialogueSegment = dialogueAsset.dialogue[i];
            nameText.text = dialogueSegment.speakerName;
            //checks if this dialogue line needs to trigger new music
            //if so, runs SetNewMusicTrack method in MusicManager
            if (dialogueSegment.triggerNewMusic == true)
            {
                Debug.Log("new music is " + dialogueSegment.triggerMusicTrack);
                musicManager.SetNewMusicTrack(dialogueSegment.triggerMusicTrack);
            }
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

    public void SkipLine(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            skipLineTriggered = true;
        }
    }

    public void StartBlackoutFadeIn()
    {
        if (!blackoutImage) { return; }

        fadeTimer = 0f;
        bFadeToBlack = true;
        blackoutImage.color = new Color(blackoutImage.color.r, blackoutImage.color.g, blackoutImage.color.b, 0f);
        blackoutImage.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (bFadeToBlack)
        {
            fadeTimer += Time.deltaTime;

            if (fadeTimer > fadeDuration) // done fading
            {
                fadeTimer = 0f;
                bFadeToBlack = false;

                blackoutImage.color = new Color(blackoutImage.color.r, blackoutImage.color.g, blackoutImage.color.b, 1f);

                OnBlackoutComplete.Invoke();
            }
            else // still fading
            {
                float alpha = Mathf.Lerp(0, 1, fadeTimer/fadeDuration);
                blackoutImage.color = new Color(blackoutImage.color.r, blackoutImage.color.g, blackoutImage.color.b, alpha);
            }
        }
    }
}
