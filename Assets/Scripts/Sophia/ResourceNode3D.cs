using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourceNode3D : MonoBehaviour, IInteractable
{
    public string resourceType = "gem";
    public int remaining = 5;
    public float harvestTime = 0.7f;

    public Vector3 GetWorldPosition() => transform.position;
    public bool IsValid() => remaining > 0 && gameObject.activeInHierarchy;
    public float InteractionTime() => harvestTime;

    public void Interact(MonoBehaviour actor, NPCInventory inventory)
    {
        if (remaining <= 0) return;
        if (!inventory.CanAccept(resourceType, 1)) return;

        remaining--;
        inventory.Add(resourceType, 1);
        if (remaining <= 0) Destroy(gameObject);
    }
}
