using UnityEngine;

public class ShouldDestroyCollectable : MonoBehaviour
{
    [SerializeField] private string itemID; // ���������� ID �������� (������� �������!)

    private void Start()
    {
        if (WasCollected())
            Destroy(gameObject);
    }

    // ���������� ��� ������� �������� (��������, �� ������� �������)
    public void MarkAsCollected()
    {
        PlayerPrefs.SetInt(itemID, 1); 
        Debug.Log(PlayerPrefs.HasKey(itemID));
    }

    // ���������, ��� �� ������� ������ �����
    private bool WasCollected()
    {
        return PlayerPrefs.HasKey(itemID) && PlayerPrefs.GetInt(itemID) == 1;
    }
}
