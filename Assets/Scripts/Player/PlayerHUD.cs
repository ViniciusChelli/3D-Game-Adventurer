using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public GameObject canvas;
    public GameObject inventoryCanvas;

    public void ActiveCanvas(bool isActive)
    {
        canvas.SetActive(isActive);
    }

    public void ActivateInventory(bool isActive)
    {
        inventoryCanvas.SetActive(isActive);
    }
}
