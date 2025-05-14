using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue
{
    public DialogueNode[] nodes;
    [Header("Событие после завершения диалога")]
    public UnityEvent onDialogueEnd;
}


[System.Serializable]
public class DialogueCondition
{
    public string questId;
    public Quest.QuestState requiredState;
}

[System.Serializable]
public class DialogueNode
{
    public SpeakerData speaker;
    [TextArea(3, 10)] public string sentence;
    public DialogueChoice[] choices;
    public UnityEvent onNodeEnter;
}


[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public int nextNodeIndex;
}

[System.Serializable]
public class DialogueVariant
{
    public DialogueNode[] nodes;
    public DialogueCondition condition;
}



[System.Serializable]
public class DialogueOverride
{
    [Tooltip("Узлы диалога для этого варианта")]
    public DialogueNode[] nodes;

    [Tooltip("Условия для активации")]
    public DialogueCondition[] conditions;

    [Tooltip("Приоритет (чем выше, тем важнее)")]
    public int priority = 0;
}

