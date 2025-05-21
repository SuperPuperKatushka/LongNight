using UnityEngine;

public class ShouldDestroyCollectable : MonoBehaviour
{
    [SerializeField] private string itemID; // Уникальный ID предмета (задаётся вручную!)

    private void Start()
    {
        if (WasCollected())
            Destroy(gameObject);
    }

    // Вызывается при подборе предмета (например, из другого скрипта)
    public void MarkAsCollected()
    {
        PlayerPrefs.SetInt(itemID, 1); 
        Debug.Log(PlayerPrefs.HasKey(itemID));
    }

    // Проверяет, был ли предмет собран ранее
    private bool WasCollected()
    {
        return PlayerPrefs.HasKey(itemID) && PlayerPrefs.GetInt(itemID) == 1;
    }
}
