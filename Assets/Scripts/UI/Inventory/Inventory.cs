using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryCanvas;
    public Button confirmBt;
    public GameObject container;

    private List<InventorySlot> slots = new List<InventorySlot>();

    public static Inventory Instance;

    private void Awake()
    {
        Instance = this;

        slots.Clear();

        foreach (InventorySlot slot in container.GetComponentsInChildren<InventorySlot>())
        {
            slots.Add(slot);
        }
    }

    public void AddItem(ItemData item)
    {
        foreach (InventorySlot slot in slots)
        {
            if(slot.itemData == null)
            {
                slot.itemData = item;
              //  Debug.Log(slot.gameObject.name);
                Debug.Log("Item adicionado: " + item.itemName);
                return;
            }
        }
    }

    public void ActivateInventory(bool isActive)
    {
        inventoryCanvas.SetActive(isActive);
    }

    public void RemoveApplyEffect()
    {
        confirmBt.onClick.RemoveAllListeners();
    }
}
