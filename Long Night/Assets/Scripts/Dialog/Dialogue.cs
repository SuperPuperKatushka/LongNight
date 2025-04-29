using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue
{
    public DialogueNode[] nodes;
    [Header("������� ����� ���������� �������")]
    public UnityEvent onDialogueEnd;
}

[System.Serializable]
public class DialogueNode
{
    public SpeakerData speaker;
    [TextArea(3, 10)]
    public string sentence;
    public DialogueChoice[] choices; // null ���� ��� ������ ����� ��� ������
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public int nextNodeIndex;
}
