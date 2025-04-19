using UnityEngine;

public class PlayerPositionData : MonoBehaviour
{
    public static PlayerPositionData Instance;

    public Vector2 savedPosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // сохраняется между сценами
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePosition(Vector3 position)
    {
        savedPosition = position;
    }

    public Vector2 GetSavedPosition()
    {
        return savedPosition;
    }
}
