using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Asset", menuName = "Quest Asset")]
public class QuestAsset : ScriptableObject
{
    [Header("Dialogue")]
    public DialogueAsset StartQuestDialogue;
    public DialogueAsset MidQuestDialogue;
    public DialogueAsset EndQuestDialogue;

    [Header("Items")]
    public List<Item> GivenItems;
    public List<Item> RequiredItems;

    [Header("Progression")]
    public QuestAsset NextQuest;
}
