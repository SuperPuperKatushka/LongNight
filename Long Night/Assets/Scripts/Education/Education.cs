using UnityEngine;
using UnityEngine.EventSystems;
public class Education : MonoBehaviour
{
    public GameObject EducationUI;

    void Start()
    {
        string startOrContinue = PlayerPrefs.GetString("StartOrContinue");
        if (EducationUI != null)
        {
            if (startOrContinue != "Continue")
            {
                PlayerPrefs.SetString("StartOrContinue", "Continue");
                EducationUI.SetActive(true);
            }
            else
            {
                EducationUI.SetActive(false);
            }
        }
    }

   private void Update()
    {
        if (Input.GetMouseButtonDown(0) && EducationUI != null && EducationUI.activeSelf)
        {
            EducationUI.SetActive(false);
        }
    }
}


