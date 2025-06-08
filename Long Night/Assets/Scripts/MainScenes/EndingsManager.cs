using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingsManager : MonoBehaviour
{
    public GameObject GoodEndScreen;
    public GameObject BadEndScreen;

    public Button BackToMainScreenBad;
    public Button BackToMainScreenGood;


    private const string CHOSEN_SIDE_KEY = "ChoiceDialog_Side";
    private string chosenSide = ""; // "good" или "evil"

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BackToMainScreenBad.onClick.AddListener(OnBackToMainScreenClicked);
        BackToMainScreenGood.onClick.AddListener(OnBackToMainScreenClicked);

        chosenSide = PlayerPrefs.GetString(CHOSEN_SIDE_KEY, "");

        if (chosenSide == "good")
        {
            GoodEndScreen.SetActive(true);
        }
        else  if (chosenSide == "evil")
        {
            BadEndScreen.SetActive(true);
        }
    }

    private void OnBackToMainScreenClicked()
    {
        GameManager.Instance.NewGame();
        SceneManager.LoadScene("StartScene");
    }

}
