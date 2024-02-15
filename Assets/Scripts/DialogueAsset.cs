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
}

[CreateAssetMenu]
public class DialogueAsset : ScriptableObject
{
    public DialogueSegment[] dialogue;
}
