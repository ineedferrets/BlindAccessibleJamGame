using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueSegment
{    
    [SerializeField]
    public string speakerName;

    [SerializeField, TextArea]
    public string dialogueText;

    [SerializeField, Tooltip("Number represents number of letters revealed per second.")]
    public float dialogueSpeed = 5;

    [SerializeField]
    public Image characterPortrait;

    [SerializeField]
    public bool portraitOnLeft;

    [SerializeField, Tooltip("This dialogue line triggers new song to be played")]
    public bool triggerNewMusic;

    [SerializeField, Tooltip("Name of new song triggered by this line(if Trigger New Music is enabled)")]
    public string triggerMusicTrack;
}

[CreateAssetMenu]
public class DialogueAsset : ScriptableObject
{
    public DialogueSegment[] dialogue;
}
