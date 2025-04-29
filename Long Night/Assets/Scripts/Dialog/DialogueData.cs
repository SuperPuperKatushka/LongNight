using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public DialogueNode[] nodes;
    [Header("������� ����� ���������� �������")]
    public UnityEvent onDialogueEnd;
}
