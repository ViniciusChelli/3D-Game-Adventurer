using UnityEngine;

public interface IInteractable
{
    Vector3 GetWorldPosition();
    bool IsValid();
    float InteractionTime();
    void Interact(MonoBehaviour actor, NPCInventory inventory);
}
