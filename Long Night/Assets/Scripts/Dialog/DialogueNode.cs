using UnityEngine;

[System.Serializable]
public class DialogueNodeScript
{
    [TextArea(3, 10)]
    public string sentence;
    public DialogueChoice[] choices;
}
