using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float showTime = 2f;

    // ���������� ��������� � ������� � ������� (���� ����)
    public void ShowItemMessage(string text)
    {
        messageText.text = text;
        gameObject.SetActive(true);
        CancelInvoke(); // �������� ���������� Hide, ���� ���
        Invoke("Hide", showTime);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}