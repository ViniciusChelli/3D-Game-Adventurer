using UnityEngine;
using UnityEngine.EventSystems;

public class CustomizeRotationButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonRight;
    public float rotationSpeed;

    private bool isHolding;

    private float direction;

    private void Update()
    {
        if(isHolding)
        {
            if(buttonRight)
            {
                direction = 1f;
            }
            else
            {
                direction = -1f;
            }

            Player.Instance.transform.Rotate(0f, direction * rotationSpeed * Time.deltaTime, 0f);
        }
    }

    //é chamado ao clicar em um elemento de UI em que esteja com esse script
    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
    }

    //é chamado ao tirar o dedo do mouse
    public void OnPointerUp(PointerEventData eventData) 
    {
        isHolding = false;
    }
}
