using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public ItemData itemData;

    public Button slotBt;

    public GameObject tooltip;
    public TextMeshProUGUI tooltipDescText;

    //é chamado toda vez que o objeto é ativado na cena
    private void OnEnable()
    {
        if (itemData != null)
        {
            slotBt.gameObject.SetActive(true);
            slotBt.GetComponent<Image>().sprite = itemData.icon;
        }
        else
        {
            slotBt.gameObject.SetActive(false);
        }
    }

    public void UseItem()
    {
        Inventory.Instance.confirmBt.onClick.AddListener(ApplyItem);
    }

    //é chamado no botao do slot para ativar o efeito
    private void ApplyItem()
    {
        if(itemData != null)
        {
            itemData.Apply();
            slotBt.gameObject.SetActive(false);
            itemData = null;
            Inventory.Instance.ActivateInventory(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemData != null)
        {
            tooltip.SetActive(true);
            tooltipDescText.text = itemData.desc;
        }     
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(itemData != null)
        {
            tooltip.SetActive(false);
        }       
    }
}
