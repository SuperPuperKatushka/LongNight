using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Speaker Data")]
public class SpeakerData : ScriptableObject
{
    public string speakerName;
    public string speakerID;
    public GameObject avatar;
}
