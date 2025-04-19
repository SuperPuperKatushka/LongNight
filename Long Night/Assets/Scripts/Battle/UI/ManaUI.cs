using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ManaUI : MonoBehaviour
{
    public GameObject manaDotPrefab;
    public Sprite filledSprite;
    public Sprite emptySprite;

    private List<Image> manaDots = new List<Image>();

    public void UpdateMana(int current, int max)
    {
        // ������� ������
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        manaDots.Clear();

        // ������� �����
        for (int i = 0; i < max; i++)
        {
            GameObject dot = Instantiate(manaDotPrefab, transform);
            Image img = dot.GetComponent<Image>();

            if (i < current)
                img.sprite = filledSprite;
            else
                img.sprite = emptySprite;

            manaDots.Add(img);
        }
    }
}
