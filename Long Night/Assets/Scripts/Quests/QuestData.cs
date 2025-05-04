using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest")]
public class QuestData : ScriptableObject
{
    public string questId;
    public string questName;
    public string description;
    public QuestObjective[] objectives;
    public bool sequential = true;
}
