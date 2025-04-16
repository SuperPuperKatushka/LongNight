using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;
    public DialogueNode[] nodes;
}

[System.Serializable]
public class DialogueNode
{
    [TextArea(3, 10)]
    public string sentence;
    public DialogueChoice[] choices; // null если это просто фраза без выбора
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public int nextNodeIndex;
}
