using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject UISettingsMenu;
    [SerializeField] private GameObject UIInventoryMenu;
    [SerializeField] private GameObject UIDiaryMenu;
    [SerializeField] private GameObject UIExitHelpPanel;


    private void Update()
    {
        // �������� ������ ���� �� ����� ��������
        if (GameInput.Instance.IsDiaryPressed())
        {
            OpenMenu(UIDiaryMenu);
        }

        if (GameInput.Instance.IsInventoryPressed())
        {
            OpenMenu(UIInventoryMenu);
        }

        // ESC: ����������� ������� ���� (���������/��������� ��)
        if (GameInput.Instance.IsMenuPressed())
        {
            if (IsAnyMenuOpen())
                CloseAllMenus();
            else
                OpenMenu(UISettingsMenu);
        }
    }

    private bool IsAnyMenuOpen()
    {
        return UISettingsMenu.activeSelf ||
               UIInventoryMenu.activeSelf ||
               UIDiaryMenu.activeSelf;
    }

    public void OpenMenu(GameObject menu)
    {
        CloseAllMenus();
        menu.SetActive(true);
        UIExitHelpPanel.SetActive(true);
        Time.timeScale = 0; // �����
    }

    private void CloseAllMenus()
    {
            UISettingsMenu.SetActive(false);
            UIInventoryMenu.SetActive(false);
            UIDiaryMenu.SetActive(false);
            UIExitHelpPanel.SetActive(false);
            Time.timeScale = 1; // ������������ ����
    }
}
