// DialogueData.cs
using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public DialogueNode[] defaultNodes;
    public DialogueOverride[] overrides;
    public UnityEvent onDialogueEnd;
}




