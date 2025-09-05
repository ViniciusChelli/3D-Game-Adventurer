using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeSlot : MonoBehaviour
{
    public CategoryType type;
    public Sprite icon;
    public Image iconImage;
    public TextMeshProUGUI categoryText;
    public TextMeshProUGUI amountText;

    private int currentIndex;
    private int totalParts;

    private void Start()
    {
        categoryText.text = type.ToString();

        iconImage.sprite = icon;

        UpdateParts();
    }

    public void Next()
    {
        currentIndex++;

        if (currentIndex >= totalParts)
            currentIndex = 0;

        UpdateParts();
    }

    public void Previous()
    {
        currentIndex--;

        if (currentIndex < 0)
            currentIndex = totalParts - 1;

        UpdateParts();
    }

    public void ResetToDefault()
    {
        currentIndex = 0;

        amountText.text = currentIndex.ToString() + "/" + totalParts.ToString();

        UpdateParts();
    }

    private void UpdateParts()
    {
        foreach (var t in Player.Instance.customizeBody.bodyParts)
        {
            if (t.category == type)
            {
                totalParts = t.parts.Count;

                //executar a lógica de ativar o objeto atual
                for (int i = 0; i < t.parts.Count; i++)
                {
                    t.parts[i].SetActive(i == currentIndex);
                }
            }
        }

        amountText.text = currentIndex.ToString() + "/" + totalParts.ToString();
    }
}
