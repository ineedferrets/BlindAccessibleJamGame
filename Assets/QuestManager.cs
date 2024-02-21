using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    static public QuestManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public QuestAsset CurrentQuest;

    enum QuestState
    {
        NotStarted,
        Ongoing,
        ReadyToHandIn
    }

    private QuestState _state;

    public void RunCurrentDialogue()
    {
        DialogueBoxController dialogueBoxController = DialogueBoxController.instance;

        if (dialogueBoxController == null) { return; }

        DialogueAsset dialogueAsset = CurrentQuest.StartQuestDialogue;
        switch(_state)
        {
            case QuestState.NotStarted:
                dialogueAsset = CurrentQuest.StartQuestDialogue;
                _state = QuestState.Ongoing;
                break;
            case QuestState.Ongoing:
                dialogueAsset = CurrentQuest.MidQuestDialogue;
                break;
            case QuestState.ReadyToHandIn:
                dialogueAsset = CurrentQuest.EndQuestDialogue;
                break;
        }

        dialogueBoxController.StartDialogue(dialogueAsset, 0);
    }
}
