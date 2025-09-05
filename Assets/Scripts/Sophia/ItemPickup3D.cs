using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup3D : MonoBehaviour, IInteractable
{
    public string itemType = "item";
    public int amount = 1;

    public Vector3 GetWorldPosition() => transform.position;
    public bool IsValid() => gameObject.activeInHierarchy;
    public float InteractionTime() => 0.05f;

    public void Interact(MonoBehaviour actor, NPCInventory inventory)
    {
        if (!inventory.CanAccept(itemType, amount)) return;
        inventory.Add(itemType, amount);
        Destroy(gameObject);
    }
}
