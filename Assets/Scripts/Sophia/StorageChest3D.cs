using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StorageChest3D : MonoBehaviour, IInteractable
{
    public Vector3 GetWorldPosition() => transform.position;
    public bool IsValid() => gameObject.activeInHierarchy;
    public float InteractionTime() => 0.2f;

    public void Interact(MonoBehaviour actor, NPCInventory inventory)
    {
        var dumped = inventory.DumpAll();
        foreach (var kv in dumped)
            Debug.Log($"[Chest] Recebeu {kv.Value} x {kv.Key}");
    }
}
